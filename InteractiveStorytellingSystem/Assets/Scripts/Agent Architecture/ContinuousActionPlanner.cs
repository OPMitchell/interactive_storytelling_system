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
		AddGoal(new Goal(GoalType.Interest, "hunger > 0.5"));
	}

	public void AddGoal(Goal g)
	{
		Goals.Add(g);
	}

	private void CreatePlan(Goal g)
	{
		List<Action> actions = GetComponent<ActionDirectory>()
			.GetActionsByEffect(g.Parameters);
		if(actions.Count > 0)
		{
			g.Plan.Push(actions[0]);
			Debug.Log(transform.name + " has developed a plan for a goal:" +
			"\n		Goal: " + g.Parameters +
			"\n		Plan:" +
			"\n			Action: " + actions[0].Name + ", Effect = " + actions[0].Effect
			);
			g.HasPlan = true;
		}
	}

	void Update()
	{
		if(Goals.Count > 0)
		{
			foreach(Goal g in Goals)
			{
				if(g.Complete)
					Goals.Remove(g);
				else
				{
					if(!g.HasPlan)
						CreatePlan(g);
					else
					{
						while(g.Plan.Count > 0)
							GetComponent<ActionQueue>().QueueAction(g.Plan.Pop());
					}
				}
			}
		}
	}

}
