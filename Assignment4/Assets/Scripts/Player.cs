using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float interactDistance = 5.0f;

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

			}
		}
		else
			Debug.Log("I'm looking at nothing");

		HighlightObj();

	}

	void DehighlightObj (GameObject obj) {
		curInterTarget = null;
		obj.GetComponent<MeshRenderer>().material.color = new Color (1, 1, 1);
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
				curInterTarget.GetComponent<MeshRenderer>().material.color = new Color (Mathf.Lerp(0, 0.75f, lerpTime), 
																				  	 	Mathf.Lerp(0, 1.5f, lerpTime), 
																				   		Mathf.Lerp(0, 0.75f, lerpTime));

				//Debug.Log(distance);
			}
			

		}	
	}

}
