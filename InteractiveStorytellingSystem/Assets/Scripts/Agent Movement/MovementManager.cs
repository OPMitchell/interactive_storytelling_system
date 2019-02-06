using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;

public class MovementManager : MonoBehaviour
{
	enum movementActions
	{
		Idle,
		Follow,
		WalkToTarget,
		TurnToTarget
	};

    public float speed;
	private Animator animator;
    private GameObject player;
    private bool isWalking;
	private movementActions currentAction;
	private Transform target;
	private bool atTarget;

	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
        player = GameObject.Find("FPSController");
        isWalking = false;
		atTarget = false;
		currentAction = movementActions.Idle;
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(currentAction)
		{
			case movementActions.Follow:
				Movement_Follow();
			break;
			case movementActions.WalkToTarget:
				Movement_WalkToTarget();
			break;
			case movementActions.TurnToTarget:
				Movement_TurnToTarget();
			break;
		}
	}

	public void FollowTarget(Transform t)
	{
		target = t;
		SetCurrentAction(movementActions.Follow);
	}
	void Movement_Follow()
	{
		if(target != null)
		{
			if(Vector3.Distance(transform.position, target.transform.position) > 2.0f)
			{
				isWalking = true;
				animator.SetBool("IsWalking", true);
				LookAtTarget();
				transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
			}
			else
			{
				if(isWalking)
				animator.SetFloat("IdleOffset", Random.Range(0.0f, 0.8f));
				isWalking = false;
				animator.SetBool("IsWalking", false);
				LookAtTarget();
			}
		}
		else
		{
			SetCurrentAction(movementActions.Idle);
		}
	}

	public void WalkToTarget(Transform t)
	{
		atTarget = false;
		target = t;
		SetCurrentAction(movementActions.WalkToTarget);
	}
	public void Movement_WalkToTarget()
	{
		if(target != null)
		{
			if(Vector3.Distance(transform.position, target.transform.position) > 2.0f)
			{
				isWalking = true;
				animator.SetBool("IsWalking", true);
				LookAtTarget();
				transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
			}
			else
			{
				atTarget = true;
				if(isWalking)
				animator.SetFloat("IdleOffset", Random.Range(0.0f, 0.8f));
				isWalking = false;
				animator.SetBool("IsWalking", false);
				LookAtTarget();
				target = null;
			}
		}
		else
		{
			SetCurrentAction(movementActions.Idle);
			FinishedAction();
		}
	}

	private void FinishedAction()
	{
		GetComponent<ActionExecutor>().StopExecuting();
	}

	public void TurnToTarget(Transform t)
	{
		target = t;
		SetCurrentAction(movementActions.TurnToTarget);
	}
	public void Movement_TurnToTarget()
    {
		if(target != null)
		{
			var lookPos = target.position - transform.position;
			lookPos.y = 0;
			var rotation = Quaternion.LookRotation(lookPos);
			if(transform.rotation != rotation)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed);
			}
			else
			{
				target = null;
			}
		}
		else
		{
			SetCurrentAction(movementActions.Idle);
		}
    }
	public void LookAtTarget()
	{
		var lookPos = target.position - transform.position;
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation(lookPos);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed);
	}

	void SetCurrentAction(movementActions action)
	{
		currentAction = action;
	}

}
