using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge 
{

	private const float kEdgeY = 0.05f;

	
	public GameObject  gfx = null;
	public GameObject[] collisionObjs = new GameObject[2] { null, null};
	public List<EndPoint> points = new List<EndPoint> ();

	public Edge(GameObject lineGFXPrefab)
	{
		if (gfx != null)
		{
			Object.Destroy(gfx);
		}
		
		if (lineGFXPrefab != null)
		{
			gfx = Object.Instantiate( lineGFXPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		}
		
		CreateCollisionObject();
	}
	
	public Edge( EndPoint p0, EndPoint p1) 
	{ 
		points.Add(p0); 
		points.Add(p1);
		
		if (p0.edges.Contains(this) == false)
		{
			p0.edges.Add(this);
		}
		
		if (p1.edges.Contains(this) == false)
		{
			p1.edges.Add(this);
		}
		
		gfx = new GameObject("edge");
		gfx.SetActive(false);
		
		//Line
		gfx.AddComponent<LineRenderer>();
		
		LineRenderer lr = gfx.GetComponent<LineRenderer>();
		
		lr.SetWidth (0.25f, 0.25f);
		
		Vector3 startPos = points[0].point;
		startPos.y += kEdgeY;
		lr.SetPosition (0, startPos);
		
		Vector3 endPos = points[1].point;
		endPos.y += kEdgeY;
		lr.SetPosition (1, endPos);
		
		
		Material mat = Resources.Load("Materials/LineGFX", typeof(Material)) as Material; 
		Material [] mats = new Material[1];
		mats[0] = mat;
		lr.materials = mats;
		
		// Collision.
		CreateCollisionObject();
		
		
		UpdateCollisionObject(startPos, endPos);
		
		
	}



	public void UpdateCollisionObject( Vector3 startPos, Vector3 endPos) 
	{
		for (int index = 0; index < 2; index ++) 
		{

			collisionObjs[index].transform.position = startPos;

			Vector3[] vertices = new Vector3[]
			{
				startPos 							- startPos - new Vector3(0, 1, 0),
				endPos 								- startPos - new Vector3(0, 1, 0),
				startPos + new Vector3 (0, 3, 0) 	- startPos,
				endPos + new Vector3 (0, 3, 0) 		- startPos
			};
			
			
			
			collisionObjs[index].GetComponent<MeshFilter> ().mesh.name = "wallCollision";
			collisionObjs[index].GetComponent<MeshFilter> ().mesh.vertices = vertices;
			collisionObjs[index].GetComponent<MeshFilter> ().mesh.uv = new [] {
				new Vector2 (0, 0),
				new Vector2 (1, 0),
				new Vector2 (0, 1),
				new Vector2 (1, 1)
			};
			
			switch (index)
			{
				case 0:
					collisionObjs[index].GetComponent<MeshFilter> ().mesh.triangles = new [] {0, 1, 2, 2, 1, 3};
					break;
				case 1:
					collisionObjs[index].GetComponent<MeshFilter> ().mesh.triangles = new [] {0, 2, 1, 1, 2, 3};
					break;
			}
			collisionObjs[index].GetComponent<MeshFilter>().mesh.RecalculateNormals();
			collisionObjs[index].GetComponent<MeshFilter>().mesh.RecalculateBounds();

			collisionObjs[index].GetComponent<MeshCollider>().sharedMesh = collisionObjs[index].GetComponent<MeshFilter>().mesh; 
		}

		AstarPath.active.Scan();
		
	}
	
	private void CreateCollisionObject()
	{
		for ( int index = 0; index < 2; index ++)
		{
			
			if (collisionObjs[index] != null)
			{
				Object.Destroy(collisionObjs[index]);
			}
			
			
			collisionObjs[index] = new GameObject();
		
			collisionObjs[index].layer = 9; //obstacles
			collisionObjs[index].name = "wallCollision";
			
			
			
			collisionObjs[index].AddComponent<MeshFilter>();
			
			collisionObjs[index].AddComponent<MeshRenderer>();
			
			
			Material mat = Resources.Load ("Materials/floortile0", typeof(Material)) as Material; 
			Material [] mats = new Material[1];
			mats [0] = mat;
			collisionObjs[index].GetComponent<Renderer> ().materials = mats;
			
			
			collisionObjs[index].AddComponent<MeshCollider>();
		}
	}
	
	
	

};
