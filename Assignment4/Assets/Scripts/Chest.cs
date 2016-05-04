using UnityEngine;
using System.Collections;

public class Chest : InteractableObject {

	public GameObject chestTop;

	private bool open = false;
	private bool opening = false;

	public Chest ()
	{

	}

	override public void Interact () {
		if (!open)
		{
		   isInteracting = open = opening = true;
			aSrc.clip = aClips[0];
			aSrc.Play();
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (opening)
		{
			Debug.Log("Opening chest");
			Quaternion target;

			target = Quaternion.Euler (-90f, 0, 0);

			chestTop.transform.localRotation = Quaternion.Slerp(chestTop.transform.localRotation, target, Time.deltaTime * 2.0f);
		}
	}
}
