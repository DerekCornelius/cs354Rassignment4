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
	public Camera endCam;

	[HideInInspector]
	public bool isLocked;

	[HideInInspector]
	public bool isInteracting = false;

	[HideInInspector]
	public int keyRequired;

	[HideInInspector]
	public Player player;

	[HideInInspector]
	public MasterScript masterScript;

	[HideInInspector]
	public bool isMasterDoor = false;

	private bool gatesDown = false;
	private bool isToggled = false;
	private float DoorOpenAngle = 90f;
	private float smooth = 2.0f;

	private float volModifier = 0.5f;

	virtual public void Interact () {
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
					if (isMasterDoor && !gatesDown) {
						aSrc.volume = 1;
						aSrc.clip = aClips[0];
						aSrc.Play();
					}
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
				bool keyFound = false;


					

				for (int i = 0; i < player.keys.Length; i++)
				{
					Key thisKey = player.keys[i];
					if (thisKey != null && thisKey.keyLevel == keyRequired)
					{
						//Debug.Log("Unlocking: " + keyRequired);
						player.DisplayMessage ("You unlocked the door with the " + thisKey.keyName + " key.");
						keyFound = true;
						isLocked = false;
						aSrc.clip = aClips[4];
						aSrc.Play();
						break;
					}
				}

				if (!keyFound)
				{	
					aSrc.clip = aClips[3];
					aSrc.Play();
					if (!isMasterDoor)
						player.DisplayMessage ("The door is locked.");
					else
						player.DisplayMessage ("This door has a masterfully created lock on it.");
					//Debug.Log("Key required: " + keyRequired);
				}

				isInteracting = false;

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

			if (isDoor && !isLocked && !isMasterDoor || (!isLocked && isMasterDoor && gatesDown))
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

				if (isMasterDoor && !player.gameIsEnded) {
					Debug.Log ("Game end");
					player.camera.enabled = false;
					player.gameIsEnded = true;
					Instantiate (endCam);
					player.DisplayMessage ("You escaped alive.", 24, 1000);
				}

			}
			else if (!isLocked && isMasterDoor && !gatesDown)
			{
				
				Transform[] children = this.transform.parent.parent.parent.GetComponentsInChildren<Transform> ();
				Vector3 target = new Vector3 (0, -1f, 0);

				foreach (Transform child in children) {
					//Debug.Log (child.gameObject.name);
					if (child.gameObject.CompareTag("Gate")) {
						child.localPosition = Vector3.Slerp(child.localPosition, target, Time.deltaTime);
					}

					if (child.localPosition.y <= target.y + 0.1f) {
						Debug.Log ("Gates down");
						gatesDown = true;
						isInteracting = false;
					}
						
				}
			}

		}

	}
}
