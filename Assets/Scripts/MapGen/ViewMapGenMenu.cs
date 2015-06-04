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

public class ViewMapGenMenu : MonoBehaviour {
	
	
	public Dictionary<EToolsMenu, string> toolsToolbarDictionary = new  Dictionary<EToolsMenu, string>();
	public Dictionary<EEditMenu, string> editToolbarDictionary = new  Dictionary<EEditMenu, string>();
	
	private int mainToolBarIndex = (int)EMainMenu.kTools;
	private int toolsToolBarIndex = 0;
	private int editToolBarIndex = 0;
	private int viewsToolBarIndex = 0;
	private int gridsToolBarIndex = 0;	
	private ModelMapGen  mapGen = null;
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
	

		toolsToolbarDictionary[EToolsMenu.kPlaceWall] =  "Place Wall"; 
		toolsToolbarDictionary[EToolsMenu.kEditWall] =  "Edit Wall"; 
		toolsToolbarDictionary[EToolsMenu.kPlaceRoom] =  "Place Room"; 

		editToolbarDictionary[EEditMenu.kSelectGrid] =  "Grid"; 
	}
	
	// Update is called once per frame
	void Update () {

		if ( mapGen == null)
		{
			GameObject mapgenGameObj = GameObject.Find ( GameObjectHierarchyRef.kGameObjectNameModelMapGen );
			if (mapgenGameObj != null) 
			{
				mapGen = mapgenGameObj.GetComponent<ModelMapGen>();
			}
		}
	}



	void OnGUI() 
	{
		DrawRectangle (new Rect (0, 0, Screen.width, 100), Color.red);  



			if (mapGen != null) {

				string [] editstrings = new string[toolsToolbarDictionary.Count];
				toolsToolbarDictionary.Values.CopyTo(editstrings, 0);

				int toolbarInt = toolsToolBarIndex;
				toolsToolBarIndex = GUI.Toolbar (new Rect (15, 5, 350, 45), toolsToolBarIndex, editstrings);

				switch(toolsToolBarIndex)
				{
				case (int)EToolsMenu.kPlaceRoom:
					if (mapGen != null) 
					{	
						if (GUI.Button (new Rect (15, 50, 100, 45), "Template A")) {
							mapGen.OnSelectRoomTemplate(0);
						}
						if (GUI.Button (new Rect (120, 50, 100, 45), "Template B")) {
							mapGen.OnSelectRoomTemplate(1);
						}
					}
					break;
				}


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
