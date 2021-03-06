﻿using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public World w;
	public GameObject player;
	public AudioSource ambience;
	public int floorSize;

	public bool isMinorMonster;
	public AudioClip huntSound;
	public AudioClip attackSound;
	public float exploreSpeed = 2f;

	private float killRange = 2f;
	public float detectRange = 20f;
	private Animator anim;
	private NavMeshAgent nav;
	private AudioSource aSrc;
	private int refreshRate = 5;
	private int chaseCounter;
	private bool huntMode;
	private bool attacked = false;

	private int huntCounter = 0;
	private int huntWaitTime = 5;

	private float huntSpeed = 5f;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		nav = GetComponent<NavMeshAgent>();
		aSrc = GetComponent<AudioSource>();
		ambience = GameObject.FindGameObjectWithTag("Ambience").GetComponent<AudioSource>();
		nav.speed = exploreSpeed;

	}

	public void setHuntSpeed(float newSpd)
	{
		huntSpeed = newSpd;
	}

	public void setExploreSpeed(float newSpd)
	{
		exploreSpeed = newSpd;
	}

	public void setDetectionRadius(float newRadius)
	{
		detectRange = newRadius;
	}
	
	// Update is called once per frame
	void Update () {
		float playerDistance = Vector2.Distance( new Vector2(player.transform.position.x, player.transform.position.z), 
												 new Vector2(this.transform.position.x, this.transform.position.z));

		// Set ambience to be louder the further the enemy is away
		if (!isMinorMonster)
			ambience.volume = Mathf.Clamp(playerDistance, 0, 1000) / 1000;

		if (playerDistance <= detectRange && !huntMode)
		{
			RaycastHit hit;
			Physics.Linecast(transform.position, player.transform.position, out hit);
			//Debug.Log("Monster ray hit target: " + hit.transform.gameObject.name);

			if (hit.transform.gameObject == player)
			{
				aSrc.clip = huntSound;
				aSrc.Play();
				Debug.Log("Monster going into hunt mode");
				huntMode = true;
				if (!isMinorMonster)
					nav.speed = huntSpeed;
				nav.SetDestination(this.transform.position);
			}

		}
		else if (playerDistance > detectRange && huntMode)
		{
			Debug.Log("Monster leaving hunt mode");
			huntMode = false;
			nav.speed = exploreSpeed;
			huntCounter = 0;
		}


		if (playerDistance <= killRange && huntMode && !attacked && huntCounter > huntWaitTime)
		{	
			attacked = true;
			anim.SetBool("Attack", true);
			aSrc.clip = attackSound;
			aSrc.Play();
			player.GetComponent<Player>().Kill();
		}


		if (huntMode)
		{
			if (huntCounter++ > huntWaitTime)
			{
				if (player != null)
				nav.SetDestination(player.transform.position);
				else
					Debug.Log("Monster error: player is null");
			}
		}
		else
		{
			float destDistance = Vector3.Distance(nav.destination, this.transform.position);
			if (destDistance <= 2f)
			{
				Debug.Log("Monster destination reached, assigning new destination");
				int r1 = Random.Range(0, floorSize-1);
				int r2 = Random.Range(0, floorSize-1);

				nav.SetDestination(w.cells[r1, 0, r2].pos);
			}
		}





		anim.SetFloat("Speed", nav.velocity.sqrMagnitude);
	}
}
