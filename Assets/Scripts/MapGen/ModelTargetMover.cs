﻿using UnityEngine;
using System.Collections;

public class ModelTargetMover : MonoBehaviour {

	public Transform target;
	

	Camera cam;
	
	public void Start () {
		//Cache the Main Camera
		cam = Camera.main;
	}
	
	public void OnGUI () {

	}
	
	// Update is called once per frame
	void Update () {
		

			UpdateTargetPosition ();

		
	}
	
	public void UpdateTargetPosition () {
		
		
		//Fire a ray through the scene at the mouse position and place the target where it hits
		RaycastHit hit;
		if (Physics.Raycast	(cam.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity/*, mask*/) && hit.point != target.position) {
			target.position = hit.point;
			target.position = new Vector3( Mathf.RoundToInt(target.position.x), 0.0f, Mathf.RoundToInt(target.position.z));
		}
	}

}
