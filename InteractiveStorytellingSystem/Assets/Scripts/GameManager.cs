using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using InteractiveStorytellingSystem;
using UnityEngine;
using System.Linq;

public static class GameManager 
{
	public static string[] SplitParameterString(string effect)
	{
		if(effect != "")
		{
			int spaces = effect.Count(char.IsWhiteSpace);
			if(spaces == 2)
			{
				return effect.Split(' ');
			}
			else
			{
				Debug.Log("Error! Tried to parse a malformed action parameter string: " + effect);
			}
		}
		return null;
	}

	public static void ChangeStat(string characterName, string statName, float value)
	{
		statName = statName.ToLower();
		switch(statName)
		{
			case "hunger":
				GameObject.Find(characterName)
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
				result = GameObject.Find(characterName)
					.GetComponent<PhysicalResourceModel>()
					.Hunger;
				break;
			case "anger":
				result = 0.6f;
				break;
			case "happiness":
				result = 0.0f;
				break;
		}
		return result;
	}

	public static bool IsParameterTrue(string characterName, string parameter)
	{
		string[] split = SplitParameterString(parameter);
		if(split != null)
		{
			if(split[1] == "lt")
			{
				float actualValue = GetStat(characterName, split[0]);
				if(actualValue <= 1.0f && actualValue >= 0.0f)
				{
					if(actualValue < float.Parse(split[2], CultureInfo.InvariantCulture.NumberFormat))
						return true;
				}
			}
			else if(split[1] == "gt")
			{
				float actualValue = GetStat(characterName, split[0]);
				if(actualValue <= 1.0f && actualValue >= 0.0f)
				{
					if(actualValue > float.Parse(split[2], CultureInfo.InvariantCulture.NumberFormat))
						return true;
				}
			}
			else if(split[0] == "inventory" && split[1] == "contains")
			{
				if(GameObject.Find(characterName).GetComponent<Inventory>().Contains(split[2]))
					return true;
			}
			else if(split[0] == "location" && split[1] == "at")
			{
				if(GameObject.Find(characterName).GetComponent<MovementManager>().CheckIfAtLocation(GameObject.Find(split[2]).transform))
					return true;
			}
		}
		return false;
	}
}
