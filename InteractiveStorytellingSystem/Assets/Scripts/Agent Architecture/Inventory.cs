using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour 
{
	private List<string> items = new List<string>();

	public bool Contains(string item)
	{
		return (items.Contains(item));
	}

	public void Add(string item)
	{
		items.Add(item);
	}

	public List<string> GetList()
	{
		return items;
	}
}
