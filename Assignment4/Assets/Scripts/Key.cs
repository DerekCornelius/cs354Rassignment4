using UnityEngine;
using System.Collections;

public class Key : InteractableObject {

	public int keyLevel = -1;
	public string keyName = "Test key";
	private bool taken = false;

	public Key ()
	{

	}

	public Key (Player p)
	{
		player = p;
	}

	override public void Interact () {
		if (!taken)
		{
			taken = true;
			
			//Debug.Log("Adding key level " + keyLevel + " to player inventory");
			string pickupMsg = "You pick up a";
			if (keyName.StartsWith("a") || keyName.StartsWith("e") || keyName.StartsWith("i") ||
				keyName.StartsWith("o") || keyName.StartsWith("u"))
				pickupMsg += "n";
			pickupMsg += " " + keyName + " key.";
			player.DisplayMessage (pickupMsg);
			player.keys[keyLevel] = this;

			if (keyLevel == 0)
				masterScript.SpawnMinorMonster (1);

			if (keyLevel == 1)
				masterScript.SpawnMinorMonster (2);

			if (keyLevel == 2)
				masterScript.SpawnHunter();

			// Disable mesh / graphic
			MeshRenderer msh = this.transform.GetComponent<MeshRenderer>();
			if (msh != null)
				msh.enabled = false;

			MeshRenderer[] cMshes = this.transform.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer cMsh  in cMshes)
			{
				if (cMsh != null)
					cMsh.enabled = false;
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
