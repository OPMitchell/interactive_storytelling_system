using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour 
{
	public MemoryPool memoryPool {get; private set;}

	void Start () 
	{
		memoryPool = new MemoryPool();
		AddMemoryPattern(new MemoryPattern(0, new string[]{"Walk", "Beach", "Sunset", "Jason"}, MemoryType.social, 0.0f, "I walked along the beach at sunset with Jason"));
	}

	public MemoryPattern RetrieveMemoryPattern(string keyword)
	{
		return memoryPool.RetrieveMemoryPattern(keyword);
	}

	public void AddMemoryPattern(MemoryPattern mp)
	{
		memoryPool.AddMemoryPattern(mp);
	}
}
