using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollowPlayer : MonoBehaviour 
{
	private Animator animator;
	private GameObject player;
	private bool isFacing;
	public bool followHead;
	private float timer = 6.0f;

	void Start()
	{
		isFacing = false;
		animator = GetComponent<Animator>();
		player = GameObject.Find("Player");
	}

	private void OnAnimatorIK (int layerIndex) 
    {
		if(followHead)
		{
			if(CheckIfPlayerWithinRange(player.transform.position, 6.0f))
			{
				RotatePart(HumanBodyBones.Head, player.transform.Find("PlayerHead"));
			}
		}
    }

	private bool CheckIfPlayerWithinRange(Vector3 obj, float range)
	{
		return Vector3.Distance(transform.position, obj) < range;
	}

	private void RotatePart(HumanBodyBones h, Transform target)
	{
		Transform part = animator.GetBoneTransform(h);
		if((Vector3.Angle(transform.forward, transform.position - target.position)) > 96.0f)
		{
			Vector3 forward = (target.position - part.position).normalized;
			Vector3 up = Vector3.Cross(forward, transform.right);
			Quaternion rotation = Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(forward, up);
			animator.SetBoneLocalRotation(h, rotation);
		}
	}

	void Update()
	{
		if(!isFacing)
		{
			if(CheckIfPlayerWithinRange(player.transform.position, 6.0f))
			{
				isFacing = true;
				timer -= Time.deltaTime;
				if (timer < 0.0f )
				{
					LookAtPlayer();
					isFacing = false;
					timer = 6.0f;
				}
			}
			else
				isFacing = false;
		}
	}

	void LookAtPlayer()
    {
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3.0f);
    }
}
