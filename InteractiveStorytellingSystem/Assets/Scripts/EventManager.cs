using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class EventManager : MonoBehaviour
{
	private EventPriorityQueue eventQueue = new EventPriorityQueue();

	public void Start()
	{
		Debug.Log("First Event at " + Time.realtimeSinceStartup);
		//add the first ever game action here to start the game!
		AddAction(GameManager.FindCharacter("Squirtle").ActionList[0]);
	}

	public void Update()
	{
		if(!eventQueue.IsEmpty())
		{
			RemoveAction();
		}
	}

	public void AddAction(Action action)
	{
		eventQueue.Add(1, action); //TODO: implement priority system!
	}

	public void RemoveAction()
	{
		Action action = eventQueue.Remove(); //Remove action from event queue
		ActionExecutor.ExecuteAction(action);//Execute the action
		GameManager.FindCharacter(action.Target).SendAction(action); //Send action to target for memory storage and response
	}
}
