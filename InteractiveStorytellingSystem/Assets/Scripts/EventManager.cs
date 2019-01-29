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
		AddAction(GameManager.FindCharacter("Rachel").ActionList[1]);
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
		ExecuteAction(action);
	}

	private void ExecuteAction(Action action)
	{
		GameObject.Find(action.Sender).GetComponent<ActionExecutor>().ExecuteAction(action);//Execute the action
	}
}
