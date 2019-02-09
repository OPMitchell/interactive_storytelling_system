﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using InteractiveStorytellingSystem.ConfigReader;
using System.Linq;

public class ActionDirectory : MonoBehaviour 
{
	[SerializeField] private TextAsset ActionListFile;
	public List<Action> ActionList {get; private set;}

	void Awake()
	{
		CreateActionList();
	}

	private void CreateActionList()
    {
        this.ActionList = ConfigReader.ReadActionList(ActionListFile.name + ".xml");
    }

	public Action GetActionByIndex(int index)
	{
		if(index < ActionList.Count)
			return ActionList[index];
		else
			Debug.LogError("Error! " + transform.name + " tried to access an action with an index outside the ActionDirectory's bounds!");
		return null;
	}

	public List<Action> FindActionsByEffect(string target, string effect)
	{
		string[] split = GameManager.SplitParameterString(effect);
		return FindActionsByParameterAndOperation(target, split[0], split[1]);
	}

	private List<Action> FindActionsByParameterAndOperation(string target, string parameter, string operation)
	{
		return ActionList.Where(
			x => string.Equals(GameManager.SplitParameterString(x.Effect)[0], parameter)
			&& string.Equals(GameManager.SplitParameterString(x.Effect)[1], operation)
			&& string.Equals(x.Target, target)
		).ToList();
	}

	private List<Action> FindActionsByParameterOperationAndValue(string target, string parameter, string operation, string value)
	{
		return ActionList.Where(
			x => string.Equals(GameManager.SplitParameterString(x.Effect)[0], parameter)
			&& string.Equals(GameManager.SplitParameterString(x.Effect)[1], operation)
			&& string.Equals(GameManager.SplitParameterString(x.Effect)[2], value)
			&& string.Equals(x.Target, target)
		).ToList();
	}

	public List<Action> FindActionsSatisfyingPrecondition(string target, string precondition)
	{
		List<Action> result = null;
		string[] split = GameManager.SplitParameterString(precondition);
		switch(split[1])
		{
			case "lessthan":
				result = FindActionsByParameterAndOperation(target, split[0], "-");
			break;
			case "greaterthan":
				result = FindActionsByParameterAndOperation(target, split[0], "+");
			break;
			case "contains":
				result = FindActionsByParameterOperationAndValue(target, split[0], "contains", split[2]);
			break;
			case "at":
				result = FindActionsByParameterOperationAndValue(target, split[0], "at", split[2]);
			break;
		}
		return result;
	}
}
