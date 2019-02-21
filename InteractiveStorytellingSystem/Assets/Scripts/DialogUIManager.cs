using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogUIManager
{
	private static MemoryManager targetMemories;

	public static void SetTarget(MemoryManager mem)
	{
		targetMemories = mem;
	}

	public static MemoryManager GetTargetMemoryManager()
	{
		return targetMemories;
	}
}
