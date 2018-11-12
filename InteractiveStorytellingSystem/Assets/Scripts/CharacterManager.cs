using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class CharacterManager : MonoBehaviour
{
	[SerializeField] private List<Character> Characters;

	public void Awake()
	{
		float x = 0f;
		foreach(Character c in Characters)
		{
			Vector3 position = new Vector3(x, 0f, 0f);
			c.CreateActionList();
			c.CreatePersonality();
			Instantiate(c, position, Quaternion.identity);
			c.SendEventManagerReference(GetComponent<EventManager>());
			x = x - c.GetComponent<Renderer>().bounds.size.x;
			position = new Vector3(x, 0f, 0f);
		}
	}

	public Character FindCharacterByName(string name)
	{
		foreach(Character c in Characters)
		{
			if(c.name == name)
			{
				return c;
			}
		}
		throw new KeyNotFoundException("Could not find a character with that name!");
	}
}
