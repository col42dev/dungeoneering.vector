﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ControllerMapGen : MonoBehaviour {
		
	private string kButtonNameNoSnap = "Jump";
	private float mouseSensitivity  = 0.2f;
	private Vector3 camerPanLastMousePosition = new Vector3();
	private ViewMapGenMenu  mapGenMenu = null;
	private ModelMapGen  mapGen = null;

	private float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
	private float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.


	Plane floorPlane;

	int lastXpos = -1;
	int lastZpos = -1;

	bool bPlaceingWall = false;
	bool bEditingWall = false;

	// Use this for initialization
	void Start () {
	
		floorPlane = new Plane(Vector3.up, new Vector3(0.0f, 0.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
		if (mapGen == null) {
			GameObject mapgenGameObj = GameObject.Find (GameObjectHierarchyRef.kGameObjectNameModelMapGen);
			if (mapgenGameObj != null) {
				mapGen = mapgenGameObj.GetComponent<ModelMapGen> ();
			}
			return;
		} 

		if (mapGenMenu == null) {
			GameObject mapgenMenuGameObj = GameObject.Find (GameObjectHierarchyRef.kGameObjectNameViewMapGenMenu);
			if (mapgenMenuGameObj != null) {
				mapGenMenu = mapgenMenuGameObj.GetComponent<ViewMapGenMenu> ();
			}
			return;
		} 
		
		
		if (GUI.changed) {
			return;
		}

		if (Input.touchCount < 1) {
			switch (mapGenMenu.GetViewMenuSelectionIndex ()) {
			case 0:
				if (Input.GetMouseButton (1)) {
					Vector3 delta = Input.mousePosition - camerPanLastMousePosition;
					transform.Translate (delta.x * mouseSensitivity, delta.y * mouseSensitivity, 0);
				}
				transform.localEulerAngles = new Vector3 (90, 0, 0);
				break;
			case 1:
				if (Input.GetMouseButton (1)) {
					Vector3 delta = Input.mousePosition - camerPanLastMousePosition;
					transform.Translate (delta.x * mouseSensitivity, delta.y * mouseSensitivity, delta.y * mouseSensitivity);
				}
				transform.localEulerAngles = new Vector3 (45, 0, 0);
				break;
			}
			camerPanLastMousePosition = Input.mousePosition;
		}


	
		// Zoom
		if (Input.touchCount < 1) 
		{
			transform.Translate (0.0f, 0.0f, Input.GetAxis ("Mouse ScrollWheel") * 4);
		}


			
			
		/*
		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			
			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


			transform.Translate(0.0f, 0.0f, deltaMagnitudeDiff * 12);


		}
		*/


		//Pos

		Vector2 screenPosition = Input.mousePosition;

		if (Input.touchCount >= 1) 
		{
			screenPosition = Input.GetTouch(0).position;
		}
	
		Ray ray = Camera.main.ScreenPointToRay (screenPosition);
		float hitDist = 0.0f;

		if ( screenPosition.y < (Screen.height - 100) && floorPlane.Raycast( ray, out hitDist)) 
		{
			bool inputPlaceBegin = Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began);
			bool inputPlaceEnd = Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended);

			Vector3 pointerPosition = ray.GetPoint(hitDist);

			pointerPosition.x = Mathf.RoundToInt(pointerPosition.x);
			pointerPosition.z = Mathf.RoundToInt(pointerPosition.z);

			if ( lastXpos != (int)pointerPosition.x || lastZpos !=(int)pointerPosition.z) 
			{
				lastXpos = (int) pointerPosition.x;
				lastZpos = (int) pointerPosition.z;
			}

			if ( mapGenMenu.GetMainMenuSelection() == EMainMenu.kTools) 
			{
				if ( mapGenMenu.GetToolsMenuSelection() == EToolsMenu.kPlaceWall) 
				{
					if (inputPlaceBegin)
					{
						bPlaceingWall = true;
						mapGen.placePoint( pointerPosition, Input.GetButton(kButtonNameNoSnap));
					}

					if (inputPlaceEnd)
					{
						if ( bPlaceingWall == true)
						{
							bPlaceingWall = false;
							mapGen.placePoint( pointerPosition, Input.GetButton(kButtonNameNoSnap));
							mapGen.placePoint(Vector3.zero, Input.GetButton(kButtonNameNoSnap));
						}
					}

					if ( Input.GetKeyDown(KeyCode.Backspace) && bPlaceingWall == true)
					{
						mapGen.placePoint(Vector3.zero, Input.GetButton(kButtonNameNoSnap));
					}
				}

				if ( mapGenMenu.GetToolsMenuSelection() == EToolsMenu.kPlaceRoom) 
				{
					if (inputPlaceBegin)
					{
						mapGen.placeRoom( pointerPosition, Input.GetButton(kButtonNameNoSnap));
					}
				}

				if ( mapGenMenu.GetToolsMenuSelection() == EToolsMenu.kEditWall) 
				{
					if (inputPlaceBegin)
					{
						bEditingWall = true;
						mapGen.editPoint( pointerPosition);
					}


					if ( Input.GetKeyDown(KeyCode.Backspace))
					{
						bEditingWall = false;
						mapGen.removePoint( pointerPosition );
					}

					if ( inputPlaceEnd)
					{
						if ( bEditingWall == true)
						{
							bEditingWall = false;
							mapGen.editPoint( pointerPosition);
						}
					}

				}
			}
		}
	}





		
}
