using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class World : MonoBehaviour {

	public bool enableCeilings;
	public float scale;
	public int floorSize;
	public int numFloors;
	//public GameObject player;
	public GameObject key;
	public GameObject[] ceiling;
	public GameObject[] floor;
	public GameObject[] wall;
	public GameObject[] door;
	public GameObject[] corner;
	public GameObject[] miscellaneous;

	public Cell[,,] cells;
	public float spacing;
	public float wallHeight;

	private RoomBuilder rb;
	private GameObject floors;
	private int maxKeyLevel;

	/*
	 * Public Methods:
	 * 
	 * void breadthFirstSearch(int sX, int sY, int sZ, bool debug = false)
	 * void removeWalls(int tX, int tY, int tz, int wallNum, bool debug = false)
	 * void createWalls(int tX, int tY, int tZ, int wallNum, bool debug = false) 
	 * void createDoors(int tX, int tY, int tZ, int doorNum, bool debug = false)
	 * 
	 * Private Methods:
	 * 
	 * void createFloor()
	 * void GenerateRandomWalls ()
	 * void EnclosureCheck (bool debug = false)
	 */

	//unused default constructor
	public World() {

	}

	//basic constructor: start code goes here
	public World(bool eC, float s, int fS, int nF, GameObject[] c, GameObject[] f, GameObject[] w, GameObject[] d, GameObject[] cn, GameObject[] m, GameObject k) {
		enableCeilings = eC;
		scale = s;
		floorSize = fS;
		numFloors = nF;

		ceiling = c;
		floor = f;
		wall = w;
		door = d;
		corner = cn;
		miscellaneous = m;
		key = k;



		Debug.Log ("Creating new Cell array: [" + floorSize + "][" + numFloors + "][" + floorSize + "]\n");
		cells = new Cell[floorSize, numFloors, floorSize];

		spacing = 10f * scale;
		wallHeight = 10f;

		createFloor ();

		GenerateRandomWalls ();

		rb = new RoomBuilder(floorSize, this);
		//sample rooms
		rb.buildXByZRoom (0, 5, 3);
		rb.buildXByZRoom (0, 1, 5);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildXByZRoom (0, 2, 2);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);
		rb.buildRoom (r_type.THREE_BY_THREE, 0);

		removeWalls (floorSize - 1, 0, floorSize - 1, 4);

		EnclosureCheck (false);

		connectAllCellsTo(0, 0, 0, 0, false);

		placeAllKeysStartingFrom (0, 0, 0, true);

		addElements (); 

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// ***** PUBLIC FUNCTIONS *****

	//labels cells with their distance (depth) from the starting cell, in a breadth-first manner
	public void breadthFirstSearch(int sX, int sY, int sZ, int startingDepth, bool debug = false) { //the s is for Starting

		const int WALL_DEPTH = -1; //legacy code for adding blocking maze at a certain depth

		Queue<Cell> Q = new Queue<Cell>();
		Cell root = cells [sX, sY, sZ];
		root.depth = startingDepth;
		Q.Enqueue(root);

		while (Q.Count != 0) {
			Cell curCell = Q.Dequeue ();

			if (debug) {
				if (curCell.depth <= WALL_DEPTH) {
					//float colorByDepth = (float) 1 - ((float) .05 * curCell.depth);
					//curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (colorByDepth, 0, 0);
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (0, 1, 0);
				} else {
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (1, 0, 0);
				}
			}

			int nX = curCell.index[0];
			int nY = curCell.index[1];
			int nZ = curCell.index[2];

			float yOffset = ((wallHeight / 2) * scale) + (nY * wallHeight * scale); 

			if (curCell.walls [0] == null) {
				Cell next = cells [nX, nY, nZ - 1];
				if (next.depth == -1) {
					next.depth = curCell.depth + 1;
					Q.Enqueue (next);
				}
				if (curCell.depth == WALL_DEPTH && next.depth > curCell.depth) {
					curCell.walls[0] = (GameObject) Instantiate (wall[0], 
						new Vector3 (spacing * nX, yOffset, spacing * nZ - (spacing / 2)), 
						Quaternion.Euler(0, 0, 0));
					curCell.walls[0].transform.parent = curCell.cellObj.transform;
					curCell.walls[0].transform.localScale *= scale;
					curCell.walls[0].GetComponent<MeshRenderer>().material.color = new Color (0, 0, 1);
				}
			}
			if (curCell.walls [1] == null) {
				Cell next = cells [nX - 1, nY, nZ];
				if (next.depth == -1) {
					next.depth = curCell.depth + 1;
					Q.Enqueue (next);
				}
				if (curCell.depth == WALL_DEPTH && next.depth > curCell.depth) {
					curCell.walls[1] = (GameObject) Instantiate (wall[0], 
						new Vector3 (spacing * nX - (spacing / 2), yOffset, spacing * nZ), 
						Quaternion.Euler(0, 90, 0));
					curCell.walls[1].transform.parent = curCell.cellObj.transform;
					curCell.walls[1].transform.localScale *= scale;
					curCell.walls[1].GetComponent<MeshRenderer>().material.color = new Color (0, 0, 1);
				}
			}
			if (curCell.walls [2] == null) {
				Cell next = cells [nX, nY, nZ + 1];
				if (next.depth == -1) {
					next.depth = curCell.depth + 1;
					Q.Enqueue (next);
				}
				if (curCell.depth == WALL_DEPTH && next.depth > curCell.depth) {
					curCell.walls[2] = (GameObject) Instantiate (wall[0], 
						new Vector3 (spacing * nX, yOffset, spacing * nZ + (spacing / 2)), 
						Quaternion.Euler(0, 180, 0));
					curCell.walls[2].transform.parent = curCell.cellObj.transform;
					curCell.walls[2].transform.localScale *= scale;
					curCell.walls[2].GetComponent<MeshRenderer>().material.color = new Color (0, 0, 1);
				}
			}
			if (curCell.walls [3] == null) {
				Cell next = cells [nX + 1, nY, nZ];
				if (next.depth == -1) {
					next.depth = curCell.depth + 1;
					Q.Enqueue (next);
				}
				if (curCell.depth == WALL_DEPTH && next.depth > curCell.depth) {
					curCell.walls[3] = (GameObject) Instantiate (wall[0], 
						new Vector3 (spacing * nX + (spacing / 2), yOffset, spacing * nZ), 
						Quaternion.Euler(0, 270, 0));
					curCell.walls[3].transform.parent = curCell.cellObj.transform;
					curCell.walls[3].transform.localScale *= scale;
					curCell.walls[3].GetComponent<MeshRenderer>().material.color = new Color (0, 0, 1);
				}
			}

		}

	}


	public void removeWalls(int tX, int tY, int tZ, int wallNum, bool debug = false) { //4 means ALL walls

		Debug.Assert (tX < floorSize && tY < numFloors && tZ < floorSize);

		Cell target = cells [tX, tY, tZ];
		if ((wallNum == 0 || wallNum == 4) && tZ != 0 && target.walls[0] != null) {
			if (debug) {
				target.walls [0].GetComponent<MeshRenderer> ().material.color = new Color (0, .5f, .5f);
			} else {
				GameObject.Destroy (target.walls [0]);
				target.walls [0] = cells [tX, tY, tZ - 1].walls [2] = null;
			}
		}
		if ((wallNum == 1 || wallNum == 4) && tX != 0 && target.walls[1] != null) {
			if (debug) {
				target.walls [1].GetComponent<MeshRenderer> ().material.color = new Color (0, .5f, .5f);
			} else {
				GameObject.Destroy (target.walls [1]);
				target.walls [1] = cells [tX - 1, tY, tZ].walls [3] = null;
			}
		}
		if ((wallNum == 2 || wallNum == 4) && tZ < floorSize - 1 && target.walls[2] != null) {
			if (debug) {
				target.walls [2].GetComponent<MeshRenderer> ().material.color = new Color (0, .5f, .5f);
			} else {
				GameObject.Destroy (target.walls [2]);
				target.walls [2] = cells [tX, tY, tZ + 1].walls [0] = null;
			}
		}
		if ((wallNum == 3 || wallNum == 4) && tX < floorSize - 1 && target.walls[3] != null) {
			if (debug) {
				target.walls [3].GetComponent<MeshRenderer> ().material.color = new Color (0, .5f, .5f);
			} else {
				GameObject.Destroy (target.walls [3]);
				target.walls [3] = cells [tX + 1, tY, tZ].walls [1] = null;
			}
		}
	}

	public void createWalls(int tX, int tY, int tZ, int wallNum, bool debug = false) { //4 means ALL walls

		Debug.Assert (tX < floorSize && tY < numFloors && tZ < floorSize);

		Cell target = cells [tX, tY, tZ];
		float yOffset = ((wallHeight / 2) * scale) + (tY * wallHeight * scale); 

		if ((wallNum == 0 || wallNum == 4) && tZ >= 0 && target.walls [0] == null) {
			target.walls [0] = cells [tX, tY, tZ - 1].walls [2] = (GameObject)Instantiate (wall [0], 
				new Vector3 (spacing * tX, yOffset, spacing * tZ - (spacing / 2)), 
				Quaternion.Euler (0, 0, 0));
			target.walls [0].transform.parent = target.cellObj.transform;
			target.walls [0].transform.localScale *= scale;
			target.walls [0].name = "Custom wall: " + tX + ", " + tY + ", " + tZ + ", " + wallNum;

			if (debug) target.walls [0].GetComponent<MeshRenderer> ().material.color = new Color (1, .5f, .5f);
		} //else
		//Debug.Log (wallNum + ", " + tZ + ", " + target.walls [0]);

		if ((wallNum == 1 || wallNum == 4) && tX >= 0 && target.walls [1] == null) {
			target.walls[1] = cells[tX - 1, tY, tZ].walls[3] = (GameObject) Instantiate (wall[0], 
				new Vector3 (spacing * tX - (spacing / 2), yOffset, spacing * tZ), 
				Quaternion.Euler(0, 90, 0));
			target.walls[1].transform.parent = target.cellObj.transform;
			target.walls[1].transform.localScale *= scale;
			target.walls[1].name = "Custom wall: " + tX + ", " + tY + ", " + tZ + ", " + wallNum;

			if (debug) target.walls [1].GetComponent<MeshRenderer> ().material.color = new Color (1, .5f, .5f);
		} //else
		//Debug.Log (wallNum + ", " + tX + ", " + target.walls [1]);

		if ((wallNum == 2 || wallNum == 4) && tZ < floorSize - 1 && target.walls [2] == null) {
			target.walls[2] = cells[tX, tY, tZ + 1].walls[0] = (GameObject) Instantiate (wall[0], 
				new Vector3 (spacing * tX, yOffset, spacing * tZ + (spacing / 2)), 
				Quaternion.Euler(0, 180, 0));
			target.walls[2].transform.parent = target.cellObj.transform;
			target.walls[2].transform.localScale *= scale;
			target.walls[2].name = "Custom wall: " + tX + ", " + tY + ", " + tZ + ", " + wallNum;

			if (debug) target.walls [2].GetComponent<MeshRenderer> ().material.color = new Color (1, .5f, .5f);
		} //else
		//Debug.Log (wallNum + ", " + tZ + ", " + target.walls [2]);

		if ((wallNum == 3 || wallNum == 4) && tX < floorSize - 1 && target.walls [3] == null) {
			target.walls[3] = cells[tX + 1, tY, tZ].walls[1] = (GameObject) Instantiate (wall[0], 
				new Vector3 (spacing * tX + (spacing / 2), yOffset, spacing * tZ), 
				Quaternion.Euler(0, 90, 0));
			target.walls[3].transform.parent = target.cellObj.transform;
			target.walls[3].transform.localScale *= scale;
			target.walls[3].name = "Custom wall: " + tX + ", " + tY + ", " + tZ + ", " + wallNum;

			if (debug) target.walls [3].GetComponent<MeshRenderer> ().material.color = new Color (1, .5f, .5f);
		} //else
		//Debug.Log (wallNum + ", " + tX + ", " + target.walls [3]);
	}

	public void createDoors(int tX, int tY, int tZ, int doorNum, bool debug = false) { //4 means ALL doors

		Debug.Assert (tX < floorSize && tY < numFloors && tZ < floorSize);

		Cell target = cells [tX, tY, tZ];
		float yOffset = ((wallHeight / 2) * scale) + (tY * wallHeight * scale); 

		if ((doorNum == 0 || doorNum == 4) && tZ >= 0 && target.checkBorder(0) == b_type.NONE ) {
			target.doors [0] = cells [tX, tY, tZ - 1].doors [2] = (GameObject)Instantiate (door [0], 
				new Vector3 (spacing * tX, yOffset, spacing * tZ - (spacing / 2)), 
				Quaternion.Euler (0, 0, 0));
			target.doors [0].transform.parent = target.cellObj.transform;
			target.doors [0].transform.localScale *= scale;
			target.doors [0].name = "Custom door: " + tX + ", " + tY + ", " + tZ + ", " + doorNum;

			if (debug) target.doors [0].GetComponent<MeshRenderer> ().material.color = new Color (0, 1f, 1f);
		} //else
		//Debug.Log (wallNum + ", " + tZ + ", " + target.walls [0]);

		if ((doorNum == 1 || doorNum == 4) && tX >= 0 && target.checkBorder(1) == b_type.NONE) {
			target.doors[1] = cells[tX - 1, tY, tZ].doors[3] = (GameObject) Instantiate (door[0], 
				new Vector3 (spacing * tX - (spacing / 2), yOffset, spacing * tZ), 
				Quaternion.Euler(0, 90, 0));
			target.doors[1].transform.parent = target.cellObj.transform;
			target.doors[1].transform.localScale *= scale;
			target.doors[1].name = "Custom door: " + tX + ", " + tY + ", " + tZ + ", " + doorNum;

			if (debug) target.doors [1].GetComponent<MeshRenderer> ().material.color = new Color (0, 1, .5f);
		} //else
		//Debug.Log (wallNum + ", " + tX + ", " + target.walls [1]);

		if ((doorNum == 2 || doorNum == 4) && tZ < floorSize - 1 && target.checkBorder(2) == b_type.NONE) {
			target.doors[2] = cells[tX, tY, tZ + 1].doors[0] = (GameObject) Instantiate (door[0], 
				new Vector3 (spacing * tX, yOffset, spacing * tZ + (spacing / 2)), 
				Quaternion.Euler(0, 180, 0));
			target.doors[2].transform.parent = target.cellObj.transform;
			target.doors[2].transform.localScale *= scale;
			target.doors[2].name = "Custom door: " + tX + ", " + tY + ", " + tZ + ", " + doorNum;

			if (debug) target.doors [2].GetComponent<MeshRenderer> ().material.color = new Color (0, 1, .5f);
		} //else
		//Debug.Log (wallNum + ", " + tZ + ", " + target.walls [2]);

		if ((doorNum == 3 || doorNum == 4) && tX < floorSize - 1 && target.checkBorder(3) == b_type.NONE) {
			target.doors[3] = cells[tX + 1, tY, tZ].doors[1] = (GameObject) Instantiate (door[0], 
				new Vector3 (spacing * tX + (spacing / 2), yOffset, spacing * tZ), 
				Quaternion.Euler(0, 90, 0));
			target.doors[3].transform.parent = target.cellObj.transform;
			target.doors[3].transform.localScale *= scale;
			target.doors[3].name = "Custom door: " + tX + ", " + tY + ", " + tZ + ", " + doorNum;

			if (debug) target.doors [3].GetComponent<MeshRenderer> ().material.color = new Color (0, 1, .5f);
		} //else
		//Debug.Log (wallNum + ", " + tX + ", " + target.walls [3]);
	}


	// ***** PRIVATE FUNCTIONS *****

	private void createFloor() {

		int alternator = 0; // Used for creating elements every odd tile, etc.

		for (int y = 0; y < numFloors; y++){

			floors = new GameObject();
			floors.name = "Floor " + y;


			for (int x = 0; x < floorSize; x++) {
				for (int z = 0; z < floorSize; z++) {

					alternator++;
					
					// CELL GENERATION

					Cell cell = new Cell (x, y, z);
					cells[x, y, z] = cell;
					cell.pos = new Vector3 (spacing * x, wallHeight * scale * y, spacing * z); 

					GameObject cellObj = new GameObject ();
					cellObj.name = "Cell (" + x + ", " + y + ", " + z + ")";
					cell.cellObj = cellObj;
					cellObj.transform.parent = floors.transform;

					// BORDER WALL GENERATION

					float yOffset = ((wallHeight / 2) * scale) + (y * wallHeight * scale); 

					// Create 0 degree walls on borders
					if (z == 0) {
						cell.walls[0] = (GameObject) Instantiate (wall[0], 
							new Vector3 (spacing * x, yOffset, spacing * z - (spacing / 2)), 
							Quaternion.Euler(0, 0, 0));
						cell.walls[0].transform.parent = cellObj.transform;
						cell.walls[0].transform.localScale *= scale;
					}

					// Create 90 degree walls on borders
					if (x == 0) {
						cell.walls[1] = (GameObject) Instantiate (wall[0], 
							new Vector3 (spacing * x - (spacing / 2), yOffset, spacing * z), 
							Quaternion.Euler(0, 90, 0));
						cell.walls[1].transform.parent = cellObj.transform;
						cell.walls[1].transform.localScale *= scale;
					}

					// Create 180 degree walls on borders
					if (z == floorSize - 1) {
						cell.walls[2] = (GameObject) Instantiate (wall[0], 
							new Vector3 (spacing * x, yOffset, spacing * z + (spacing / 2)), 
							Quaternion.Euler(0, 180, 0));
						cell.walls[2].transform.parent = cellObj.transform;
						cell.walls[2].transform.localScale *= scale;
					}

					// Create 270 degree walls on borders
					if (x == floorSize - 1) {
						cell.walls[3] = (GameObject) Instantiate (wall[0], 
							new Vector3 (spacing * x + (spacing / 2), yOffset, spacing * z), 
							Quaternion.Euler(0, 270, 0));
						cell.walls[3].transform.parent = cellObj.transform;
						cell.walls[3].transform.localScale *= scale;
					}


					// FLOOR GENERATION

					// Create floor tile per cell
					cell.floor = (GameObject) Instantiate (floor[0], 
						new Vector3 (spacing * x, wallHeight * scale * y, spacing * z), 
						floor[0].transform.rotation);
					cell.floor.transform.parent = cellObj.transform;
					cell.floor.transform.localScale *= scale;


					cellObj.transform.parent = floors.transform;

					// Ceiling GENERATION

					// Create ceiling tile per cell
					if (enableCeilings) {
						cell.ceiling = (GameObject) Instantiate (ceiling[0], 
							new Vector3 (spacing * x, wallHeight * scale * (y + 1), spacing * z), 
							ceiling[0].transform.rotation);
						cell.ceiling.transform.parent = cellObj.transform;
						cell.ceiling.transform.localScale *= scale;
					}


					// LIGHT GENERATION

					/*float lightOffset = 0.3f;
					if (alternator % 2 == 0)
					{
						if (z == 0) {
							Debug.Log ("Creating z torch");
							GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
								new Vector3 (spacing * x, yOffset, spacing * z - (spacing / 2) + lightOffset), 
								Quaternion.Euler(0, 0, 0));
							torch.transform.parent = cellObj.transform;
							torch.transform.localScale *= scale;
						}

						if (x == 0) {
							GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
								new Vector3 (spacing * x - (spacing / 2) + lightOffset, yOffset, spacing * z), 
								Quaternion.Euler(0, 90, 0));
							torch.transform.parent = cellObj.transform;
							torch.transform.localScale *= scale;
						}

						if (z == floorSize - 1) {
							GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
								new Vector3 (spacing * x, yOffset, spacing * z + (spacing / 2) - lightOffset), 
								Quaternion.Euler(0, 180, 0));
							torch.transform.parent = cellObj.transform;
							torch.transform.localScale *= scale;
						}

						if (x == floorSize - 1) {
							GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
								new Vector3 (spacing * x + (spacing / 2) - lightOffset, yOffset, spacing * z), 
								Quaternion.Euler(0, 270, 0));
							torch.transform.parent = cellObj.transform;
							torch.transform.localScale *= scale;
						}
					}*/

				}
			}
		}
	}


	// Random wall generation
	private void GenerateRandomWalls () {

		for (int y = 0; y < numFloors; y++){			
			for (int x = 0; x < floorSize; x++) {
				for (int z = 0; z < floorSize; z++) {

					if (cells[x, y, z].getNumWalls () < 2){
						int random = Random.Range (0, 4);
						//Debug.Log ("Attempting to insert random wall at (" + x + ", " + y + ", " + z + ") with random: " + random);

						Cell testCell = null;

						int directionality = 0;
						/*
						Random to x/z translations:
						random == 0, then x - 1 (or + 1 for borders)
						random == 1, then z - 1 (or + 1 for borders)
						random == 2, then x + 1 (or - 1 for borders)
						random == 3, then z + 1 (or - 1 for borders)

						x/z to directionality translations:
						z - 1 == 0
						x - 1 == 1
						z + 1 == 2
						x + 1 == 3
						*/



						//Get neighboring cell by random number

						float yOffset = ((wallHeight / 2) * scale) + (y * wallHeight * scale); 


						if (random == 0)
						if (x == 0) {
							testCell = cells[x + 1, y, z];
							directionality = 3;
						}
						else {
							testCell = cells[x - 1, y, z];
							directionality = 1;
						}

						else if (random == 1)
						if (z == 0) {
							testCell = cells[x, y, z + 1];
							directionality = 2;
						}
						else {
							testCell = cells[x, y, z - 1];
							directionality = 0;
						}

						else if (random == 2)
						if (x == floorSize - 1) {
							testCell = cells[x - 1, y, z];
							directionality = 1;
						}
						else {
							testCell = cells[x + 1, y, z];
							directionality = 3;
						}

						else if (random == 3)
						if (z == floorSize - 1) {
							testCell = cells[x, y, z - 1];
							directionality = 0;
						}
						else {
							testCell = cells[x, y, z + 1];
							directionality = 2;
						}

						// Add wall to randomly picked neighbor if they also have less than two walls
						if (testCell.getNumWalls () < 2) {

							if (directionality == 0 && testCell.walls[2] == null) {
								//Debug.Log ("A match was found for d:" + directionality);

								cells[x, y, z].walls[0] = testCell.walls[2] = (GameObject) Instantiate (wall[0], 
									new Vector3 (spacing * x, yOffset, spacing * z - (spacing / 2)), 
									Quaternion.Euler(0, 0, 0));
								cells[x, y, z].walls[0].transform.parent = cells[x, y, z].cellObj.transform;
								cells[x, y, z].walls[0].transform.localScale *= scale;
								cells[x, y, z].walls[0].name = "Rand d:" + directionality;
							}

							else if (directionality == 1 && testCell.walls[3] == null) {
								//Debug.Log ("A match was found for d:" + directionality);

								cells[x, y, z].walls[1] = testCell.walls[3] = (GameObject) Instantiate (wall[0], 
									new Vector3 (spacing * x - (spacing / 2), yOffset, spacing * z), 
									Quaternion.Euler(0, 90, 0));
								cells[x, y, z].walls[1].transform.parent = cells[x, y, z].cellObj.transform;
								cells[x, y, z].walls[1].transform.localScale *= scale;
								cells[x, y, z].walls[1].name = "Rand d:" + directionality;
							}

							else if (directionality == 2 && testCell.walls[0] == null) {
								//Debug.Log ("A match was found for d:" + directionality);

								cells[x, y, z].walls[2] = testCell.walls[0] = (GameObject) Instantiate (wall[0], 
									new Vector3 (spacing * x, yOffset, spacing * z + (spacing / 2)), 
									Quaternion.Euler(0, 180, 0));
								cells[x, y, z].walls[2].transform.parent = cells[x, y, z].cellObj.transform;
								cells[x, y, z].walls[2].transform.localScale *= scale;
								cells[x, y, z].walls[2].name = "Rand d:" + directionality;
							}

							else if (directionality == 3 && testCell.walls[1] == null) {
								//Debug.Log ("A match was found for d:" + directionality);

								cells[x, y, z].walls[3] = testCell.walls[1] = (GameObject) Instantiate (wall[0], 
									new Vector3 (spacing * x + (spacing / 2), yOffset, spacing * z), 
									Quaternion.Euler(0, 90, 0));
								cells[x, y, z].walls[3].transform.parent = cells[x, y, z].cellObj.transform;
								cells[x, y, z].walls[3].transform.localScale *= scale;
								cells[x, y, z].walls[3].name = "Rand d:" + directionality;
							}


						}

					}

				}
			}
		}
	}

	// Checks to make sure there aren't any rectangular enclosed areas
	private void EnclosureCheck (bool debug = false) {

		for (int y = 0; y < numFloors; y++){			
			for (int x = 0; x < floorSize; x++) {
				for (int z = 0; z < floorSize; z++) {


					if (cells[x,y,z].walls[1] != null && cells[x,y,z].walls[2] != null)
					{
						bool possibleEnclosure = true;
						Cell start = cells[x,y,z];
						//cells[x,y,z].walls[2].GetComponent<MeshRenderer>().material.color = new Color (0, 1, 0);
						int xCoord = x;
						int zCoord = z;
						int curCorners = 1;
						Cell[] possibleDoorCells = new Cell [4];

						if (debug)
							start.floor.GetComponent<MeshRenderer>().material.color = new Color (1, 0, 1);

						possibleDoorCells [0] = cells [xCoord, y, zCoord];

						while (possibleEnclosure)
						{
							if ((xCoord != x || zCoord != z) && debug)
								cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (0, 1, 0);

							if (curCorners == 1) {
								if (cells[xCoord, y, zCoord].walls[2] != null && cells[xCoord, y, zCoord].walls[3] == null && xCoord + 1 < floorSize) {
									++xCoord;
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[2] != null && cells[xCoord, y, zCoord].walls[3] != null) {
									++curCorners;
									possibleDoorCells [1] = cells [xCoord, y, zCoord];
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[2] == null) {
									possibleEnclosure = false;
									if (debug)
										cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (1, 1, 0);
									break;
								}
							}

							if (curCorners == 2) {
								if (cells[xCoord, y, zCoord].walls[3] != null && cells[xCoord, y, zCoord].walls[0] == null && zCoord - 1 >= 0) {
									--zCoord;
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[3] != null && cells[xCoord, y, zCoord].walls[0] != null) {
									++curCorners;
									possibleDoorCells [2] = cells [xCoord, y, zCoord];
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[3] == null) {
									possibleEnclosure = false;
									if (debug)
										cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (1, 1, 0);
									break;
								}
							}

							if (curCorners == 3) {
								if (cells[xCoord, y, zCoord].walls[0] != null && cells[xCoord, y, zCoord].walls[1] == null && xCoord - 1 >= 0) {
									--xCoord;
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[0] != null && cells[xCoord, y, zCoord].walls[1] != null) {
									++curCorners;
									possibleDoorCells [3] = cells [xCoord, y, zCoord];
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[0] == null) {
									possibleEnclosure = false;
									if (debug)
										cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (1, 1, 0);
									break;
								}
							}

							if (curCorners == 4) {
								if (cells[xCoord, y, zCoord].walls[1] != null && cells[xCoord, y, zCoord].walls[2] == null && zCoord + 1 < floorSize) {
									++zCoord;
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[1] != null && cells[xCoord, y, zCoord].walls[2] != null) {
									if (cells[xCoord, y, zCoord].Equals(cells[x, y, z])) {
										Debug.Assert(xCoord == x && zCoord == z);
										if (debug) {
											Debug.Log ("Enclosure found at (" + xCoord + ", " + y + ", " + zCoord + ")");
										}
										//cells[xCoord, y, zCoord].walls[2].GetComponent<MeshRenderer>().material.color = new Color (0, 1, 0);

										int removeX;
										int removeZ;
										bool doorPlaced = false;
										int rand;

										rand = Random.Range(0, 4);

										// Random door placement
										while (!doorPlaced) {
											if (rand == 0 && zCoord != 0) {
												removeX = possibleDoorCells [3].index [0];
												removeZ = possibleDoorCells [3].index [2];
												removeWalls (removeX, y, removeZ, 0);
												createDoors (removeX, y, removeZ, 0);
												doorPlaced = true;
											} else if (rand == 1 && xCoord != 0) {
												removeX = possibleDoorCells [0].index [0];
												removeZ = possibleDoorCells [0].index [2];
												removeWalls (removeX, y, removeZ, 1);
												createDoors (removeX, y, removeZ, 1);
												doorPlaced = true;
											} else if (rand == 2 && zCoord != floorSize - 1) {
												removeX = possibleDoorCells [0].index [0];
												removeZ = possibleDoorCells [0].index [2];
												removeWalls (removeX, y, removeZ, 2);
												createDoors (removeX, y, removeZ, 2);
												doorPlaced = true;
											} else if (rand == 3 && xCoord != floorSize - 1) {
												removeX = possibleDoorCells [1].index [0];
												removeZ = possibleDoorCells [1].index [2];
												removeWalls (removeX, y, removeZ, 3);
												createDoors (removeX, y, removeZ, 3);
												doorPlaced = true;
											} else {
												rand = Random.Range (0, 4);
											}
										}



										if (debug)
											cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (1, 0, 0);
										break;
									}
									else
									{
										possibleEnclosure = false;
										if (debug)
											cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (0, 0, 1);
										break;
									}
								}
								else if (cells[xCoord, y, zCoord].walls[1] == null) {
									possibleEnclosure = false;
									if (debug)
										cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (1, 1, 0);
									break;
								}
							}
						}

					}
				}
			}
		}

	}

	private void connectAllCellsTo(int sX, int sY, int sZ, int startingDepth, bool debug = false) { //x, y, z starting point for interconnectivity check
		breadthFirstSearch (sX, sY, sZ, startingDepth, debug);

		List<Cell> l = new List<Cell>();

		for (int k = 0; k < numFloors; k++) {
			for (int i = 0; i < floorSize; i++) {
				for (int j = 0; j < floorSize; j++) {
					if (cells [i, k, j].depth == -1)
						l.Add (cells [i, k, j]);
				}
			}
		}

		if (l.Count == 0)
			return;
		int x = 0, y = 0, z = 0;
		int nextDepth = 0;

		bool success = false;
		while (!success) {
			int r = Random.Range (0, l.Count);

			Cell current = l [r];
			x = current.index [0];
			y = current.index [1];
			z = current.index [2];

			if (debug) {
				Debug.Log ("Removing wall to connect at " + x + ", " + y + ", " + z);
				current.floor.GetComponent<MeshRenderer> ().material.color = new Color (0, 1, 0);
			}

			int random = Random.Range (0, 4);


			for (int i = random; i < random + 4; i++) {
				if (debug)
					Debug.Log ("i: " + i);
				int target = i % 4;
				if (debug)
					Debug.Log ("Trying wall: " + target);
				if (!((target == 0 && z == 0) ||
				   (target == 1 && x == 0) ||
				   (target == 2 && z == floorSize - 1) ||
				   (target == 3 && x == floorSize - 1))) {
					if (debug)
						Debug.Log ("Wall is not border wall.");

					Cell neighbor;
					if (target == 0)
						neighbor = cells [x, y, z - 1];
					else if (target == 1)
						neighbor = cells [x - 1, y, z];
					else if (target == 2)
						neighbor = cells [x, y, z + 1];
					else
						neighbor = cells [x + 1, y, z];

					if ((neighbor.depth != -1) && (current.walls [target] != null)) {
						removeWalls (x, y, z, target);
						createDoors (x, y, z, target);
						if (debug) {
							Debug.Log ("Success.");
							Debug.Log ("Neighbor depth: " + neighbor.depth);
							Debug.Log ("Replacing Wall: " + target);
						}
						success = true;
						nextDepth = neighbor.depth + 1;
						continue;
					} else {
						if (debug)
							Debug.Log ("Failed. Neighbor invalid or wall does not exist.");
					}
				} else {
					if (debug)
						Debug.Log ("Failed. Wall is border wall.");
				}
			}

			if (!success) {
				l.RemoveAt (r);
				if (l.Count == 0) {
					Debug.Log ("Completely failed to connect disconnected Rooms. You should not be seeing this message.");
					return;
				}
			}
		}

		connectAllCellsTo (x, y, z, nextDepth, debug); //this function repeats itself until all cells are connected
	}

	public void addElements () {

		int alternator = 0; // Used for creating elements every odd tile, etc.
		List<List<Cell>> keyList = new List<List<Cell>>(); //used for setting what tiles are valid for key placement
		for (int i = 0; i < maxKeyLevel; i++)
			keyList.Add (new List<Cell> ());

		for (int y = 0; y < numFloors; y++) {			
			for (int x = 0; x < floorSize - 1; x++) {
				for (int z = 0; z < floorSize - 1; z++) {

					alternator++;

					// CORNER GENERATION

					bool[] cornerCase = new bool[4];


					cornerCase [0] = (cells [x, y, z].checkBorder(3) != b_type.NONE) &&
									 (cells [x, y, z].checkBorder(2) != b_type.NONE) &&
									 (cells [x, y, z+1].checkBorder(3) == b_type.NONE) &&
									 (cells [x+1, y, z+1].checkBorder(0) == b_type.NONE);

					cornerCase [1] = (cells [x, y, z].checkBorder(3) == b_type.NONE) &&
									 (cells [x, y, z].checkBorder(2) != b_type.NONE) &&
									 (cells [x, y, z+1].checkBorder(3) != b_type.NONE) &&
									 (cells [x+1, y, z+1].checkBorder(0) == b_type.NONE);

					cornerCase [2] = (cells [x, y, z].checkBorder(3) == b_type.NONE) &&
 									 (cells [x, y, z].checkBorder(2) == b_type.NONE) &&
 									 (cells [x, y, z+1].checkBorder(3) != b_type.NONE) &&
									 (cells [x+1, y, z+1].checkBorder(0) != b_type.NONE);

					cornerCase [3] = (cells [x, y, z].checkBorder(3) != b_type.NONE) &&
									 (cells [x, y, z].checkBorder(2) == b_type.NONE) &&
									 (cells [x, y, z+1].checkBorder(3) == b_type.NONE) &&
									 (cells [x+1, y, z+1].checkBorder(0) != b_type.NONE);

					float yOffset = ((wallHeight / 2) * scale) + (y * wallHeight * scale); 


					if (cornerCase[0] || cornerCase[1] || cornerCase[2] || cornerCase[3])
					{
						
						GameObject newCorner = (GameObject) Instantiate (corner[0], 
							new Vector3 (spacing * x + (spacing / 2), yOffset, spacing * z + (spacing / 2)), 
							Quaternion.Euler(0, 0, 0));
						newCorner.transform.localScale *= scale;
						newCorner.transform.parent = cells[x,y,z].cellObj.transform;

					}

					// RANDOM LIGHT GENERATION
					float lightOffset = 0.3f;

					for (int i = 0; i < 4; i++) {
						bool validCell = true;
						Cell tgtCell = cells [x, y, z];
						int r1 = Random.Range (0, 5);
						int r2 = Random.Range (1, 3);

						for (int j = 0; j < 4; j++) {
							if (tgtCell.checkBorder (j) == b_type.DOOR)
								validCell = false;
						}
						if (validCell) {
							

							if (r1 == 1 && tgtCell.cellType == c_type.ROOM) {
								//float objYOffset = 
								float rX = Random.Range (0, spacing);
								float rZ = Random.Range (0, spacing);

								GameObject obj = (GameObject) Instantiate (miscellaneous[r2], 
									new Vector3 (spacing * x + rX, yOffset, spacing * z + rZ), 
									Quaternion.Euler(0, 0, 0));
								obj.transform.parent = tgtCell.cellObj.transform;
								obj.transform.localScale *= scale;
							}
						}



						if (r1 == 0) 
						{
							

							if (i == 0 && tgtCell.checkBorder(i) == b_type.WALL) {
								GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
									new Vector3 (spacing * x, yOffset, spacing * z - (spacing / 2) + lightOffset), 
									Quaternion.Euler(0, 0, 0));
								torch.transform.parent = tgtCell.cellObj.transform;
								torch.transform.localScale *= scale;
							}

							if (i == 1 && tgtCell.checkBorder(i) == b_type.WALL) {
								GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
									new Vector3 (spacing * x - (spacing / 2) + lightOffset, yOffset, spacing * z), 
									Quaternion.Euler(0, 90, 0));
								torch.transform.parent = tgtCell.cellObj.transform;
								torch.transform.localScale *= scale;
							}

							if (i == 2 && tgtCell.checkBorder(i) == b_type.WALL) {
								GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
									new Vector3 (spacing * x, yOffset, spacing * z + (spacing / 2) - lightOffset), 
									Quaternion.Euler(0, 180, 0));
								torch.transform.parent = tgtCell.cellObj.transform;
								torch.transform.localScale *= scale;
							}

							if (i == 3 && tgtCell.checkBorder(i) == b_type.WALL) {
								GameObject torch = (GameObject) Instantiate (miscellaneous[0], 
									new Vector3 (spacing * x + (spacing / 2) - lightOffset, yOffset, spacing * z), 
									Quaternion.Euler(0, 270, 0));
								torch.transform.parent = tgtCell.cellObj.transform;
								torch.transform.localScale *= scale;
							}

						}

					}

					//RANDOM KEY LIST CREATION

					Cell curCell = cells [x, y, z];
					int currentKeyDepth = curCell.keyDepth;
					List<Cell> listBasedOnKeyDepth = keyList [currentKeyDepth];
					listBasedOnKeyDepth.Add (curCell);

				}
			}
		}

		//RANDOM KEY GENERATION AND PLACEMENT
		Debug.Log(maxKeyLevel);
		for (int i = 0; i < maxKeyLevel; i++) {
			List<Cell> currentKeyLevel = keyList [i];
			int randomTargetKeyCell = Random.Range (0, currentKeyLevel.Count);
			Cell keyCell = currentKeyLevel [randomTargetKeyCell];

			keyCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (1, 1, 0);

		}
	}

	//key placement algorithm
	public void placeAllKeysStartingFrom(int sX, int sY, int sZ, bool debug = false) {

		Queue<int[]> toBeSearched = new Queue<int[]> ();

		int startDepth = 0;
		int keyLevel = 1;

		int[] start = { sX, sY, sZ, startDepth, keyLevel };
		toBeSearched.Enqueue (start);

		//flags all rooms based on key depth
		while (toBeSearched.Count != 0) {
			int[] next = toBeSearched.Dequeue ();
			Queue<int[]> toAdd = breadthFirstKeyPlacement (next [0], next [1], next [2], next [3], next [4], debug, 10);
			while (toAdd.Count != 0) {
				int[] nextI = toAdd.Dequeue ();
				if (keyLevel < nextI [4])
					keyLevel = nextI [4];
				toBeSearched.Enqueue (nextI);
			}
		}
			
		//keys are placed in appropriate depth rooms in addElements() function!!
		maxKeyLevel = keyLevel;
	}

	//key depth determination algorithm
	public Queue<int[]> breadthFirstKeyPlacement(int sX, int sY, int sZ, int startDepth, int keyLevel, bool debug = false, int LOCK_DEPTH = 10) { //the s is for Starting


		Queue<Cell> Q = new Queue<Cell>();
		Queue<int[]> Qi = new Queue<int[]> ();
		Cell root = cells [sX, sY, sZ];
		root.keyDepth = startDepth;
		Q.Enqueue(root);

		while (Q.Count != 0) {
			Cell curCell = Q.Dequeue ();

			if (debug) {
				if (keyLevel == 1)
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (0, 1, 0);
				else if (keyLevel == 2)
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (1, 0, 0);
				else if (keyLevel == 3)
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 1);
				else if (keyLevel == 4)
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (.5f, .27f, .1f);
				else if (keyLevel == 5)
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (.1f, .5f, .27f);
				else if (keyLevel == 6)
					curCell.floor.GetComponent<MeshRenderer> ().material.color = new Color (.27f, .1f, .5f);
				
			}

			int nX = curCell.index[0];
			int nY = curCell.index[1];
			int nZ = curCell.index[2];

			b_type n = curCell.checkBorder (0);
			if (((n == b_type.DOOR) && (curCell.depth < (LOCK_DEPTH * keyLevel))) || n == b_type.NONE) {
				Cell next = cells [nX, nY, nZ - 1];
				if (next.keyDepth == -1) {
					next.keyDepth = curCell.keyDepth;
					Q.Enqueue (next);
				}
			} else if (n == b_type.DOOR) {
				Cell next = cells[nX, nY, nZ - 1];
				if (next.keyDepth == -1) {
					//LOCK DOOR 0
					if (debug)
						curCell.doors[0].GetComponent<MeshRenderer>().material.color = new Color (1, 0, 0);
						//Destroy(curCell.doors[0]);
					int[] nextSearch = {nX, nY, nZ - 1, curCell.keyDepth + 1, keyLevel + 1};
					Qi.Enqueue (nextSearch);
				}
			}

			n = curCell.checkBorder (1);
			if (((n == b_type.DOOR) && (curCell.depth < (LOCK_DEPTH * keyLevel))) || n == b_type.NONE) {
				Cell next = cells [nX - 1, nY, nZ];
				if (next.keyDepth == -1) {
					next.keyDepth = curCell.keyDepth;
					Q.Enqueue (next);
				}
			} else if (n == b_type.DOOR) {
				Cell next = cells[nX - 1, nY, nZ];
				if (next.keyDepth == -1) {
					//LOCK DOOR 1
					if (debug)
						curCell.doors[1].GetComponent<MeshRenderer>().material.color = new Color (1, 0, 0);
						//Destroy(curCell.doors[1]);
						
					int[] nextSearch = {nX - 1, nY, nZ, curCell.keyDepth + 1, keyLevel + 1};
					Qi.Enqueue (nextSearch);
				}
			}

			n = curCell.checkBorder (2);
			if (((n == b_type.DOOR) && (curCell.depth < (LOCK_DEPTH * keyLevel))) || n == b_type.NONE) {
				Cell next = cells [nX, nY, nZ + 1];
				if (next.keyDepth == -1) {
					next.keyDepth = curCell.keyDepth;
					Q.Enqueue (next);
				}
			} else if (n == b_type.DOOR) {
				Cell next = cells[nX, nY, nZ + 1];
				if (next.keyDepth == -1) {
					//LOCK DOOR 2
					if (debug)
						curCell.doors[2].GetComponent<MeshRenderer>().material.color = new Color (1, 0, 0);
						//Destroy(curCell.doors[2]);

					int[] nextSearch = {nX, nY, nZ + 1, curCell.keyDepth + 1, keyLevel + 1};
					Qi.Enqueue (nextSearch);
				}
			}

			n = curCell.checkBorder (3);
			if (((n == b_type.DOOR) && (curCell.depth < (LOCK_DEPTH * keyLevel))) || n == b_type.NONE) {
				Cell next = cells [nX + 1, nY, nZ];
				if (next.keyDepth == -1) {
					next.keyDepth = curCell.keyDepth;
					Q.Enqueue (next);
				}
			} else if (n == b_type.DOOR) {
				Cell next = cells[nX + 1, nY, nZ];
				if (next.keyDepth == -1) {
					//LOCK DOOR 3
					if (debug)
						curCell.doors[3].GetComponent<MeshRenderer>().material.color = new Color (1, 0, 0);
						//Destroy(curCell.doors[0]);
						
					int[] nextSearch = {nX + 1, nY, nZ, curCell.keyDepth + 1, keyLevel + 1};
					Qi.Enqueue (nextSearch);
				}
			}
		}

		return Qi;

	}

} //end of class

