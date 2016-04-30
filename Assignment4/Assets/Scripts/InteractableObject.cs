using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour {


	public bool isDoor;

	[HideInInspector]
	public bool isInteracting = false;

	private bool isToggled = false;
	private float DoorOpenAngle = 90f;
	private float smooth = 2.0f;

	public void Interact () {
		if (isInteracting == false)
			isInteracting = true;

		if (isDoor)
		{
			Debug.Log("I am a door being interacted with");
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (isInteracting)
		{
			if (isDoor)
			{
				Quaternion target;

				if (!isToggled)
					target = Quaternion.Euler (0, DoorOpenAngle, 0);
				else
					target = Quaternion.Euler (0, 0, 0);

				transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, target, Time.deltaTime * smooth);
				if (transform.parent.localRotation == target)
				{
					isInteracting = false;
					isToggled = !isToggled;	
				}
			}
		}

	}
}
