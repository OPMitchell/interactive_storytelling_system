using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWalkSwitch : MonoBehaviour 
{
	Animator animator;

	void Start () 
	{
		animator = GetComponent<Animator>();
		animator.SetBool("IsWalking", false);
	}
	
	void Update () 
	{
		if(Input.GetKeyDown("q"))
		{
			if(animator.GetBool("IsWalking"))
				animator.SetBool("IsWalking", false);
			else
				animator.SetBool("IsWalking", true);
		}
	}
}
