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

	public void Remove(string item)
	{
		if(Contains(item))
			items.Remove(item);
		else
			Debug.LogError(transform.name + " tried to remove item: " + item + " from their inventory but their inventory doesn't contain it!");
	}

	public List<string> GetList()
	{
		return items;
	}
}
