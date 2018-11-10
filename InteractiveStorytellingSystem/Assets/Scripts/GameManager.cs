using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class GameManager : MonoBehaviour
{
	void Start () 
	{
		CharacterManager.CreateCharacters();
	}

}
