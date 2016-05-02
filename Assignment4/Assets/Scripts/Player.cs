using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : MonoBehaviour {

	public float interactDistance = 5.0f;
	public bool invincible = false;
	public bool isDead = false;
	public List<GameObject> keys;

	private Camera camera;
	private GameObject curTarget;
	private GameObject curInterTarget;


	// Use this for initialization
	void Start () {
		camera = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
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
			camera.transform.parent = null;
			this.enabled = false;
			isDead = true;
		}
	}

}
