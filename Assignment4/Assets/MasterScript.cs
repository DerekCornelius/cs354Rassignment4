using UnityEngine;
using System.Collections;

public class MasterScript : MonoBehaviour {

	public int floorSize;
	public GameObject player;
	public GameObject[] floor;
	public GameObject[] wall;

	// Use this for initialization
	void Start () {
		//Initialize player
		player = (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player";

		//Generate floors and walls
		GameObject floors = new GameObject();
		floors.name = "Floor 1";

		GameObject walls = new GameObject();
		walls.name = "Walls 1";
		walls.transform.parent = floors.transform;

		float spacing = 10f;
		for (int x = 0; x < floorSize; x++) {
            for (int z = 0; z < floorSize; z++) {

				if (x == 0) {
					Vector3 rot;
					//rot = wall [0].transform.rotation;
					//rot *= Quaternion.Euler (0, 90, 0);
					GameObject new_wall = (GameObject) Instantiate (wall[0], new Vector3 (spacing * x - (spacing / 2), 0, spacing * z), Quaternion.Euler(0, 90, 0));
				}

				GameObject new_floor = (GameObject) Instantiate (floor[0], new Vector3 (spacing * x, 0, spacing * z), floor[0].transform.rotation);
				new_floor.transform.parent = floors.transform;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
