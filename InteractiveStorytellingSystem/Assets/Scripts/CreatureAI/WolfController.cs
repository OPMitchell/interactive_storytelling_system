using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using InteractiveStorytellingSystem;

public class WolfController : MonoBehaviour 
{
	Animator animator;
	NavMeshAgent agent;
	GameObject chasing;

	void Start()
	{
		chasing = null;
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
	}

	void Update()
	{
		NotifyCharactersOfChase();
		SelectCharacterToChase();
		ChaseCharacter();
	}

	private void SelectCharacterToChase()
	{
		float min = float.MaxValue;
		foreach(GameObject c in GameManager.GetAllCharacters())
		{
			float current = Distance(c);
			if(current < min)
			{
				min = current;
				chasing = c;
			}
		}
	}

	private void NotifyCharactersOfChase()
	{
		foreach(GameObject c in GameManager.GetAllCharacters())
		{
			if(Distance(c) < 10.0f)
			{
				NotifyCharacterOfChase(c);
			}
		}
	}

	private void ChaseCharacter()
	{
		if(chasing != null)
		{
			if(Distance(chasing) <= 30.0f && Distance(chasing) > 22.0f)
			{
				SetCreeping();
			}
			else if (Distance(chasing) <= 22.0f && Distance(chasing) > 2.0f)
			{
				SetRunning();
			}
			else 
			{
				SetIdle();
			}
		}
	}

	private void SetCreeping()
	{
		SetMovement(false, true, false, false, 1.25f);
	}

	private void SetWalking()
	{
		SetMovement(false, false, true, false, 2.5f);
	}

	private void SetRunning()
	{
		SetMovement(false, false, false, true, 5.0f);
	}

	private void SetMovement(bool idle, bool creep, bool walk, bool run, float speed)
	{
		animator.SetBool("isIdle", idle);
		animator.SetBool("isCreeping", creep);
		animator.SetBool("isWalking", walk);
		animator.SetBool("isRunning", run);
		MoveToTarget(speed);
	}

	private void SetIdle()
	{
		animator.SetBool("isIdle", true);
		animator.SetBool("isCreeping", false);
		animator.SetBool("isWalking", false);
		animator.SetBool("isRunning", false);
		agent.isStopped = true;
	}

	private void MoveToTarget(float speed)
	{
		agent.isStopped = false;
		//Vector3 moveDirection = chasing.transform.position - transform.position;
		//Vector3 newPos = transform.position + moveDirection;
		agent.speed = speed;
		agent.destination = chasing.transform.position; 
	}

	private void NotifyCharacterOfChase(GameObject c)
	{
		if(c.transform.name != "Player" && !c.GetComponent<ActionQueue>().ContainsActionType("FleeFromSender"))
		{
			bool valid = true;
			if(c.GetComponent<ActionExecutor>().currentAction != null)
			{
				if(c.GetComponent<ActionExecutor>().currentAction.Type == "FleeFromSender")
					valid = false;
			}
			if(valid)
			{
				Action wolfChase = new Action("WolfChase", "Chased", transform.name, c.transform.name, "", "", "", "", 0);
				c.GetComponent<ReceivingQueue>().QueueAction(wolfChase);
			}
		}
	}

	private float Distance(GameObject c)
	{
		return Vector3.Distance(transform.position, c.transform.position);
	}

}
