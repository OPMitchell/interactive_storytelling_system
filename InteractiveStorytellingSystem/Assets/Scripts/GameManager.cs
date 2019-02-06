using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

	public static void ChangeStat(string characterName, string statName, float value)
	{
		statName = statName.ToLower();
		switch(statName)
		{
			case "hunger":
				FindCharacter(characterName)
					.GetComponent<PhysicalResourceModel>()
					.Hunger += value;
				break;
		}
	}

	private static float GetStat(string characterName, string statName)
	{
		statName = statName.ToLower();
		float result = float.MinValue;
		switch(statName)
		{
			case "hunger":
				result = FindCharacter(characterName)
					.GetComponent<PhysicalResourceModel>()
					.Hunger;
				break;
		}
		return result;
	}

	public static bool IsParameterTrue(string characterName, string parameter)
	{
		string[] split = SplitEffectString(parameter);
		if(split != null)
		{
			float actualValue = GetStat(characterName, split[0]);
			if(split[1] == "lt")
			{
				if(actualValue < float.Parse(split[2], CultureInfo.InvariantCulture.NumberFormat))
					return true;
			}
			else if(split[1] == "gt")
			{
				if(actualValue > float.Parse(split[2], CultureInfo.InvariantCulture.NumberFormat))
					return true;
			}
		}
		return false;
	}
}
