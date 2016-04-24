using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	private Cell[,,] cells;
	private float spacing;
	private float wallHeight;

	// Use this for initialization
	void Start () {

		Debug.Log ("Creating new Cell array: [" + floorSize + "][" + numFloors + "][" + floorSize + "]\n");
		cells = new Cell[floorSize, numFloors, floorSize];
		
		spacing = 10f * scale;
		wallHeight = 10f;
		for (int y = 0; y < numFloors; y++){

			GameObject floors = new GameObject();
			floors.name = "Floor " + y;

			
			for (int x = 0; x < floorSize; x++) {
	            for (int z = 0; z < floorSize; z++) {

	            	// CELL GENERATION

	            	Cell cell = new Cell (x, y, z);
					cells[x, y, z] = cell;
	            	cell.pos = new Vector3 (x, y, z); 

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

	            }
	        }
		}

		GenerateRandomWalls ();

		//Initialize player
		player = (GameObject) Instantiate (player, new Vector3 (0, 1, 0), Quaternion.identity);
		player.name = "Player"; 


		//Thread t = new Thread (EnclosureCheck);
		//t.Start();
		EnclosureCheck ();

	}


	// Random wall generation
	void GenerateRandomWalls () {

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

							if (directionality == 0) {
								//Debug.Log ("A match was found for d:" + directionality);

								cells[x, y, z].walls[0] = testCell.walls[2] = (GameObject) Instantiate (wall[0], 
														 new Vector3 (spacing * x, yOffset, spacing * z - (spacing / 2)), 
														 Quaternion.Euler(0, 0, 0));
								cells[x, y, z].walls[0].transform.parent = cells[x, y, z].cellObj.transform;
								cells[x, y, z].walls[0].transform.localScale *= scale;
								cells[x, y, z].walls[0].name = "Rand d:" + directionality;
							}

							else if (directionality == 1) {
								//Debug.Log ("A match was found for d:" + directionality);

								cells[x, y, z].walls[1] = testCell.walls[3] = (GameObject) Instantiate (wall[0], 
														 new Vector3 (spacing * x - (spacing / 2), yOffset, spacing * z), 
														 Quaternion.Euler(0, 90, 0));
								cells[x, y, z].walls[1].transform.parent = cells[x, y, z].cellObj.transform;
								cells[x, y, z].walls[1].transform.localScale *= scale;
								cells[x, y, z].walls[1].name = "Rand d:" + directionality;
							}

							else if (directionality == 2) {
								//Debug.Log ("A match was found for d:" + directionality);

								cells[x, y, z].walls[2] = testCell.walls[0] = (GameObject) Instantiate (wall[0], 
														 new Vector3 (spacing * x, yOffset, spacing * z + (spacing / 2)), 
														 Quaternion.Euler(0, 180, 0));
								cells[x, y, z].walls[2].transform.parent = cells[x, y, z].cellObj.transform;
								cells[x, y, z].walls[2].transform.localScale *= scale;
								cells[x, y, z].walls[2].name = "Rand d:" + directionality;
							}

							else if (directionality == 3) {
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
	void EnclosureCheck () {

		for (int y = 0; y < numFloors; y++){			
			for (int x = 0; x < floorSize; x++) {
	            for (int z = 0; z < floorSize; z++) {

	            	
					if (cells[x,y,z].walls[1] && cells[x,y,z].walls[2])
					{
						bool possibleEnclosure = true;
						Cell start = cells[x,y,z];
						//cells[x,y,z].walls[2].GetComponent<MeshRenderer>().material.color = new Color (0, 1, 0);
						int xCoord = x;
						int zCoord = z;
						int curCorners = 1;

						start.floor.GetComponent<MeshRenderer>().material.color = new Color (1, 0, 1);


						while (possibleEnclosure)
						{
							if (xCoord != x || zCoord != z)
								cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (0, 1, 0);

							if (curCorners == 1) {
								if (cells[xCoord, y, zCoord].walls[2] != null && cells[xCoord, y, zCoord].walls[3] == null && xCoord + 1 < floorSize) {
									++xCoord;
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[2] != null && cells[xCoord, y, zCoord].walls[3] != null) {
									++curCorners;
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[2] == null) {
									possibleEnclosure = false;
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
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[3] == null) {
									possibleEnclosure = false;
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
									continue;
								}
								else if (cells[xCoord, y, zCoord].walls[0] == null) {
									possibleEnclosure = false;
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
									if (cells[xCoord, y, zCoord] == cells[x, y, z]) {
										Debug.Log ("Enclosure found at (" + xCoord + ", " + y + ", " + zCoord + ")");
										cells[xCoord, y, zCoord].walls[2].GetComponent<MeshRenderer>().material.color = new Color (0, 1, 0);
										cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (1, 0, 0);
										break;
									}
									else
									{
										possibleEnclosure = false;
										cells[xCoord, y, zCoord].floor.GetComponent<MeshRenderer>().material.color = new Color (0, 0, 1);
										break;
									}
								}
								else if (cells[xCoord, y, zCoord].walls[1] == null) {
									possibleEnclosure = false;
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


	
	// Update is called once per frame
	void Update () {
		
	}
}
