using UnityEngine;
using System.Collections;

public class MasterScript : MonoBehaviour {

	public int floor_size;
	public GameObject player;
	public GameObject[] floor;

	// Use this for initialization
	void Start () {
		//Initialize player
		player = (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player";

		//Generate floors
		GameObject floors = new GameObject();
		floors.name = "Floor 1";
		float spacing = 10f;
		for (int x = 0; x < 5; x++) {
            for (int z = 0; z < 5; z++) {
				GameObject new_floor = (GameObject) Instantiate (floor[0], new Vector3 (spacing * x, 0, spacing * z), floor[0].transform.rotation);
				new_floor.transform.parent = floors.transform;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
