using UnityEngine;
using System.Collections;

public enum r_type { THREE_BY_THREE };

public class RoomBuilder : MonoBehaviour {

	private int floorSize;
	private MasterScript m;

	public void buildRoom(r_type buildType, int floor, bool debug = false) {
		switch (buildType) {
		case (r_type.THREE_BY_THREE):
			buildThreeByThreeRoom (floor, debug);
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

	//This method can only be called explicitly - buildRoom cannot specify the size correctly
	public void buildXByYRoom(int x, int floor, int z, bool debug = false) {

		int xM = x - 1; //the M stands for minus 1
		int zM = z - 1;

		Debug.Assert (floorSize > xM);
		Debug.Assert (floorSize > zM);


		int xRandom = Random.Range (0, floorSize - xM);
		int zRandom = Random.Range (0, floorSize - zM);

		int attempts = 0;
		while (attempts < 5) {
			bool clear = true;

			for (int i = xRandom; i < xRandom + x; i++) {
				for (int j = zRandom; j < zRandom + z; j++) {
					if (m.cells [i, floor, j].cellType == c_type.ROOM)
						clear = false;
				}
			}
			if (!clear) {
				attempts++;
				xRandom = Random.Range (0, floorSize - xM);
				zRandom = Random.Range (0, floorSize - zM);
			}
			else
				break;
		}
		if (attempts > 4) {
			Debug.Log ("Exceeded 5 attempts, room creation failed.");
			return;
		} else {
			for (int i = xRandom; i < xRandom + x; i++) {
				for (int j = zRandom; j < zRandom + z; j++) {
					m.cells [i, floor, j].cellType = c_type.ROOM;
					m.removeWalls (i, floor, j, 4, debug);
				}
			}
		}

		for (int i = xRandom; i < xRandom + x; i++) {
			m.createWalls (i, floor, zRandom, 0, debug);
			m.createWalls (i, floor, zRandom + zM, 2, debug);
		}
		for (int j = zRandom; j < zRandom + z; j++) {
			m.createWalls (xRandom, floor, j, 1, debug);
			m.createWalls (xRandom + xM, floor, j, 3, debug);
		}



	}

	private void buildThreeByThreeRoom(int floor, bool debug = false) {

		Debug.Assert (floorSize > 2);

		int xRandom = Random.Range (0, floorSize - 2);
		int zRandom = Random.Range (0, floorSize - 2);



		int attempts = 0;
		while (attempts < 5) {
			bool clear = true;

			for (int i = xRandom; i < xRandom + 3; i++) {
				for (int j = zRandom; j < zRandom + 3; j++) {
					if (m.cells [i, floor, j].cellType == c_type.ROOM)
						clear = false;
				}
			}
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
			for (int i = xRandom; i < xRandom + 3; i++) {
				for (int j = zRandom; j < zRandom + 3; j++) {
					m.cells [i, floor, j].cellType = c_type.ROOM;
				}
			}
		}

		m.createWalls (xRandom, floor, zRandom, 0, debug);
		m.createWalls (xRandom, floor, zRandom, 1, debug);
		m.createWalls (xRandom + 1, floor, zRandom, 0, debug);
		m.createWalls (xRandom + 2, floor, zRandom, 0, debug);
		m.createWalls (xRandom + 2, floor, zRandom, 3, debug);
		m.createWalls (xRandom, floor, zRandom + 1, 1, debug);
		m.createWalls (xRandom, floor, zRandom + 2, 1, debug);
		m.createWalls (xRandom, floor, zRandom + 2, 2, debug);
		m.createWalls (xRandom + 1, floor, zRandom + 2, 2, debug);
		m.createWalls (xRandom + 2, floor, zRandom + 2, 2, debug);
		m.createWalls (xRandom + 2, floor, zRandom + 2, 3, debug);
		m.createWalls (xRandom + 2, floor, zRandom + 1, 3, debug);

		m.removeWalls (xRandom + 1, floor, zRandom + 1, 4, debug);
		m.removeWalls (xRandom + 1, floor, zRandom, 1, debug);
		m.removeWalls (xRandom + 1, floor, zRandom, 3, debug);
		m.removeWalls (xRandom, floor, zRandom + 1, 0, debug);
		m.removeWalls (xRandom, floor, zRandom + 1, 2, debug);
		m.removeWalls (xRandom + 1, floor, zRandom + 2, 1, debug);
		m.removeWalls (xRandom + 1, floor, zRandom + 2, 3, debug);
		m.removeWalls (xRandom + 2, floor, zRandom + 1, 0, debug);
		m.removeWalls (xRandom + 2, floor, zRandom + 1, 2, debug);

	}
}
