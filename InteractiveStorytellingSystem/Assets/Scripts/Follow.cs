using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour 
{
    public float speed;

	private Animator animator;
    private GameObject player;
    private bool isWalking;

	void Start () 
	{
		animator = GetComponent<Animator>();
        player = GameObject.Find("FPSController");
        isWalking = false;
	}
	
	void Update () 
	{
        
        if(Vector3.Distance(transform.position, player.transform.position) > 2.0f)
        {
            isWalking = true;
            animator.SetBool("IsWalking", true);
            LookAtPlayer();
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
        else
        {
            if(isWalking)
                animator.SetFloat("IdleOffset", Random.Range(0.0f, 0.8f));
            isWalking = false;
            animator.SetBool("IsWalking", false);
            LookAtPlayer();
        }
        
	}

    void LookAtPlayer()
    {
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed);
    }
}
