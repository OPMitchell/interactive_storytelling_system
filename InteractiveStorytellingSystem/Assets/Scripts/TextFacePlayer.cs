using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFacePlayer : MonoBehaviour 
{
	GameObject player;
	// Use this for initialization
	void Start () 
	{
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(Camera.main.transform);
		transform.RotateAround (transform.position, transform.up, 180f);
	}
}
