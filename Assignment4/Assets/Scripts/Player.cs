using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : MonoBehaviour {

	public GUIStyle style;

	public float interactDistance = 5.0f;
	public bool invincible = false;
	public bool isDead = false;
	public bool gameIsEnded = false;

	public Key[] keys = new Key[10];

	[HideInInspector]
	private string msg = "TEST";



	public Camera camera;
	private GameObject curTarget;
	private GameObject curInterTarget;

	private int msgDisplayTime = 120;

	private int msgCounter = 0;
	private bool isDisplayingMsg;
	private Quaternion deathRot;

	// FadeInOut

	/*Texture2D fadeTexture;
	float fadeSpeed = 0.2f;
	int drawDepth = -1000;

	private float alpha = 1.0f; 
	private float fadeDir = -1f;

	bool fading = true;*/


	void OnGUI(){

		/*if (fading) {
			alpha += fadeDir * fadeSpeed * Time.deltaTime;  
			alpha = Mathf.Clamp01(alpha);   

			GUI.color = new Color(0, 0, 0, alpha);

			GUI.depth = drawDepth;

			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
		}*/


		if (isDisplayingMsg) {
			if (msgCounter++ < msgDisplayTime) {
				GUI.TextArea (new Rect (1, 1, Screen.width, Screen.height), msg, style);
			}
			else {
				msg = "";
				msgCounter = 0;
				isDisplayingMsg = false;
			}
		}
	}

	public void DisplayMessage (string message, int fontSize = 12, int displayTime = 120) {
		isDisplayingMsg = true;
		Debug.Assert (isDisplayingMsg);
		msg = message;
		msgCounter = 0;
		msgDisplayTime = displayTime;
		style.fontSize = fontSize;
	}

	// Use this for initialization
	void Start () {
		camera = GetComponentInChildren<Camera>();
		isDisplayingMsg = false;
		//GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		//keys = new List<Key>(10);
	}
	
	// Update is called once per frame
	void Update () {
		if (isDead)
		{
			camera.transform.rotation = deathRot;
		}
		else
		{
			Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;

			if (Input.GetMouseButtonDown(0) && curInterTarget != null)
	            curInterTarget.GetComponent<InteractableObject>().Interact();
	        
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.gameObject != curTarget || curInterTarget == null)
				{
					curTarget = hit.transform.gameObject;

					if (curInterTarget != null)
						DehighlightObj(curInterTarget);

					//Debug.Log("I'm now looking at " + curTarget.name);
					if (curTarget.GetComponent<InteractableObject>() != null)
					{
						curInterTarget = curTarget;
					}
					else if (curTarget.transform.GetComponentInParent<InteractableObject>() != null)
					{
						curInterTarget = curTarget.transform.parent.gameObject;
					}

				}
			}
			else
				Debug.Log("I'm looking at nothing");

			HighlightObj();
		}
	}

	void DehighlightObj (GameObject obj) {
		curInterTarget = null;

		// Reset color
		ApplyColor (obj, new Color(1,1,1));

	}

	void HighlightObj () {
		if (curInterTarget != null)
		{
			float distance = Vector3.Distance(curInterTarget.transform.position, gameObject.transform.position);
			if (distance > interactDistance || curInterTarget.GetComponent<InteractableObject>().isInteracting)
			{
				DehighlightObj (curInterTarget);
			}
			else
			{
				float lerpTime = 0.5f + Mathf.Abs(Time.time % 1 - 0.5f);
				Color highlight = new Color (Mathf.Lerp(0, 1.75f, lerpTime), 
											 Mathf.Lerp(0, 3.5f, lerpTime), 
											 Mathf.Lerp(0, 1.75f, lerpTime));

				ApplyColor (curInterTarget, highlight);


				//Debug.Log(distance);
			}
			

		}	
	}

	// Applies a color to an obj and all its children
	public void ApplyColor(GameObject obj, Color newColor, bool applyToChildren = true)
	{
			Transform[] allChildren = obj.GetComponentsInChildren<Transform>();
			MeshRenderer msh = obj.GetComponent<MeshRenderer>();
			Material[] mats;

			// Color all children
			if (applyToChildren)
				foreach (Transform child in allChildren) 
			     {
					MeshRenderer cMsh = child.gameObject.GetComponent<MeshRenderer>();

			     	if (cMsh != null)
			     	{
			     		mats = cMsh.materials;
			     		foreach (Material mat in mats)
							mat.color = newColor;
			     	}

				 }

			// Color main obj
			if (msh != null)
			{
				mats = msh.materials;
		     		foreach (Material mat in mats)
						mat.color = newColor;
			}
	}

	public void Kill () {
		if (!invincible && !isDead)
		{
			Debug.Log("You died");
			this.GetComponent<CharacterController>().enabled = false;
			camera.GetComponent<SphereCollider>().enabled = true;
			camera.GetComponent<Rigidbody>().useGravity = true;
			deathRot = camera.transform.rotation;
			camera.transform.parent = null;
			this.enabled = false;
			isDead = true;
			gameIsEnded = true;
		}
	}

}
