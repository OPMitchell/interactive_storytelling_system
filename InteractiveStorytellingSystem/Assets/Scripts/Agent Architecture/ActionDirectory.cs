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

	public List<Action> GetActionsByPrecondition(string effect)
	{
		string[] split = GameManager.SplitEffectString(effect);
		if(split[1] == "lt")
		{
			return GetActionsByParameterAndOperation(split[0], "-");
		}
		else if(split[1] == "gt")
		{
			return GetActionsByParameterAndOperation(split[0], "+");
		}
		else
		{
			Debug.Log("Error! Tried to parse an action precondition which doesn't contain a '<' or '>'!");
			return null;
		}
	}

	private List<Action> GetActionsByParameterAndOperation(string parameter, string operation)
	{
		List<Action> result = new List<Action>();
		foreach(Action action in ActionList)
		{
			if(action.Effect != "")
			{
				string[] s = GameManager.SplitEffectString(action.Effect);
				if(s[0] == parameter && s[1] == operation)
					result.Add(new Action(action));
			}
		}
		return result;
	}

	public List<Action> GetActionsByEffect(Action action)
	{
		string[] x = GameManager.SplitEffectString(action.Effect);
		List<Action> result = new List<Action>();
		foreach(Action a in ActionList)
		{
			if(a.Effect != "")
			{
				string[] y = GameManager.SplitEffectString(a.Effect);
				if(x[0] == y[0] && x[1] == y[1])
					result.Add(new Action(a));
			}
		}
		return result;
	}

}
