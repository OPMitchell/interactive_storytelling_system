using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalResourceModel : MonoBehaviour 
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
		Hunger = 0.5f;
		Tiredness = 0.0f;
		StartCoroutine(UpdatePhysicalAttributes());
	}
	
	IEnumerator UpdatePhysicalAttributes()
	{
		while(alive)
		{
			if(Hunger + hungerDecay > 1.0f)
				Hunger = 1.0f;
			else
				Hunger += hungerDecay;
			if(Tiredness + tirednessDecay > 1.0f)
				Tiredness = 1.0f;
			else
				Tiredness += tirednessDecay;
			yield return new WaitForSeconds(5.0f);
		}
	}
}
