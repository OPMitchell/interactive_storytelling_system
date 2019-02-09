using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using InteractiveStorytellingSystem;
using UnityEngine;
using System.Linq;

public static class Testing
{
    public static string GetActionInfo(Action action)
	{
		return ("Action(name = " + action.Name + ", sender = " + action.Sender + ", target = " + action.Target + ", precondition = " + action.Precondition + ", sendereffect = " + action.SenderEffect + ", targeteffect = " + action.TargetEffect + ")");
	}

	public static void PrintMessage(string message)
	{
		Debug.Log(message);
	}
}