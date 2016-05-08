using UnityEngine;
using System.Collections;
using System.Threading;

public class MasterScript : MonoBehaviour {

	public int occlusionRange;
	public bool enableCeilings;
	public bool invincibility;
	public bool spawnMonsters;
	public bool noClipping;
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
	public Material[] extraFloorMats;



	private int occlusionCheck = 0;
	private int occlusionRate = 30;
	private World w;
	private int randMonster;
	private bool resettingWorld = false;
	private bool creatingWorld = false;
	private int waitForReset = 0;
	private int waitForCreation = 0;

	void Awake () {
		CreateWorld ();
	}

	void CreateWorld ()
	{
		creatingWorld = true;
		waitForCreation = 0;

		//Initialize player
		player =  (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player";
		player.GetComponent<Player>().invincible = invincibility;
		player.transform.parent = this.transform;

		if (noClipping)
			player.GetComponent<MeshCollider> ().enabled = false;

		w = new World (enableCeilings, scale, floorSize, numFloors, ceiling, 
			floor, wall, door, corner, miscellaneous, key, player, extraFloorMats, this);

		randMonster = Random.Range(0, 2);

		if (!enableCeilings)
			RenderSettings.fog = false;
	}

	void DestroyWorld ()
	{
		resettingWorld = true;
		waitForReset = 0;
		Transform[] children = GetComponentsInChildren<Transform> ();

		foreach (Transform child in children) 
		{
			if (child != this.transform)
				Destroy (child);
		}
	}


	public void SpawnMinorMonster (int tgtDepth)
	{
		if (spawnMonsters) {
			Debug.Log ("Spawning minor monster");
			int r1 = Random.Range (0, floorSize);
			int r2 = Random.Range (0, floorSize);
			while (w.cells [r1, 0, r2].keyDepth != 2) {
				r1 = Random.Range (0, floorSize);
				r2 = Random.Range (0, floorSize);
			}

			GameObject minorMonster = (GameObject) Instantiate (monster[randMonster+2], w.cells[r1, 0, r2].pos, Quaternion.identity);
			minorMonster.name = "Minor Monster";
			minorMonster.transform.parent = this.transform;
			minorMonster.GetComponent<Monster>().player = player;
			minorMonster.GetComponent<Monster>().floorSize = floorSize;
			minorMonster.GetComponent<Monster>().w = w;
		}
	}

	public void SpawnHunter ()
	{
		if (spawnMonsters) {
			Debug.Log ("Spawning hunter monster");
			//GameObject thisMonster = (GameObject) Instantiate (monster[randMonster], w.cells[floorSize-1, 0, floorSize-1].pos, Quaternion.identity);
			GameObject thisMonster = (GameObject)Instantiate (monster [randMonster], w.cells [0, 0, 0].pos, Quaternion.identity);
			thisMonster.name = "Hunter monster";
			thisMonster.transform.parent = this.transform;
			thisMonster.GetComponent<Monster> ().player = player;
			thisMonster.GetComponent<Monster> ().floorSize = floorSize;
			thisMonster.GetComponent<Monster> ().w = w;
		}
	}





	// Update is called once per frame
	void Update () {
		/*
		if (Input.GetKeyDown (KeyCode.F12) && !resettingWorld) {
			Debug.Log ("Resetting world");
			DestroyWorld ();
		}

		if (resettingWorld && ++waitForReset >= 30) {
			CreateWorld ();
		}

		if (creatingWorld && ++waitForCreation >= 30) {
			creatingWorld = resettingWorld = false;
		}*/


		if (occlusionCheck++ == occlusionRate && !resettingWorld && !creatingWorld)
		{
			occlusionCheck = 0;
			for (int y = 0; y < numFloors; y++) {
				for (int x = 0; x < floorSize; x++) {
					for (int z = 0; z < floorSize; z++) {
						Cell tCell = w.cells [x, y, z];
						Transform[] allChildren = tCell.cellObj.GetComponentsInChildren<Transform> ();

						if (Vector3.Distance (tCell.pos, player.transform.position) <= occlusionRange) {
							foreach (Transform child in allChildren) {
								child.gameObject.layer = 0; // Default layer, rendered
							}
						} else {
							foreach (Transform child in allChildren) {
								child.gameObject.layer = 8; // Unrendered layer

							}
						}
					}
				}
			}
			Debug.Log ("Updating occlusion");

			occlusionCheck = 0;
		}


	}
}
