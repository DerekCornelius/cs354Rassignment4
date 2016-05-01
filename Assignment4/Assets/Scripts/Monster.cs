using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public World w;
	public GameObject player;
	public AudioSource ambience;
	public int floorSize;

	private float killRange = 2f;
	private float detectRange = 20f;
	private Animator anim;
	private NavMeshAgent nav;
	private int refreshRate = 5;
	private int chaseCounter;
	private bool huntMode;

	private float huntSpeed = 4f;
	private float exploreSpeed = 2f;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		nav = GetComponent<NavMeshAgent>();
		ambience = GameObject.FindGameObjectWithTag("Ambience").GetComponent<AudioSource>();
		nav.speed = exploreSpeed;

	}
	
	// Update is called once per frame
	void Update () {
		float playerDistance = Vector3.Distance(player.transform.position, this.transform.position);

		// Set ambience to be louder the further the enemy is away
		ambience.volume = Mathf.Clamp(playerDistance, 0, 1000) / 1000;
		//ambience.ignoreListenerVolume = false;
		//ambience.volume = 0f;

		if (playerDistance <= detectRange && !huntMode)
		{
			//Ray ray = new Ray(this.transform.position, (player.transform.position - this.transform.position));
			RaycastHit hit;

			//Debug.Log("Monster ray hit target: " + hit.transform.gameObject.name);
			//if (Physics.Raycast(ray, out hit, detectRange) && hit.transform.gameObject == player)
			Physics.Linecast(transform.position, player.transform.position, out hit);
			if (hit.transform.gameObject == player)
			{
				Debug.Log("Monster going into hunt mode");
				huntMode = true;
				nav.speed = huntSpeed;
			}
			//Debug.Log("Monster ray hit target: " + hit.transform.gameObject.name);

		}
		else if (playerDistance > detectRange && huntMode)
		{
			huntMode = false;
			nav.speed = exploreSpeed;
		}


		if (playerDistance <= killRange && huntMode)
		{	
			anim.SetBool("Attack", true);
			player.GetComponent<Player>().Kill();
		}


		if (huntMode)
		{
			if (player != null)
				nav.SetDestination(player.transform.position);
			else
				Debug.Log("Monster error: player is null");
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
