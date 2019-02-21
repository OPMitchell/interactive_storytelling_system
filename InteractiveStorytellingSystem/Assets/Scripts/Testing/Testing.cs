using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using InteractiveStorytellingSystem;
using UnityEngine;
using System.Linq;
using System.IO;

public static class Testing
{
	public static void WriteToLog(string characterName, string text)
	{
		string path = Application.dataPath + "/Logs/" + characterName + "_Log";
		string finalText = System.DateTime.Now.ToString() + " - " + text;

		using(StreamWriter writer = File.AppendText(path))
		{
			writer.WriteLine(finalText);       
		}
	}

    public static string GetActionInfo(Action action)
	{
		return ("Action(name = " + action.Name + ", sender = " + action.Sender + ", target = " + action.Target + ", precondition = " + action.Precondition + ", effect = " + action.Effect + ")");
	}

	public static void PrintMessage(string message)
	{
		Debug.Log(message);
	}

	public static string GetActionQueue(Transform agent)
	{
		ActionQueue aq = agent.GetComponent<ActionQueue>();
		string queue = agent.name + "'s Action Queue:";
		foreach (var kvp in aq.GetQueue().GetQueue())
		{
			queue += "\n 	" + GetActionInfo(kvp.Value);
		}
		return queue;
	}
}