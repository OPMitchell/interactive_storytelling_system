using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using InteractiveStorytellingSystem;

public class MovementManager : MonoBehaviour
{
	public enum MovementType
	{
		Idle = 1,
		Walking = 2,
		Turning = 3,
		Fleeing = 4
	}

    [SerializeField] private float speed;
	private NavMeshAgent agent;
	private Animator animator;
    private GameObject player;
	public MovementType movementType { get; private set; }
	private Transform target;


	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
        player = GameObject.Find("FPSController");
		movementType = MovementType.Idle;
		target = null;
	}

	void Update()
	{
		switch(movementType)
		{
			case MovementType.Idle:
				break;
			case MovementType.Walking:
				Movement_WalkToTarget(target);
				break;
			case MovementType.Turning:
				Movement_TurnToTarget(target);
				break;
			case MovementType.Fleeing:
				Movement_FleeFromTarget(target);
				break;
		}
	}

	public bool CheckIfAtLocation(Transform target)
	{
		if(target != null)
			return (Vector3.Distance(transform.position, target.transform.position) < 2.0f);
		return false;
	}

	public void Movement_WalkToTarget(Transform target)
	{
		if(target != null)
		{
			if(Vector3.Distance(transform.position, target.transform.position) > 2.0f)
			{
				agent.isStopped = false;
				animator.SetBool("IsWalking", true);
				agent.speed = 2.0f;
          		agent.destination = target.position; 
			}
			else
			{
			    agent.isStopped = true;
				animator.SetFloat("IdleOffset", Random.Range(0.0f, 0.8f));
				animator.SetBool("IsWalking", false);
				SetMovementType(MovementType.Idle);
			}
		}
	}

	private void Movement_FleeFromTarget(Transform target)
	{
		if(target != null)
		{
			if(Vector3.Distance(transform.position, target.transform.position) <= 11.0f)
			{
				Vector3 moveDirection = transform.position - target.transform.position;
				Vector3 newPos = transform.position + moveDirection;
				agent.isStopped = false;
				animator.SetBool("IsWalking", true);
				agent.speed = 3.0f;
				agent.destination = newPos;
			}
			else
			{
			    agent.isStopped = true;
				animator.SetFloat("IdleOffset", Random.Range(0.0f, 0.8f));
				animator.SetBool("IsWalking", false);
				SetMovementType(MovementType.Idle);
			} 
		}
	}

	public void FleeFromTarget(Transform t)
	{
		target = t;
		SetMovementType(MovementType.Fleeing);
	}

	public void WalkToTarget(Transform t)
	{
		target = t;
		SetMovementType(MovementType.Walking);
	}

	public void Movement_TurnToTarget(Transform target)
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
				SetMovementType(MovementType.Idle);
		}
    }

	public void TurnToTarget(Transform t)
	{
		target = t;
		movementType = MovementType.Turning;
	}

	private void LookAtTarget(Transform target)
	{
		var lookPos = target.position - transform.position;
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation(lookPos);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed);
	}

	private void SetMovementType(MovementType mt)
	{
		movementType = mt;
	}

}
