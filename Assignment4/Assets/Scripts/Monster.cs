using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null)
			GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
		else
			Debug.Log("Monster error: player is null");

		GetComponent<Animator>().SetFloat("Speed", 1f);
	}
}
