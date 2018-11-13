using System.Collections;
using System.Collections.Generic;
using InteractiveStorytellingSystem;
using UnityEngine;

public static class GameManager 
{
	public static Character FindCharacter(string name)
	{
		return GameObject.Find(name).GetComponent<Character>();
	}

	public static void AddActionToEventManager(Action action)
	{
		GameObject.Find("EventManager").GetComponent<EventManager>().AddAction(action);
	}
}
