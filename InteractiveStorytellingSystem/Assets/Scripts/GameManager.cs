using System.Collections;
using System.Collections.Generic;
using InteractiveStorytellingSystem;
using UnityEngine;
using System.Linq;

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

	public static string[] SplitEffectString(string effect)
	{
		int spaces = effect.Count(char.IsWhiteSpace);
		if(spaces == 2)
		{
			return effect.Split(' ');
		}
		else
		{
			Debug.Log("Error! Tried to parse a malformed action effect!");
			return null;
		}
	}

	public static string GetActionInfo(Action action)
	{
		return "Action(name = " + action.Name + ", sender = " + action.Sender + ", target = " + action.Target +")";
	}
}
