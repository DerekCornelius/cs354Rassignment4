using UnityEngine;
using System.Collections;


public enum c_type { HALLWAY, ROOM };
public enum b_type { NONE, WALL, DOOR };

public class Cell : MonoBehaviour {

	public Vector3 pos;
	//Derek added these
	public c_type cellType;
	public int depth = -1;
	public int keyDepth = -1;

	/*
	Wall array index by orientation:
	0 = 0 degrees
	1 = 90 degrees
	2 = 180 degrees
	3 = 270 degrees

	     +Z
	      ^
		  |

	      2
	   _______
	   |     |
	1  |	 |  3	----> +X
	   |_____|

	      0
	    
	*/
	public int[] index = new int[3]; // This cell's index in the cell [x,y,z] array
	public GameObject[] walls = new GameObject[4];
	public GameObject[] doors = new GameObject[4];
	public GameObject floor;
	public GameObject ceiling;
	public GameObject cellObj;


	// Constructors
	public Cell () {

	}

	public Cell (int x, int y, int z) {
		index[0] = x;
		index[1] = y;
		index[2] = z;

		cellType = c_type.HALLWAY;
	}

	public int getNumWalls ()
	{
		int counter = 0;
		for (int i = 0; i < walls.Length; i++) {
			if (walls[i] != null)
				counter++;
		}
		return counter;
	}

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		else
			return (index[0] == ((Cell)obj).index[0]) &&
				   (index[1] == ((Cell)obj).index[1]) && 
				   (index[2] == ((Cell)obj).index[2]);
	}

	//before making a border GameObject, assert that type = NONE
	public b_type checkBorder(int index) {
		if (walls [index] != null)
			return b_type.WALL;
		else if (doors [index] != null)
			return b_type.DOOR;
		else
			return b_type.NONE;
		
	}


	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
