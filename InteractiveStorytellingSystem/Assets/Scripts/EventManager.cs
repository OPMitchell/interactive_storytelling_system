using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class EventManager : MonoBehaviour
{
	private EventPriorityQueue eventQueue = new EventPriorityQueue();
	private CharacterManager characterManager;

	public void Awake()
	{
		characterManager = GetComponent<CharacterManager>();
	}	

	public void Start()
	{
		//add the first ever game action here to start the game!
		AddAction(GameObject.Find("Squirtle").GetComponent<Character>().ActionList[0]);
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
		//characterManager.FindCharacterByName(action.Target).SendAction(action); //Send to target for response
		GameObject.Find(action.Target).GetComponent<Character>().SendAction(action);
	}
}
