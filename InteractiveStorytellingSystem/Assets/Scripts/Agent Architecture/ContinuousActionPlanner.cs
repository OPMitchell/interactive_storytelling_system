using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using System.Linq;
using InteractiveStorytellingSystem.ConfigReader;

// The Continuous Action Planner (CAP) is used by each agent to construct plan; a series of actions that, when executed, achieve a goal.
// The CAP is "continuous" because it is able to react to real-time changes in the game-world state, identifying if/when the constituent actions
// comprising a plan fail to execute, and re-plan accordingly. This is known as "action repair".
public class ContinuousActionPlanner : MonoBehaviour 
{
	// A collection of goals that the agent wishes to accomplish.
	[SerializeField] private TextAsset GoalsFile;
	public List<Goal> Goals {get; private set;}
	private ActionDirectory actionDirectory;

	// Maximum number of times a goal can be replanned.
	const int maxFail = 3;

	// Used for initialisation.
	void Start()
	{
		CreateGoalsList();
		actionDirectory = GetComponent<ActionDirectory>();
	}

	// Get initial goals from xml file and populate the collection.
	private void CreateGoalsList()
    {
        this.Goals = ConfigReader.ReadGoals(GoalsFile.name + ".xml");
    }

	// Adds the specified goal to the collection of goals
	public void AddGoal(Goal g)
	{
		if(g != null)
			Goals.Add(g);
	}

	// Creates a collection of plans and returns it. Each LinkedList of actions represents one plan for achieving the specified goal.
	private void CreatePlans(Goal g, string target, string parameters, PlanList plans, Plan p, List<Action> ignoreList)
	{
		List<Action> matches = actionDirectory.FindActionsSatisfyingPrecondition(target, parameters);
		RemovePreviouslyFailedActions(matches, ignoreList);
		foreach(Action action in matches)
		{
			Plan newPlan = new Plan(p);
			newPlan.AddAction(action);
			if(!action.HasPrecondition() || action.IsPreconditionSatisfied())
			{
				plans.Add(newPlan);
			}
			else
			{
				string nextTarget = transform.name;
				string[] split = GameManager.SplitParameterString(action.Precondition);
				if(split[0] == "location")
					nextTarget = split[2];		
				CreatePlans(g, nextTarget, action.Precondition, plans, newPlan, ignoreList);
			}
		}
	}

	private void RemovePreviouslyFailedActions(List<Action> actionList, List<Action> ignoreList)
	{
		actionList.RemoveAll(x => ignoreList.Any(a => a.Compare(x)));
	}

	private bool ReplaceAction(Action action, List<Action> ignoreList)
	{
		List<Action> actions = actionDirectory.FindActionsByEffect(action.Target, action.Effect);
		RemovePreviouslyFailedActions(actions, ignoreList);
		if(actions.Count > 0)
		{
			action.Replace(actions[Random.Range(0, actions.Count)]);
			action.SetStatus(Status.notSent);
			return true;
		}
		return false;
	}

	// Called every frame.
	void Update()
	{
		// Iterate through all the agent's goals.
		for (int i = 0; i < Goals.Count; i++)
		{
			// Get a reference to the current goal.
			Goal g = Goals[i];
			
			// If the goal is complete, remove it from the collection.
			if(g.Complete)
			{
				Goals.Remove(g);
			}
			
			// If the goal isn't complete, we need to check if a plan has been made or if one is currently executing.
			else
			{
				// If no plan has been made yet we need to construct one.
				if(g.Plan == null)
				{
					// Create a LinkedList to store the plans and populate it.
					PlanList planList = new PlanList();
					CreatePlans(g, g.Target, g.Parameters, planList, new Plan(), g.FailedActions);
					// If there are no plans in the collection then the goal is already satisifed or its impossible to reach.
					if(planList.Count() < 1)
					{
						// Set the goal to complete and it will be removed from the collection the next time Update() is run.
						g.Complete = true;
						Testing.WriteToLog(transform.name, "cancelled goal: " + g.Parameters + " because a plan is impossible to make.");
					}
					// If we successfully constructed one or more plans then we need to pick one for the agent to use.
					else
					{
						Testing.WriteToLog(transform.name, "created a plan for goal: " + g.Parameters);
						//pick a plan from List<LinkedList<Action>> plans and assign to g.plan
						g.SetPlan(planList.GetBestPlan());
					}
				}
				// If a plan has already been constructed we need to monitor its progress.
				else
				{
					List<Action> toRemove = new List<Action>();
					foreach(Action action in g.Plan.GetActions())
					{
						// If the action has been sent, wait for it to change status (succeed or fail) before continuing with any other actions.
						if(action.Status == Status.Sent)
						{
							break;
						}
						// If the action has executed successfully, remove it from the plan.
						else if(action.Status == Status.Successful)
						{
							toRemove.Add(action);
						}
						// If the action has failed to execute, we need to perform action repair and select an alternate action or a new plan altogether.
						else if(action.Status == Status.Failed)
						{
							g.AddFailedAction(action);
							if(!ReplaceAction(action, g.FailedActions))
							{
								Testing.WriteToLog(transform.name, "Could not find a replacement action for failed action: " + Testing.GetActionInfo(action) + ". Attempting to replan...");
								g.TimesFailed++;
								if(g.TimesFailed >= maxFail)
								{
									Testing.WriteToLog(transform.name, "Exceeded maximum replan allowance for goal: " + g.Parameters);
									g.Complete = true;
								}
								else
								{
									g.SetPlan(null);
								}
							}
							else
							{
								Testing.WriteToLog(transform.name, "Found replacement action: " + Testing.GetActionInfo(action));
							}
							break;
						}
						// if the action has not yet been sent, send it to the agent's action queue.
						else if(action.Status == Status.notSent)
						{
							GetComponent<ActionQueue>().QueueAction(action);
							action.SetStatus(Status.Sent);
							break;
						}
						else if(action.Status == Status.Interrupted)
						{
							action.SetStatus(Status.Sent);
							break;
						}
					}
					foreach(Action action in toRemove)
					{
						g.Plan.RemoveAction(action);
					}
					// If all actions have been executed then the goal has been reached and can be removed.
					if(g.Plan != null && g.Plan.CountActions() <= 0)
					{
						Testing.WriteToLog(transform.name, "Completed goal: " + g.Parameters);
						g.Complete = true;
					}
				}
			}
		}

	}

}
