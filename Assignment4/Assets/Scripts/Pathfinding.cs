using UnityEngine;
using System.Collections;

public class Pathfinding : MonoBehaviour {


	public int Pathfind (Cell srcCell, Cell tgtCell) {
		for (int i = 0; i < 4; i++){
			if (srcCell.walls[i] == null)
				Debug.Log ("Cell (" + srcCell.index[0] + ", " + srcCell.index[1] + ", " + srcCell.index[2] + ") Wall :" + i + " is null");
		}
		return 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
