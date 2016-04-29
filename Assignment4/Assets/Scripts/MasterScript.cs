using UnityEngine;
using System.Collections;
using System.Threading;

public class MasterScript : MonoBehaviour {

	public bool enableCeilings;
	public float scale;
	public int floorSize;
	public int numFloors;
	public GameObject player;
	public GameObject[] ceiling;
	public GameObject[] floor;
	public GameObject[] wall;
	public GameObject[] door;


	/*
	public Cell[,,] cells;
	public float spacing;
	public float wallHeight;
	*/


	// Use this for initialization
	void Start () {

		World w = new World (enableCeilings, scale, floorSize, numFloors, ceiling, floor, wall, door);

		//Initialize player
		player = (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player";

	}





	
	// Update is called once per frame
	void Update () {

	}
}
