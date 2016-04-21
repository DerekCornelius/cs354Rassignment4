using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

	public Vector3 pos;

	/*
	Wall array index by orientation:
	0 = 0 degrees
	1 = 90 degrees
	2 = 180 degrees
	3 = 270 degrees
	*/

	public GameObject[] walls = new GameObject[4]; 
	public GameObject floor;
	public GameObject ceiling;
	public GameObject cellObj;

	public int getNumWalls ()
	{
		int counter = 0;
		for (int i = 0; i < walls.Length; i++) {
			if (walls[i] != null)
				counter++;
		}
		return counter;
	}


	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
