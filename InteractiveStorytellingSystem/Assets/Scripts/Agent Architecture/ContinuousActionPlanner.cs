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

	// Maximum number of times a goal can be replanned.
	const int maxFail = 3;

	// Used for initialisation.
	void Start()
	{
		CreateGoalsList();
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
	private List<LinkedList<Action>> CreatePlans(Goal g)
	{
		// Initialise a collection of plans.
		List<LinkedList<Action>> plans = new List<LinkedList<Action>>();
		// Add a single empty plan to the collection.
		LinkedList<Action> plan = new LinkedList<Action>();
		// Call function to recursively create all possible plans.
		// Specify the parameters string value as the desired goal's effects.
		AddToPlan(plan, plans, g.Target, g.Parameters, g.FailedActions);
		return plans;
	}

	private void RemovePreviouslyFailedActions(List<Action> actions, List<Action> ignore)
	{
		for(int i = 0; i < actions.Count; i++)
		{
			for(int j = 0; j < ignore.Count; j++)
			{
				if(actions[i].Compare(ignore[j]))
					actions.Remove(actions[i]);
			}
		}
	}

	// Recursively constructs individual plans and adds them to the specified collection.
	// Receives a collection of plans, an individual under-construction plan, and the parameter pre-condition for the action at
	// the top of the under-construction plan.
	private void AddToPlan(LinkedList<Action> plan, List<LinkedList<Action>> plans, string target, string parameters, List<Action> ignore)
	{
		// Create a collection of all actions which, when executed, will satisfy the given parameters.
		List<Action> actions = GetComponent<ActionDirectory>()
			.GetActionsThatSatisfyPrecondition(target, parameters);
		// Removes any actions from the collection that have previously failed in this plan.
		RemovePreviouslyFailedActions(actions, ignore);
		// Iterate through the collection of actions and construct a plan.
		foreach(Action action in actions)
		{
			// Create a new LinkedList to store the plan's series of actions.
			LinkedList<Action> newPlan = new LinkedList<Action>();
			// Copy all the actions from the specified under-construction plan to the new LinkedList.
			foreach(Action a in plan)
			{
				newPlan.AddFirst(a);
			}
			// Finally, add the action currently being evaluated onto the front of the LinkedList plan.
			newPlan.AddFirst(action);
			// If the action's precondition is blank or is already satisfied then the plan is fully constructed and we can add
			// it to the collection of plans.
			if(!action.HasPrecondition() || GameManager.IsParameterTrue(action.Target, action.Precondition))
			{
				// Add the finished plan to the collection.
				plans.Add(newPlan);
			}
			// If the action's precondition isn't blank and isn't already satisfied, call the function again to continue construction.
			else
			{
				AddToPlan(newPlan, plans, transform.name, action.Precondition, ignore);
			}
		}
	}

	// Attempts to replace the specified failed action with a different action that produces the same effect.
	private Action GetReplacementAction(Action action, List<Action> ignore)
	{
		Action failedAction = new Action(action);
		// Get a list of all actions which produce the same effect as the failed action.
		List<Action> actions = new List<Action>();
		if(failedAction.Target == transform.name)
		{
			actions = GetComponent<ActionDirectory>()
				.GetActionsBySenderEffect(failedAction);
		}
		else
		{
			actions = GetComponent<ActionDirectory>()
				.GetActionsByTargetEffect(failedAction);
		}
		RemovePreviouslyFailedActions(actions, ignore);
		// If one or more valid actions were found.
		if(actions.Count > 0)
		{
			// Pick a random action out of those found.
			failedAction = actions[Random.Range(0, actions.Count)];
			Debug.Log("Replacement found!");
			return failedAction;
		}
		// If no valid actions were found return null;
		return null;
	}

	// Takes a list of plans and orders them by the number of their constituent actions
	private List<LinkedList<Action>> OrderPlansByLength(List<LinkedList<Action>> plans)
	{
		 return plans.OrderBy(x => x.Count).ToList();
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
					List<LinkedList<Action>> plans = CreatePlans(g);
					// If there are no plans in the collection then the goal is already satisifed or its impossible to reach.
					if(plans.Count < 1)
					{
						// Set the goal to complete and it will be removed from the collection the next time Update() is run.
						g.Complete = true;
						Debug.Log(transform.name + " cancelled Goal: " + g.Parameters + " because a plan is impossible to make or goal is already satisfied.");
					}
					// If we successfully constructed one or more plans then we need to pick one for the agent to use.
					else
					{
						//pick a plan from List<LinkedList<Action>> plans and assign to g.plan
						plans = OrderPlansByLength(plans);
						g.SetPlan(plans[0]);
					}
				}
				// If a plan has already been constructed we need to monitor its progress.
				else
				{
					// If there are still actions that haven't been executed we need to execute them.
					if(g.Plan.Count > 0)
					{
						List<Action> toRemove = new List<Action>();
						// Iterate through all the remaining actions that need to be executed.
						foreach(Action action in g.Plan)
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
								break;
							}
							// If the action has failed to execute, we need to perform action repair and select an alternate action or a new plan altogether.
							else if(action.Status == Status.Failed)
							{
								// Keep a record of the action that has failed so it is not chosen again.
								g.AddFailedAction(action);
								// Find new action with same effect to replace the failed action.
								Action replacement = GetReplacementAction(action, g.FailedActions);
								// If a suitable replacement is found:
								if(replacement != null)
								{
									// Replace the failed action with the new action.
									action.Replace(replacement);
									// Set the status to not sent so that it will be re-sent in the next frame.
									action.SetStatus(Status.notSent);
									break;
								}
								// If no action could be found then create a new plan
								else
								{
									// If we haven't exceed the re-plan limit, set the plan to null so that it is remade in the next frame.
									if(g.TimesFailed < maxFail)
									{
										Debug.Log("No alternate action could be found! Creating new plan!");
										g.SetPlan(null);
										g.TimesFailed++;
										break;
									}
									// If we have already tried to create a new plan more times than allowed, cancel the goal.
									else
									{
										g.Complete = true;
										Debug.Log("Attempts to make plan exceeded limit. Cancelling goal.");
										break;
									}
								}
							}
							// if the action has not yet been sent, send it to the agent's action queue.
							else if(action.Status == Status.notSent)
							{
								GetComponent<ActionQueue>().QueueAction(action);
								action.SetStatus(Status.Sent);
								break;
							}
						}
						// Remove all actions that have executed.
						foreach(Action action in toRemove)
						{
							g.Plan.Remove(action);
						}
					}
					// If all actions have been executed then the goal has been reached and can be removed.
					else
					{
						Debug.Log(transform.name + " completed Goal: " + g.Parameters + "!");
						g.Complete = true;
					}
				}
			}
		}

	}

}
