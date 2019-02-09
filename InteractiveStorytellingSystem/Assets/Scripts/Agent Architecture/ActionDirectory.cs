using System.Collections;
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

	public List<Action> GetActionsThatSatisfyPrecondition(string target, string precondition)
	{
		string[] split = GameManager.SplitParameterString(precondition);
		if(split[1] == "lt")
		{
			return GetActionsByParameterAndOperation(target, split[0], "-");
		}
		else if(split[1] == "gt")
		{
			return GetActionsByParameterAndOperation(target, split[0], "+");
		}
		else if(split[0] == "inventory" && split[1] == "contains")
		{
			return GetActionsByParameterOperationAndValue(target, split[0], "contains", split[2]);
		}
		else if(split[0] == "location" && split[1] == "at")
		{
			return GetActionsByParameterOperationAndValue(target, split[0], "at", split[2]);
		}
		else
		{
			Debug.Log("Error! Tried to parse an action precondition which doesn't contain a valid operator!");
			return null;
		}
	}

	private List<Action> GetActionsByParameterOperationAndValue(string target, string parameter, string operation, string value)
	{
		List<Action> result = new List<Action>();
		foreach(Action action in ActionList)
		{
			string[] s = null;
			if(action.Target == target)
			{
				if(target == transform.name)
					s = GameManager.SplitParameterString(action.SenderEffect);
				else
					s = GameManager.SplitParameterString(action.TargetEffect);
			}
			if(s != null)
			{
			if(s[0] == parameter && s[1] == operation && s[2] == value)
				result.Add(new Action(action));
			}
		}
		return result;
	}

	private List<Action> GetActionsByParameterAndOperation(string target, string parameter, string operation)
	{
		List<Action> result = new List<Action>();
		foreach(Action action in ActionList)
		{
			string[] s = null;
			if(action.Target == target)
			{
				if(target == transform.name)
					s = GameManager.SplitParameterString(action.SenderEffect);
				else
					s = GameManager.SplitParameterString(action.TargetEffect);
			}
			if(s != null)
			{
				if(s[0] == parameter && s[1] == operation)
					result.Add(new Action(action));
			}
		}
		return result;
	}

	public List<Action> GetActionsBySenderEffect(Action action)
	{
		List<Action> result = new List<Action>();
		if(action.SenderEffect != "")
		{
			string[] x = GameManager.SplitParameterString(action.SenderEffect);
			foreach(Action candidate in ActionList)
			{
				if(candidate.Target == transform.name)
				{
					if(candidate.SenderEffect != "" && (GameManager.IsParameterTrue(transform.name, candidate.Precondition) || candidate.Precondition == ""))
					{
						string[] y = GameManager.SplitParameterString(candidate.SenderEffect);
						if(x[0] == y[0] && x[1] == y[1])
							result.Add(new Action(candidate));
					}
				}
			}
		}
		return result;
	}

	public List<Action> GetActionsByTargetEffect(Action action)
	{
		List<Action> result = new List<Action>();
		if(action.TargetEffect != "")
		{
			string[] x = GameManager.SplitParameterString(action.TargetEffect);
			foreach(Action candidate in ActionList)
			{
				if(candidate.Target == action.Target)
				{
					if(candidate.TargetEffect != "" && (GameManager.IsParameterTrue(transform.name, candidate.Precondition) || candidate.Precondition == ""))
					{
						string[] y = GameManager.SplitParameterString(candidate.TargetEffect);
						if(x[0] == y[0] && x[1] == y[1])
							result.Add(new Action(candidate));
					}
				}
			}
		}
		return result;
	}

}
