using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour 
{
	public MemoryPool memoryPool {get; private set;}

	void Start () 
	{
		memoryPool = new MemoryPool();
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
