using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum EMainMenu {
	kTools = 0,
}

public enum EToolsMenu {
	kPlaceWall = 0,
	kEditWall,
	kPlaceRoom,
}

public enum EEditMenu {
	kSelectGrid = 0,

}

public class MapGenMenu : MonoBehaviour {

	public Dictionary<EMainMenu, string> mainToolbarDictionary = new  Dictionary<EMainMenu, string>();
	public Dictionary<EToolsMenu, string> toolsToolbarDictionary = new  Dictionary<EToolsMenu, string>();
	public Dictionary<EEditMenu, string> editToolbarDictionary = new  Dictionary<EEditMenu, string>();
	
	private int mainToolBarIndex = (int)EMainMenu.kTools;
	private int toolsToolBarIndex = 0;
	private int editToolBarIndex = 0;
	private int viewsToolBarIndex = 0;
	private int gridsToolBarIndex = 0;	
	private MapGen  mapGen = null;
	public Material material = null;


	public EMainMenu GetMainMenuSelection()
	{
		return (EMainMenu)mainToolBarIndex;
	}

	public EToolsMenu GetToolsMenuSelection()
	{
		return (EToolsMenu)toolsToolBarIndex;
	}

	public EEditMenu GetEditMenuSelection()
	{
		return (EEditMenu)editToolBarIndex;
	}

	public int GetViewMenuSelectionIndex()
	{
		return viewsToolBarIndex;
	}



	
	// Use this for initialization
	void Start () {
	
		mainToolbarDictionary[EMainMenu.kTools] =  "Tools"; 

		toolsToolbarDictionary[EToolsMenu.kPlaceWall] =  "Place Wall"; 
		toolsToolbarDictionary[EToolsMenu.kEditWall] =  "Edit Wall"; 
		toolsToolbarDictionary[EToolsMenu.kPlaceRoom] =  "Place Room"; 

		editToolbarDictionary[EEditMenu.kSelectGrid] =  "Grid"; 
	}
	
	// Update is called once per frame
	void Update () {

		if ( mapGen == null)
		{
			GameObject mapgenGameObj = GameObject.Find ("MapGen");
			if (mapgenGameObj != null) 
			{
				mapGen = mapgenGameObj.GetComponent<MapGen>();
			}
		}
	}



	void OnGUI() 
	{
		DrawRectangle (new Rect (0, 0, Screen.width, 100), Color.red);  

		string [] foos = new string[mainToolbarDictionary.Count];
		mainToolbarDictionary.Values.CopyTo(foos, 0);

		mainToolBarIndex = GUI.Toolbar (new Rect (15, 15, 750, 25), mainToolBarIndex, foos);

		switch (mainToolBarIndex) 
		{

		case (int)EMainMenu.kTools:
			if (mapGen != null) {

				string [] editstrings = new string[toolsToolbarDictionary.Count];
				toolsToolbarDictionary.Values.CopyTo(editstrings, 0);

				int toolbarInt = toolsToolBarIndex;
				toolsToolBarIndex = GUI.Toolbar (new Rect (15, 45, 250, 25), toolsToolBarIndex, editstrings);

				switch(toolsToolBarIndex)
				{
				case (int)EToolsMenu.kPlaceWall:
					if (mapGen != null) 
					{
						//mapGen.PlaceWallPoint();
					}
					break;
				case (int)EToolsMenu.kPlaceRoom:
					if (mapGen != null) 
					{	
						if (GUI.Button (new Rect (15, 75, 80, 25), "Template A")) {
							mapGen.OnSelectRoomTemplate(0);
						}
						if (GUI.Button (new Rect (95, 75, 80, 25), "Template B")) {
							mapGen.OnSelectRoomTemplate(1);
						}
					}
					break;
				}


			}
			break;

		}




	}


	void DrawRectangle (Rect position, Color color)
	{    
		// We shouldn't draw until we are told to do so.
		if (Event.current.type != EventType.Repaint)
			return;
		
		// Please assign a material that is using position and color.
		if (material == null) {
			material = Resources.Load("Materials/LineGFX", typeof(Material)) as Material; 
		}
		
		material.SetPass (0);
		
		// Optimization hint: 
		// Consider Graphics.DrawMeshNow
		GL.Color (color);
		GL.Begin (GL.QUADS);
		GL.Vertex3 (position.x, position.y, 0);
		GL.Vertex3 (position.x + position.width, position.y, 0);
		GL.Vertex3 (position.x + position.width, position.y + position.height, 0);
		GL.Vertex3 (position.x, position.y + position.height, 0);
		GL.End ();
	}


}
