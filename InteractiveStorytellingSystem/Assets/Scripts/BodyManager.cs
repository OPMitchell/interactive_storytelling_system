using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour 
{
	private bool alive {get;set;}
	public float Hunger {get;set;}
	public float Tiredness {get;set;}
	[SerializeField] private float hungerDecay;
	[SerializeField] private float tirednessDecay;

	// Use this for initialization
	void Start () 
	{
		alive = true;
		Hunger = 0.0f;
		Tiredness = 0.0f;
		StartCoroutine(UpdatePhysicalAttributes());
	}
	
	IEnumerator UpdatePhysicalAttributes()
	{
		while(alive)
		{
			Hunger += hungerDecay;
			Tiredness += tirednessDecay;
			yield return new WaitForSeconds(5.0f);
		}
	}
}
