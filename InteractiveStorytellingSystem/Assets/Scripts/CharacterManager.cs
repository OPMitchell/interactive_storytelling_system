using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class CharacterManager : MonoBehaviour
{
	[SerializeField] private List<Character> Characters;

	public void Start()
	{
		foreach(Character c in Characters)
		{
			Debug.Log(c.name);
		}
	}
}
