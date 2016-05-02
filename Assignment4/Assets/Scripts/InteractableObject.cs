using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour {

	public AudioSource aSrc;
	public AudioClip[] aClips;

	// Door audio clips:
	// 0 : open
	// 1 : open 2
	// 2 : close
	// 3 : locked
	// 4 : unlocked

	public bool isDoor;

	[HideInInspector]
	public bool isLocked;

	[HideInInspector]
	public bool isInteracting = false;

	[HideInInspector]
	public int keyRequired;

	private bool isToggled = false;
	private float DoorOpenAngle = 90f;
	private float smooth = 2.0f;

	private float volModifier = 0.5f;

	public void Interact () {
		if (isInteracting == false)
			isInteracting = true;

		// DOOR INTERACTIONS

		if (isDoor)
		{
			//Debug.Log("I am a door being interacted with");

			if (!isLocked)
			{
				if (!isToggled)
				{
					//int r1 = Random.Range(0, 2);
					//aSrc.clip = aClips[r1];
					//aSrc.Play();
				}
				else
				{
					aSrc.clip = aClips[2];
					aSrc.Play();
				}
			}
			else
			{
				aSrc.clip = aClips[3];
				aSrc.Play();
				isInteracting = false;
				Debug.Log("Key required: " + keyRequired);
			}

		}
	}

	// Use this for initialization
	void Start () {
		aSrc.volume = volModifier;
	}
	
	// Update is called once per frame
	void Update () {

		if (isInteracting)
		{

			// DOOR UPDATE INTERACTIONS	

			if (isDoor && !isLocked)
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
			else
			{
				
			}

		}

	}
}
