using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public GameObject player;
	Animator anim;
	NavMeshAgent nav;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		nav = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null)
			GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
		else
			Debug.Log("Monster error: player is null");

		
		anim.SetFloat("Speed", nav.velocity.sqrMagnitude);
	}
}
