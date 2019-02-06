using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class ContinuousActionPlanner : MonoBehaviour 
{
	public List<Goal> Goals {get; private set;}

	void Start()
	{
		Goals = new List<Goal>();
		AddGoal(new Goal(GoalType.Interest, "hunger lt 0.5"));
	}

	public void AddGoal(Goal g)
	{
		Goals.Add(g);
	}

	private List<Stack<Action>> CreatePlans(Goal g)
	{
		List<Stack<Action>> plans = new List<Stack<Action>>();
		if(!GameManager.IsParameterTrue(transform.name, g.Parameters))
		{
			List<Action> actions = GetComponent<ActionDirectory>()
				.GetActionsByEffect(g.Parameters);
			foreach(Action action in actions)
			{
				Stack<Action> plan = new Stack<Action>();
				plan.Push(action);
				if(!action.HasPrecondition())
				{
					plans.Add(plan);
				}
				else
				{
					AddToPlan(plan, plans, action.Precondition);
				}
			}
		}
		return plans;
	}

	private void AddToPlan(Stack<Action> plan, List<Stack<Action>> plans, string parameters)
	{
		List<Action> actions = GetComponent<ActionDirectory>()
			.GetActionsByEffect(parameters);
		foreach(Action action in actions)
		{
			Stack<Action> newPlan = new Stack<Action>();
			foreach(Action a in plan)
			{
				newPlan.Push(a);
			}
			newPlan.Push(action);
			if(!action.HasPrecondition() || GameManager.IsParameterTrue(transform.name, parameters))
			{
				plans.Add(newPlan);
			}
			else
			{
				AddToPlan(newPlan, plans, action.Precondition);
			}
		}
	}

	void Update()
	{
		List<Goal> toRemove = new List<Goal>();
		foreach(Goal g in Goals)
		{
			if(g.Complete)
				toRemove.Add(g);
			else
			{
				if(g.Plan == null)
				{
					List<Stack<Action>> plans = CreatePlans(g);
					if(plans.Count < 1)
					{
						g.Complete = true;
						Debug.Log(transform.name + " cancelled Goal: " + g.Parameters + " because a plan is impossible to make or goal is already satisfied.");
					}
					else
					{
						//pick a plan from List<Stack<Action>> plans and assign to g.plan
						g.SetPlan(plans[1]);
					}
				}
				else
				{
					while(g.Plan.Count > 0)
						GetComponent<ActionQueue>().QueueAction(g.Plan.Pop());
				}
			}
		}
		foreach(Goal g in toRemove)
		{
			Goals.Remove(g);
		}
	}

}
