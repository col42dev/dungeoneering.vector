using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class ModelMapGen : MonoBehaviour {
	

	GameObject targetGameObj = null;

	//Prefabs
	public GameObject LineGFXPrefab;
	

	List<EndPoint> endpoints = new List<EndPoint> ();
	List<Edge> edges = new List<Edge> ();
	
	Edge activeEdge; 
	EndPoint activeEndPoint = null;
	VRoom activeRoom = null;

	public class VRoom
	{
		public List<EndPoint> endpoints = new List<EndPoint> ();
		public List<Edge> edges = new List<Edge> ();
	};

	List<VRoom> rooms = new List<VRoom> ();

	private const float kEndPointProximityThreshold = 1.0f;
	private const float kEdgeY = 0.05f;



	// Use this for initialization
	void Start () 
	{
		activeEdge = new Edge ( LineGFXPrefab);
		targetGameObj = GameObject.Find ( GameObjectHierarchyRef.kGameObjectNamePointerTarget);

	}

	// Update is called once per frame
	void Update() 
	{

		// update active edge gfx
		if (activeEdge.points.Count == 1) 
		{
			LineRenderer lr = activeEdge.gfx.GetComponent<LineRenderer>();
			lr.SetWidth (0.25f, 0.25f);
		
			Vector3 startPos = activeEdge.points[0].point;
			startPos.y += kEdgeY;
			lr.SetPosition (0, startPos);
		
			if (targetGameObj != null) 
			{
				Vector3 endPos = targetGameObj.transform.position;
				endPos.y += kEdgeY;
				lr.SetPosition (1, endPos);


				activeEdge.UpdateCollisionObject(startPos, endPos);
			}
		}


		// update active endpoint gfx
		if (activeEndPoint != null) 
		{
			if (targetGameObj != null) 
			{
				activeEndPoint.point = targetGameObj.transform.position;

				foreach (Edge edge in activeEndPoint.edges)
				{
					if ( edge.points.Count >= 2)
					{
						LineRenderer lr = edge.gfx.GetComponent<LineRenderer>();

						Vector3 startPos = edge.points[0].point;
						startPos.y += kEdgeY;
						lr.SetPosition (0, startPos);
						
						Vector3 endPos = edge.points[1].point;
						endPos.y += kEdgeY;
						lr.SetPosition (1, endPos);


						edge.UpdateCollisionObject(startPos, endPos);
					}
				}
			}
		}


		// update active room
		if (activeRoom != null)
		{
			Vector3 targetPos = targetGameObj.transform.position;

			foreach (Edge edge in activeRoom.edges)
			{
				edge.gfx.SetActive(true);
				LineRenderer lr = edge.gfx.GetComponent<LineRenderer>();
				
				Vector3 startPos = edge.points[0].point + targetPos;
				startPos.y += kEdgeY;
				lr.SetPosition (0, startPos);
				
				Vector3 endPos = edge.points[1].point + targetPos;
				endPos.y += kEdgeY;
				lr.SetPosition (1, endPos);
			}

		}

	}


	public void OnSelect_Save(  )
	{
		Debug.Log ("OnSelect_Save");


		//Collision
		List<GameObject> combinedGameObjectList = new List<GameObject>(); 

		for (int layer = 9; layer <= 10; layer ++) 
		{
			GameObject obj = null;

			obj = new GameObject();
			obj.transform.position = new Vector3 (0, 0, 0);

			obj.layer = 8; 

			obj.AddComponent<MeshFilter>();

			//Assign a default material since wavefront .obj format requires one.
			obj.AddComponent<MeshRenderer> ();

			if (layer != 10)
			{
				obj.GetComponent<MeshRenderer> ().enabled = false; 
				Material mat = Resources.Load ("Materials/floortile0", typeof(Material)) as Material; 
				Material [] mats = new Material[1];
				mats [0] = mat;
				obj.GetComponent<Renderer> ().materials = mats;
			}

			Transform[] transforms = gameObject.GetComponentsInChildren<Transform> ();
			List<CombineInstance> combinedInstanceList = new List<CombineInstance> ();

			if ( layer != 8)
			{
				foreach (Edge edge in edges) 
				{
					for (int index = 0; index < 2; index ++)
					{
						MeshFilter mf = edge.collisionObjs [index].GetComponent<MeshFilter> ();
						
						if (mf != null) {

							CombineInstance thisCombine = new CombineInstance ();
							thisCombine.mesh = mf.sharedMesh;
							thisCombine.transform = mf.transform.localToWorldMatrix;
								
							combinedInstanceList.Add (thisCombine);
								
							if ( layer == 10)
							{
								obj.transform.GetComponent<Renderer> ().materials = mf.gameObject.transform.GetComponent<Renderer> ().materials;
							}
						}
					}
				}
			}
			
			
			obj.GetComponent<MeshFilter> ().mesh = new Mesh ();
			obj.GetComponent<MeshFilter> ().mesh.CombineMeshes (combinedInstanceList.ToArray (), true, true);
			obj.GetComponent<MeshFilter> ().mesh.name = "levelMesh";

			
			obj.GetComponent<MeshFilter> ().mesh.RecalculateBounds ();
			obj.GetComponent<MeshFilter> ().mesh.RecalculateNormals ();
			
			
			// Add MeshCollider
			switch (layer) {
			case 8: // ground
			case 9: // obstacle
				obj.AddComponent<MeshCollider> ();
				obj.GetComponent<MeshCollider> ().sharedMesh = obj.GetComponent<MeshFilter> ().mesh;
				obj.GetComponent<MeshCollider> ().sharedMesh.RecalculateBounds ();
				obj.GetComponent<MeshCollider> ().sharedMesh.RecalculateNormals ();
				break;
			}
			
			combinedGameObjectList.Add (obj);
		}

		ObjExporter.DoExport (GameObject.Find ("Plane"), true, "ground");
		ObjExporter.DoExport (combinedGameObjectList [0], true, "obstacles");
		ObjExporter.DoExport (combinedGameObjectList [1], true, "viz");
	
		Object.Destroy (combinedGameObjectList [0]);
		Object.Destroy (combinedGameObjectList [1]);
	}



	private bool MakeEndPoint(Vector3 point, ref EndPoint ep)
	{
		float mindist = Mathf.Infinity;
		EndPoint closestEndpoint = null;
		foreach (EndPoint endpoint in endpoints) 
		{
			float dist = Vector3.Magnitude (point - endpoint.point);
			if (dist < mindist) 
			{
				mindist = dist;
				closestEndpoint = endpoint;
			}
		}
		
		if (mindist < kEndPointProximityThreshold) {
			ep = closestEndpoint;
			return false;
		} else {
			
			ep = new EndPoint ();
			ep.point = point;
			return true;
		}

	}


	private Vector3 EndPointInProximity( Vector3 point)
	{
		float mindist = Mathf.Infinity;
		EndPoint closestEndpoint = null;
		foreach (EndPoint endpoint in endpoints) 
		{
			float dist = Vector3.Magnitude (point - endpoint.point);
			if (dist < mindist) 
			{
				mindist = dist;
				closestEndpoint = endpoint;
			}
		}
		
		if (mindist < kEndPointProximityThreshold) {
			return  closestEndpoint.point;
		} 
		return Vector3.zero;
	}

	private Edge EdgeInProximity( Vector3 point, bool bEdgeSnap)
	{
		if (bEdgeSnap == false) 
		{
		
			return null;
		}

		Edge nearestEdge = null;
		float mindist = Mathf.Infinity;
		foreach (Edge edge in edges) 
		{
			float dist = VectorUtils.DistancePointLine( point, edge.points [0].point, edge.points [1].point);
			if (dist < mindist) 
			{
				mindist = dist;
				nearestEdge = edge;
			}
		}
	
		Edge returnNearestEdge = null;
		if (mindist < kEndPointProximityThreshold ) 
		{
				returnNearestEdge = nearestEdge;
		}

		return returnNearestEdge;
	}

	private Vector3 NearestPointOnEdge( Vector3 point, Edge edge)
	{
		return   VectorUtils.ProjectPointLine( point, edge.points [0].point, edge.points [1].point);
	}

	private EndPoint SplitEdge(Edge edge, Vector3 atPoint)
	{
		Debug.Log ("Split Edges ");

		EndPoint ep = new EndPoint ();
		ep.point = atPoint;

		Edge edgeA = new Edge(LineGFXPrefab);
		{
			edgeA.points.Add (edge.points [0]);
			edge.points [0].edges.Remove (edge);
			edge.points [0].edges.Add (edgeA);
			edgeA.points.Add (ep);
			LineRenderer lr = edgeA.gfx.GetComponent<LineRenderer> ();
			lr.SetWidth (0.25f, 0.25f);
			Vector3 startPos = edgeA.points[0].point;
			startPos.y += kEdgeY;
			lr.SetPosition (0, startPos);
			Vector3 endPos = edgeA.points[1].point;
			endPos.y += kEdgeY;
			lr.SetPosition (1, endPos);
			edgeA.UpdateCollisionObject(startPos, endPos);
		}
		ep.edges.Add (edgeA);

		Edge edgeB = new Edge(LineGFXPrefab);
		{
			edgeB.points.Add (edge.points [1]);
			edge.points [1].edges.Remove (edge);
			edge.points [1].edges.Add (edgeB);
			edgeB.points.Add (ep);
			LineRenderer lr = edgeB.gfx.GetComponent<LineRenderer> ();
			lr.SetWidth (0.25f, 0.25f);
			Vector3 startPos = edgeB.points[0].point;
			startPos.y += kEdgeY;
			lr.SetPosition (0, startPos);
			Vector3 endPos = edgeB.points[1].point;
			endPos.y += kEdgeY;
			lr.SetPosition (1, endPos);
			edgeB.UpdateCollisionObject(startPos, endPos);
		}
		ep.edges.Add (edgeB);


		// Add new edges to global Edges list
		edges.Add (edgeA);
		edges.Add (edgeB);

		// Add new endpoint to global endpoints list.
		endpoints.Add (ep);

		// Remove original edge from global Edges list.
		Object.Destroy (edge.gfx);
		Object.Destroy (edge.collisionObjs[0]);
		Object.Destroy (edge.collisionObjs[1]);
		edges.Remove (edge);

		return ep;
	}



	public void OnSelectRoomTemplate(int index)
	{
		VRoom r = new VRoom ();

		switch (index)
		{
		case 0:
			r.endpoints.Add (new EndPoint ( new Vector3(0, 0,0)));
			r.endpoints.Add (new EndPoint ( new Vector3(0, 0,4)));
			r.endpoints.Add (new EndPoint ( new Vector3(4, 0,4)));
			r.endpoints.Add (new EndPoint ( new Vector3(4, 0,0)));
			
			r.edges.Add (new Edge (r.endpoints[0], r.endpoints[1]));
			r.edges.Add (new Edge (r.endpoints[1], r.endpoints[2]));
			r.edges.Add (new Edge (r.endpoints[2], r.endpoints[3]));
			r.edges.Add (new Edge (r.endpoints[3], r.endpoints[0]));

			break;
		case 1:
			r.endpoints.Add (new EndPoint ( new Vector3(3, 0,0)));
			r.endpoints.Add (new EndPoint ( new Vector3(7, 0,0)));
			r.endpoints.Add (new EndPoint ( new Vector3(7, 0,8)));
			r.endpoints.Add (new EndPoint ( new Vector3(10, 0,8)));
			r.endpoints.Add (new EndPoint ( new Vector3(10, 0,11)));
			r.endpoints.Add (new EndPoint ( new Vector3(7, 0,11)));
			r.endpoints.Add (new EndPoint ( new Vector3(6, 0,14)));
			r.endpoints.Add (new EndPoint ( new Vector3(4, 0,14)));
			r.endpoints.Add (new EndPoint ( new Vector3(3, 0,11)));
			r.endpoints.Add (new EndPoint ( new Vector3(0, 0,11)));
			r.endpoints.Add (new EndPoint ( new Vector3(0, 0,8)));
			r.endpoints.Add (new EndPoint ( new Vector3(3, 0,8)));
			
			
			r.edges.Add (new Edge (r.endpoints[0], r.endpoints[1]));
			r.edges.Add (new Edge (r.endpoints[1], r.endpoints[2]));
			r.edges.Add (new Edge (r.endpoints[2], r.endpoints[3]));
			r.edges.Add (new Edge (r.endpoints[3], r.endpoints[4]));
			r.edges.Add (new Edge (r.endpoints[4], r.endpoints[5]));
			r.edges.Add (new Edge (r.endpoints[5], r.endpoints[6]));
			r.edges.Add (new Edge (r.endpoints[6], r.endpoints[7]));
			r.edges.Add (new Edge (r.endpoints[7], r.endpoints[8]));
			r.edges.Add (new Edge (r.endpoints[8], r.endpoints[9]));
			r.edges.Add (new Edge (r.endpoints[9], r.endpoints[10]));
			r.edges.Add (new Edge (r.endpoints[10], r.endpoints[11]));
			r.edges.Add (new Edge (r.endpoints[11], r.endpoints[0]));

			break;
		}

		activeRoom = r;

	}

	public void placeRoom(Vector3 point, bool bEdgeSnap)
	{
		Debug.Log ("placeRoom" + point);

		// update active room
		if (activeRoom != null) 
		{	
			for (int i = 0; i < activeRoom.endpoints.Count; i ++)
			{
				activeRoom.endpoints[i].point += point;
			}

			foreach (Edge edge in activeRoom.edges)
			{
				Vector3 startPos = edge.points[0].point;
				Vector3 endPos = edge.points[1].point;

				edge.UpdateCollisionObject(startPos, endPos);
			}

			foreach (EndPoint ep in activeRoom.endpoints)
			{
				endpoints.Add(ep);
			}

			foreach (Edge edge in activeRoom.edges)
			{
				edges.Add(edge);
			}
		}

		activeRoom = new VRoom ();
	}

	public void placePoint( Vector3 point, bool bEdgeSnap) 
	{
		if (point != Vector3.zero) 
		{
			if (activeEdge.points.Count == 0)
			{
				Vector3 startPoint = point;

				//Proximity checks
				Vector3 proximityEndPoint = EndPointInProximity( startPoint);
				if (proximityEndPoint != Vector3.zero)
				{
					startPoint = proximityEndPoint;
				}
				else 
				{
					Debug.Log(bEdgeSnap);

					Edge nearestEdge = EdgeInProximity(startPoint, bEdgeSnap);
					if ( nearestEdge != null )
					{
						startPoint =  NearestPointOnEdge(startPoint, nearestEdge); 
					}
				}


				EndPoint ep = new EndPoint ();
				ep.point = startPoint;
				ep.edges.Add(activeEdge);

				activeEdge.points.Add( ep);
			}
			else if (activeEdge.points.Count == 1)
			{
				// Start Point
				EndPoint startEndPoint = activeEdge.points[0];
				if ( MakeEndPoint(startEndPoint.point, ref startEndPoint))
				{
					Debug.Log(bEdgeSnap);

					Edge nearestEdge = EdgeInProximity(startEndPoint.point, bEdgeSnap);
					if (nearestEdge!= null)
					{
						Vector3 edgePoint =  NearestPointOnEdge( startEndPoint.point, nearestEdge);
						startEndPoint = SplitEdge(nearestEdge, edgePoint);
					}
					else
					{
						endpoints.Add(startEndPoint);
					}
				}
				startEndPoint.edges.Add( activeEdge);
				activeEdge.points[0] = startEndPoint;


				// End Point
				EndPoint endEndPoint = null;
				if ( MakeEndPoint(point, ref endEndPoint))
				{
					Debug.Log(bEdgeSnap);

					Edge nearestEdge = EdgeInProximity(point, bEdgeSnap);
					if (nearestEdge!= null)
					{
						Vector3 edgePoint =  NearestPointOnEdge( point, nearestEdge);
						endEndPoint = SplitEdge(nearestEdge, edgePoint);
					}
					else{
						endpoints.Add(endEndPoint);
					}
				}


				endEndPoint.edges.Add(activeEdge);
 

				activeEdge.points.Add( endEndPoint );

				// Update Render
				LineRenderer lr = activeEdge.gfx.GetComponent<LineRenderer>();
				Vector3 endPos = activeEdge.points[1].point;
				endPos.y += kEdgeY;
				lr.SetPosition (1, endPos);

				activeEdge.UpdateCollisionObject( activeEdge.points[0].point, activeEdge.points[1].point);

				// Add new Edge to global edge list
				edges.Add(activeEdge);

				// Update new Edge
				activeEdge = new Edge( LineGFXPrefab );
				activeEdge.points.Add(endEndPoint);

			
			}
		} else {
			activeEdge.points.Clear();
			LineRenderer lr = activeEdge.gfx.GetComponent<LineRenderer>();
			lr.SetPosition (0, Vector3.zero);
			lr.SetPosition (1, Vector3.zero);
		}

	}


	public void editPoint( Vector3 point) 
	{
		if (point != Vector3.zero) 
		{
			if (activeEndPoint == null)
			{
				float mindist = Mathf.Infinity;
				EndPoint closestEndpoint = null;
				foreach( EndPoint endpoint in endpoints)
				{
					float dist = Vector3.Magnitude(point - endpoint.point);
					if (dist < mindist)
					{
						mindist = dist;
						closestEndpoint = endpoint;
					}
				}

				if ( mindist < kEndPointProximityThreshold )
				{
					activeEndPoint = closestEndpoint;
					//Debug.Log ("Edges " + edges.Count);
					//Debug.Log ("EndPoints " + endpoints.Count);
				}
			}
			else
			{
				// release dragged point 

				bool bFoundExistingEndPoint = false;
				EndPoint endPoint = null;


				float mindist = Mathf.Infinity;
				EndPoint closestEndpoint = null;
				foreach (EndPoint endpoint in endpoints) 
				{
					if (endpoint != activeEndPoint)
					{
						float dist = Vector3.Magnitude (point - endpoint.point);
						if (dist < mindist) 
						{
							mindist = dist;
							closestEndpoint = endpoint;
						}
					}
				}
				
				if (mindist < kEndPointProximityThreshold) 
				{
					endPoint = closestEndpoint;
					bFoundExistingEndPoint = true;
				} else {
					bFoundExistingEndPoint = false;
				}

				if ( bFoundExistingEndPoint == true)
				{
					// point activeEndPoint edges at existing end point
					foreach (Edge edge in activeEndPoint.edges)
					{
						for (int epIndex = 0;  epIndex < 2; epIndex ++)
						{
							if ( edge.points[epIndex] == activeEndPoint)
							{
								edge.points[epIndex] = endPoint;

								LineRenderer lr = edge.gfx.GetComponent<LineRenderer>();
									
								Vector3 pointPos = edge.points[epIndex].point;
								pointPos.y += kEdgeY;
								lr.SetPosition (epIndex, pointPos);



								endPoint.edges.Add(edge);
							}
						}
						edge.UpdateCollisionObject(edge.points[0].point, edge.points[1].point);

					}

					//remove activeEndPoint
					foreach (EndPoint ep in endpoints)
					{
						if (ep == activeEndPoint)
						{
							endpoints.Remove(ep);
							break;
						}
					}
				}

				// update GFX

				activeEndPoint = null;
			}
		} 
		
	}



	public void removePoint( Vector3 point ) 
	{
		if (activeEndPoint != null) {
			// point activeEndPoint edges at existing end point
			foreach (Edge edge in activeEndPoint.edges) {
				for (int epIndex = 0; epIndex < 2; epIndex ++) {
					if (edge.points [epIndex] != activeEndPoint) {
						edge.points [epIndex].edges.Remove (edge);

						if (edge.points [epIndex].edges.Count == 0) { // issolated point
							endpoints.Remove (edge.points [epIndex]);
						}

						Object.Destroy (edge.gfx);

						Object.Destroy (edge.collisionObjs[0]);
						Object.Destroy (edge.collisionObjs[1]);

						edges.Remove (edge);
					}
				}
			}
				
			endpoints.Remove (activeEndPoint);
			activeEndPoint = null;

		} else {
			// remove wall section at point position

			Edge nearestEdge = null;
			float mindist = Mathf.Infinity;
			foreach (Edge edge in edges) 
			{
				float dist = VectorUtils.DistancePointLine( point, edge.points[0].point, edge.points[1].point);
				if ( dist < mindist)
				{
					mindist = dist;
					nearestEdge = edge;
				}
			}

			if ( mindist < kEndPointProximityThreshold)
			{
				for (int epIndex = 0; epIndex < 2; epIndex ++) 
				{
					nearestEdge.points [epIndex].edges.Remove (nearestEdge);
					if (nearestEdge.points [epIndex].edges.Count == 0) 
					{ // issolated point
						endpoints.Remove (nearestEdge.points [epIndex]);
					}
				}
				Object.Destroy (nearestEdge.gfx);
				Object.Destroy (nearestEdge.collisionObjs[0]);
				Object.Destroy (nearestEdge.collisionObjs[1]);
				edges.Remove (nearestEdge);
			}
		}
	}


		
}

