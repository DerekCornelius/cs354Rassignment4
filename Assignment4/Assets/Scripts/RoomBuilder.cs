using UnityEngine;
using System.Collections;

public enum r_type { THREE_BY_THREE };

public class RoomBuilder : MonoBehaviour {

	private int floorSize;
	private MasterScript m;

	public void buildRoom(r_type buildType, int floor) {
		switch (buildType) {
		case (r_type.THREE_BY_THREE):
			buildThreeByThreeRoom (floor);
			break;
		}
	}

	public RoomBuilder() {

	}

	public RoomBuilder(int fs, MasterScript ms) {
		floorSize = fs;
		m = ms;
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	private void buildThreeByThreeRoom(int floor) {

		Debug.Assert (floorSize > 2);

		int xRandom = Random.Range (0, floorSize - 2);
		int zRandom = Random.Range (0, floorSize - 2);

		/*
		int attempts = 0;
		int i = xRandom;
		int j = zRandom;
		while (attempts < 5 && ( (i < (3 + xRandom)) && (j < (3 + zRandom)) ) ) {
			if (m.cells [i, floor, j].cellType == c_type.ROOM) {
				xRandom = Random.Range (0, floorSize - 2);
				zRandom = Random.Range (0, floorSize - 2);
				i = xRandom;
				j = zRandom;
				attempts++;
			}
			else if (i < 3 + xRandom)
				i++;
			else {
				i = xRandom;
				j++;
			}
		}
		if (attempts > 4) {
			Debug.Log ("Exceeded 5 attempts, room creation failed.");
			return;
		}
		else {
			for (int ii = xRandom; ii < 3 + xRandom; ii++) {
				for (int jj = zRandom; jj < 3 + zRandom; jj++) {
					m.cells [ii, floor, jj].cellType = c_type.ROOM;
				}
			}
		}
		*/


		int attempts = 0;
		while (attempts < 5) {
			bool clear = true;
			if (m.cells [xRandom, floor, zRandom].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom + 1, floor, zRandom].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom + 2, floor, zRandom].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom, floor, zRandom + 1].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom + 1, floor, zRandom + 1].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom + 2, floor, zRandom + 1].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom, floor, zRandom + 2].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom + 1, floor, zRandom + 2].cellType == c_type.ROOM)
				clear = false;
			if (m.cells [xRandom + 2, floor, zRandom + 2].cellType == c_type.ROOM)
				clear = false;
			if (!clear) {
				attempts++;
				xRandom = Random.Range (0, floorSize - 2);
				zRandom = Random.Range (0, floorSize - 2);
			}
			else
				break;
		}
		if (attempts > 4) {
			Debug.Log ("Exceeded 5 attempts, room creation failed.");
			return;
		} else {
			m.cells [xRandom, floor, zRandom].cellType = c_type.ROOM;
			m.cells [xRandom + 1, floor, zRandom].cellType = c_type.ROOM;
			m.cells [xRandom + 2, floor, zRandom].cellType = c_type.ROOM;
			m.cells [xRandom, floor, zRandom + 1].cellType = c_type.ROOM;
			m.cells [xRandom + 1, floor, zRandom + 1].cellType = c_type.ROOM;
			m.cells [xRandom + 2, floor, zRandom + 1].cellType = c_type.ROOM;
			m.cells [xRandom, floor, zRandom + 2].cellType = c_type.ROOM;
			m.cells [xRandom + 1, floor, zRandom + 2].cellType = c_type.ROOM;
			m.cells [xRandom + 2, floor, zRandom + 2].cellType = c_type.ROOM;
		}


		m.createWalls (xRandom, floor, zRandom, 0, true);
		m.createWalls (xRandom, floor, zRandom, 1, true);
		m.createWalls (xRandom + 1, floor, zRandom, 0, true);
		m.createWalls (xRandom + 2, floor, zRandom, 0, true);
		m.createWalls (xRandom + 2, floor, zRandom, 3, true);
		m.createWalls (xRandom, floor, zRandom + 1, 1, true);
		m.createWalls (xRandom, floor, zRandom + 2, 1, true);
		m.createWalls (xRandom, floor, zRandom + 2, 2, true);
		m.createWalls (xRandom + 1, floor, zRandom + 2, 2, true);
		m.createWalls (xRandom + 2, floor, zRandom + 2, 2, true);
		m.createWalls (xRandom + 2, floor, zRandom + 2, 3, true);
		m.createWalls (xRandom + 2, floor, zRandom + 1, 3, true);

		m.removeWalls (xRandom + 1, floor, zRandom + 1, 4, true);
		m.removeWalls (xRandom + 1, floor, zRandom, 1, true);
		m.removeWalls (xRandom + 1, floor, zRandom, 3, true);
		m.removeWalls (xRandom, floor, zRandom + 1, 0, true);
		m.removeWalls (xRandom, floor, zRandom + 1, 2, true);
		m.removeWalls (xRandom + 1, floor, zRandom + 2, 1, true);
		m.removeWalls (xRandom + 1, floor, zRandom + 2, 3, true);
		m.removeWalls (xRandom + 2, floor, zRandom + 1, 0, true);
		m.removeWalls (xRandom + 2, floor, zRandom + 1, 2, true);


		/*
		m.createWalls (xRandom, floor, zRandom, 0, false);
		m.createWalls (xRandom, floor, zRandom, 1, false);
		m.createWalls (xRandom + 1, floor, zRandom, 0, false);
		m.createWalls (xRandom + 2, floor, zRandom, 0, false);
		m.createWalls (xRandom + 2, floor, zRandom, 3, false);
		m.createWalls (xRandom, floor, zRandom + 1, 1, false);
		m.createWalls (xRandom, floor, zRandom + 2, 1, false);
		m.createWalls (xRandom, floor, zRandom + 2, 2, false);
		m.createWalls (xRandom + 1, floor, zRandom + 2, 2, false);
		m.createWalls (xRandom + 2, floor, zRandom + 2, 2, false);
		m.createWalls (xRandom + 2, floor, zRandom + 2, 3, false);
		m.createWalls (xRandom + 2, floor, zRandom + 1, 3, false);

		m.removeWalls (xRandom + 1, floor, zRandom + 1, 4, false);
		m.removeWalls (xRandom + 1, floor, zRandom, 1, false);
		m.removeWalls (xRandom + 1, floor, zRandom, 3, false);
		m.removeWalls (xRandom, floor, zRandom + 1, 0, false);
		m.removeWalls (xRandom, floor, zRandom + 1, 2, false);
		m.removeWalls (xRandom + 1, floor, zRandom + 2, 1, false);
		m.removeWalls (xRandom + 1, floor, zRandom + 2, 3, false);
		m.removeWalls (xRandom + 2, floor, zRandom + 1, 0, false);
		m.removeWalls (xRandom + 2, floor, zRandom + 1, 2, false);
		*/

	}

	private void buildXByYRoom(int floor, int x, int y) {
		
	}
}
