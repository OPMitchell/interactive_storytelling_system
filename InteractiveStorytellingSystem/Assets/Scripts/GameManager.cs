using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using InteractiveStorytellingSystem;
using UnityEngine;
using System.Linq;
using System;

public static class GameManager 
{
	public static Transform FindGameObject(string name)
	{
		return GameObject.Find(name).transform;
	}

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
		return new string[3];
	}

	public static T GetStat<T>(string target, string statname)
	{
		object result = null;
		switch(statname)
		{
			case "hunger": 
				result = FindGameObject(target).GetComponent<PhysicalResourceModel>().Hunger;
			break;
			case "anger": 
				result = 0.6f;
			break;
			case "happiness":
				result = 0.0f;
			break;
		}
		return (T)Convert.ChangeType(result, typeof(T));
	}
}
