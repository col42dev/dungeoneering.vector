using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ControllerMapGen : MonoBehaviour {
		
	private string kButtonNameNoSnap = "Jump";
	private float mouseSensitivity  = 0.2f;
	private Vector3 camerPanLastMousePosition = new Vector3();
	private ViewMapGenMenu  mapGenMenu = null;
	private ModelMapGen  mapGen = null;
	
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
	
		if ( mapGen == null)
		{
			GameObject mapgenGameObj = GameObject.Find( GameObjectHierarchyRef.kGameObjectNameModelMapGen);
			if (mapgenGameObj != null) 
			{
				mapGen = mapgenGameObj.GetComponent<ModelMapGen>();
			}
			return;
		} 

		if ( mapGenMenu == null)
		{
			GameObject mapgenMenuGameObj = GameObject.Find( GameObjectHierarchyRef.kGameObjectNameViewMapGenMenu);
			if (mapgenMenuGameObj != null) 
			{
				mapGenMenu = mapgenMenuGameObj.GetComponent<ViewMapGenMenu>();
			}
			return;
		} 
		
		
		if (GUI.changed)
		{
			return;
		}


		switch ( mapGenMenu.GetViewMenuSelectionIndex())
		{
		case 0:
			if (Input.GetMouseButton(1))
			{
				Vector3 delta  = Input.mousePosition - camerPanLastMousePosition;
				transform.Translate(delta.x * mouseSensitivity, delta.y * mouseSensitivity, 0);
			}
			transform.localEulerAngles  = new Vector3(90, 0, 0);
			break;
		case 1:
			if (Input.GetMouseButton(1))
			{
				Vector3 delta  = Input.mousePosition - camerPanLastMousePosition;
				transform.Translate(delta.x * mouseSensitivity, delta.y * mouseSensitivity, delta.y * mouseSensitivity);
			}
			transform.localEulerAngles  = new Vector3(45, 0, 0);
			break;
		}

		camerPanLastMousePosition = Input.mousePosition;


		// Zoom
		transform.Translate(0.0f, 0.0f, Input.GetAxis("Mouse ScrollWheel") * 12);


		Vector2 screenPosition = Input.mousePosition;

		if (Input.touchCount == 1) 
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
					if (Input.GetMouseButtonDown(0))
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
