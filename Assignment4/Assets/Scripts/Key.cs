﻿using UnityEngine;
using System.Collections;

public class Key : InteractableObject {

	public int keyLevel = -1;

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
			
			Debug.Log("Adding key level " + keyLevel + " to player inventory");
			player.keys[keyLevel] = this;

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
