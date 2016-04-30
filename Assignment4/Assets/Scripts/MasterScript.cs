using UnityEngine;
using System.Collections;
using System.Threading;

public class MasterScript : MonoBehaviour {

	public bool enableCeilings;
	public float scale;
	public int floorSize;
	public int numFloors;
	public GameObject player;
	public GameObject monster;
	public GameObject[] ceiling;
	public GameObject[] floor;
	public GameObject[] wall;
	public GameObject[] door;
	public GameObject[] corner;
	public GameObject[] miscellaneous;


	// Use this for initialization
	void Start () {

		World w = new World (enableCeilings, scale, floorSize, numFloors, ceiling, floor, wall, door, corner, miscellaneous);

		//Initialize player
		player = (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player";

		//Initialize monster
		monster = (GameObject) Instantiate (monster, w.cells[floorSize-1, 0, floorSize-1].pos, Quaternion.identity);
		monster.name = "Monster";
		monster.GetComponent<Monster>().player = player;
	}





	
	// Update is called once per frame
	void Update () {

	}
}
