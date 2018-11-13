using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class CharacterManager : MonoBehaviour
{
	[SerializeField] private List<GameObject> CharacterPrefabs;
	public void Awake()
	{
		foreach(GameObject c in CharacterPrefabs)
		{
			GameObject obj = Instantiate(c, Vector3.zero, Quaternion.identity);
			obj.name = obj.GetComponent<Character>().Name;
		}
		Debug.Log("CharacterManager has finished instantiating the characters at " + Time.realtimeSinceStartup);
	}
}
