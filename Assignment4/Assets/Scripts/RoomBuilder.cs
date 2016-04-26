using UnityEngine;
using System.Collections;

public enum r_type { THREE_BY_THREE };

public class RoomBuilder : MonoBehaviour {

	private int floorSize;

	public void buildRoom(r_type buildType) {
		switch (buildType) {
		case (r_type.THREE_BY_THREE):
			buildThreeByThreeRoom ();
			break;
		}
	}

	public RoomBuilder(int fs) {
		floorSize = fs;
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	private void buildThreeByThreeRoom() {
		int random = Random.Range (0, floorSize);
	}
}
