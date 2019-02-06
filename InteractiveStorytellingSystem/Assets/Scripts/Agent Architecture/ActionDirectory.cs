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

	public List<Action> GetActionsByEffect(string effect)
	{
		string[] split = GameManager.SplitEffectString(effect);
		if(split[1] == "lt")
		{
			return GetActionByParameterAndOperation(split[0], "-");
		}
		else if(split[1] == "gt")
		{
			return GetActionByParameterAndOperation(split[0], "+");
		}
		else
		{
			Debug.Log("Error! Tried to parse an action effect which doesn't contain a '<' or '>'!");
			return null;
		}
	}

	public List<Action> GetActionByParameterAndOperation(string parameter, string operation)
	{
		List<Action> result = new List<Action>();
		foreach(Action action in ActionList)
		{
			if(action.Effect != "")
			{
				string[] s = GameManager.SplitEffectString(action.Effect);
				if(s[0] == parameter && s[1] == operation)
					result.Add(action);
			}
		}
		return result;
	}

}
