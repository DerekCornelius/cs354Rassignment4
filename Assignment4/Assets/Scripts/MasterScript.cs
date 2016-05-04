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
	public GameObject key;
	public GameObject[] monster;
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

		//Initialize player
		player = (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player";
		player.GetComponent<Player>().invincible = invincibility;

		w = new World (enableCeilings, scale, floorSize, numFloors, ceiling, 
			floor, wall, door, corner, miscellaneous, key, player);



		int randMonster = Random.Range(0, 2);
		//Initialize monster
		GameObject thisMonster = (GameObject) Instantiate (monster[randMonster], w.cells[floorSize-1, 0, floorSize-1].pos, Quaternion.identity);
		//GameObject thisMonster = (GameObject) Instantiate (monster[randMonster], w.cells[2, 0, 2].pos, Quaternion.identity);
		thisMonster.name = "Monster";
		thisMonster.GetComponent<Monster>().player = player;
		thisMonster.GetComponent<Monster>().floorSize = floorSize;
		thisMonster.GetComponent<Monster>().w = w;

		int r1 = Random.Range (0, floorSize);
		int r2 = Random.Range (0, floorSize);
		while (w.cells [r1, 0, r2].keyDepth != 1) {
			r1 = Random.Range (0, floorSize);
			r2 = Random.Range (0, floorSize);
		}

		GameObject minorMonster = (GameObject) Instantiate (monster[3], w.cells[r1, 0, r2].pos, Quaternion.identity);
		minorMonster.name = "Minor Monster";
		minorMonster.GetComponent<Monster>().player = player;
		minorMonster.GetComponent<Monster>().floorSize = floorSize;
		minorMonster.GetComponent<Monster>().w = w;
		minorMonster.GetComponent<Monster> ().setExploreSpeed (1);


		r1 = Random.Range (0, floorSize);
		r2 = Random.Range (0, floorSize);
		while (w.cells [r1, 0, r2].keyDepth != 2) {
			r1 = Random.Range (0, floorSize);
			r2 = Random.Range (0, floorSize);
		}

		GameObject minorMonster2 = (GameObject) Instantiate (monster[3], w.cells[r1, 0, r2].pos, Quaternion.identity);
		minorMonster2.name = "Minor Monster";
		minorMonster2.GetComponent<Monster>().player = player;
		minorMonster2.GetComponent<Monster>().floorSize = floorSize;
		minorMonster2.GetComponent<Monster>().w = w;
		minorMonster2.GetComponent<Monster> ().setExploreSpeed (1);

		if (!enableCeilings)
			RenderSettings.fog = false;

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
