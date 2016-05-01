using UnityEngine;
using System.Collections;
using System.Threading;

public class MasterScript : MonoBehaviour {

	public int occlusionRange = 5;
	public bool enableCeilings;
	public bool invincibility;
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



	private int occlusionCheck = 0;
	private int occlusionRate = 30;
	private World w;

	// Use this for initialization
	void Start () {

		w = new World (enableCeilings, scale, floorSize, numFloors, ceiling, floor, wall, door, corner, miscellaneous);

		//Initialize player
		player = (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player";
		player.GetComponent<Player>().invincible = invincibility;

		//Initialize monster
		monster = (GameObject) Instantiate (monster, w.cells[floorSize-1, 0, floorSize-1].pos, Quaternion.identity);
		//monster = (GameObject) Instantiate (monster, w.cells[2, 0, 2].pos, Quaternion.identity);
		monster.name = "Monster";
		monster.GetComponent<Monster>().player = player;
		monster.GetComponent<Monster>().floorSize = floorSize;
		monster.GetComponent<Monster>().w = w;

	}






	// Update is called once per frame
	void Update () {

		if (occlusionCheck++ == occlusionRate)
		{
			occlusionCheck = 0;
			for (int y = 0; y < numFloors; y++){
				for (int x = 0; x < floorSize; x++) {
					for (int z = 0; z < floorSize; z++) {
						Cell tCell = w.cells[x,y,z];
						Transform[] allChildren = tCell.cellObj.GetComponentsInChildren<Transform>();

						if (Vector3.Distance(tCell.pos, player.transform.position) <= occlusionRange)
						{
							foreach (Transform child in allChildren) 
						     {
								child.gameObject.layer = 0; // Default layer, rendered
							 }
						}
						else
						{
							foreach (Transform child in allChildren) 
						     {
								child.gameObject.layer = 8; // Unrendered layer

							 }
						}
					}
				}
			}
			Debug.Log("Updating occlusion culling");
		}


	}
}
