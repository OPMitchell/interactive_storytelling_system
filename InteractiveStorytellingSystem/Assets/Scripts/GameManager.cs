using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using InteractiveStorytellingSystem;
using UnityEngine;
using System.Linq;
using System;

public static class GameManager 
{
	public static GameObject FindGameObject(string name)
	{
		return GameObject.Find(name);
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
		if(statname == "hunger")
		{
			result = FindGameObject(target).GetComponent<PhysicalResourceModel>().Hunger;
		}
		else if(statname == "happiness")
		{
			result = 0.0f;
		}
		return (T)Convert.ChangeType(result, typeof(T));
	}

	public static List<GameObject> GetAllCharacters()
	{
		List<GameObject> characters = GameObject.FindGameObjectsWithTag("Character").ToList();
		return characters;
	}
}
