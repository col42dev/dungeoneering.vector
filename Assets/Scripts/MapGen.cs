using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.IO;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

//using System.Reflection;



public class MapGen : MonoBehaviour {


	GameObject targetGameObj = null;

	//Prefabs
	public GameObject LineGFXPrefab;


	public class Edge {

		public Edge()
		{
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
			gfx.AddComponent<LineRenderer>();

			LineRenderer lr = gfx.GetComponent<LineRenderer>();

			lr.SetWidth (0.25f, 0.25f);

			Vector3 startPos = points[0].point;
			startPos.y += 0.5f;
			lr.SetPosition (0, startPos);
			
			Vector3 endPos = points[1].point;
			endPos.y += 0.5f;
			lr.SetPosition (1, endPos);


			Material mat = Resources.Load("Materials/LineGFX", typeof(Material)) as Material; 
			Material [] mats = new Material[1];
			mats[0] = mat;
			lr.materials = mats;

		}

		
		public GameObject  gfx;
		public List<EndPoint> points = new List<EndPoint> ();
	};
	
	public class EndPoint {
		public EndPoint()
		{
		}

		public EndPoint(Vector3 point) 
		{ 
			this.point = point; 
		}

		public Vector3 point;
		public List<Edge> edges = new List<Edge> ();
	};

	List<EndPoint> endpoints = new List<EndPoint> ();
	List<Edge> edges = new List<Edge> ();


	Edge activeEdge = new Edge(); 
	EndPoint activeEndPoint = null;
	VRoom activeRoom = null;

	public class VRoom
	{
		public List<EndPoint> endpoints = new List<EndPoint> ();
		public List<Edge> edges = new List<Edge> ();
	};

	List<VRoom> rooms = new List<VRoom> ();

	private const float kEndPointProximityThreshold = 1.0f;

	ObjImporter objImporter = new ObjImporter ();


	// Use this for initialization
	void Start () 
	{
		activeEdge.gfx = Object.Instantiate( LineGFXPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		targetGameObj = GameObject.Find ("Target");

		CreateRooms ();
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
			startPos.y += 0.5f;
			lr.SetPosition (0, startPos);
		
			if (targetGameObj != null) 
			{
				Vector3 endPos = targetGameObj.transform.position;
				endPos.y += 0.5f;
				lr.SetPosition (1, endPos);
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
						startPos.y += 0.5f;
						lr.SetPosition (0, startPos);
						
						Vector3 endPos = edge.points[1].point;
						endPos.y += 0.5f;
						lr.SetPosition (1, endPos);
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
				startPos.y += 0.5f;
				lr.SetPosition (0, startPos);
				
				Vector3 endPos = edge.points[1].point + targetPos;
				endPos.y += 0.5f;
				lr.SetPosition (1, endPos);
			}

		}

	}



	void CreateRooms()
	{

		VRoom r = new VRoom ();

		r.endpoints.Add (new EndPoint ( new Vector3(0, 0,0)));
		r.endpoints.Add (new EndPoint ( new Vector3(0, 0,4)));
		r.endpoints.Add (new EndPoint ( new Vector3(4, 0,4)));
		r.endpoints.Add (new EndPoint ( new Vector3(4, 0,0)));

		r.edges.Add (new Edge (r.endpoints[0], r.endpoints[1]));
		r.edges.Add (new Edge (r.endpoints[1], r.endpoints[2]));
		r.edges.Add (new Edge (r.endpoints[2], r.endpoints[3]));
		r.edges.Add (new Edge (r.endpoints[3], r.endpoints[0]));

		rooms.Add (r);


		r = new VRoom ();
		
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

		
		rooms.Add (r);
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
		
		if (mindist <= kEndPointProximityThreshold) {
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

	private Edge EdgeInProximity( Vector3 point)
	{
		Edge nearestEdge = null;
		float mindist = Mathf.Infinity;
		foreach (Edge edge in edges) 
		{
			float dist = VectorUtils.DistancePointLine (point, edge.points [0].point, edge.points [1].point);
			if (dist < mindist) 
			{
				mindist = dist;
				nearestEdge = edge;
			}
		}
	
		Edge returnNearestEdge = null;
		if (mindist < kEndPointProximityThreshold) 
		{
			returnNearestEdge = nearestEdge;
		}

		return returnNearestEdge;
	}

	private Vector3 NearestPointOnEdge( Vector3 point, Edge edge)
	{
		return   VectorUtils.ProjectPointLine(point, edge.points [0].point, edge.points [1].point);
	}

	private EndPoint SplitEdge(Edge edge, Vector3 atPoint)
	{
		Debug.Log ("Split Edges ");

		EndPoint ep = new EndPoint ();
		ep.point = atPoint;

		Edge edgeA = new Edge();
		{
			edgeA.gfx = Object.Instantiate (LineGFXPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
			edgeA.points.Add (edge.points [0]);
			edge.points [0].edges.Remove (edge);
			edge.points [0].edges.Add (edgeA);
			edgeA.points.Add (ep);
			LineRenderer lr = edgeA.gfx.GetComponent<LineRenderer> ();
			lr.SetWidth (0.25f, 0.25f);
			Vector3 startPos = edgeA.points[0].point;
			startPos.y += 0.5f;
			lr.SetPosition (0, startPos);
			Vector3 endPos = edgeA.points[1].point;
			endPos.y += 0.5f;
			lr.SetPosition (1, endPos);
		}
		ep.edges.Add (edgeA);

		Edge edgeB = new Edge();
		{
			edgeB.gfx = Object.Instantiate (LineGFXPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
			edgeB.points.Add (edge.points [1]);
			edge.points [1].edges.Remove (edge);
			edge.points [1].edges.Add (edgeB);
			edgeB.points.Add (ep);
			LineRenderer lr = edgeB.gfx.GetComponent<LineRenderer> ();
			lr.SetWidth (0.25f, 0.25f);
			Vector3 startPos = edgeB.points[0].point;
			startPos.y += 0.5f;
			lr.SetPosition (0, startPos);
			Vector3 endPos = edgeB.points[1].point;
			endPos.y += 0.5f;
			lr.SetPosition (1, endPos);
		}
		ep.edges.Add (edgeB);


		// Add new edges to global Edges list
		edges.Add (edgeA);
		edges.Add (edgeB);

		// Add new endpoint to global endpoints list.
		endpoints.Add (ep);

		// Remove original edge from global Edges list.
		Object.Destroy (edge.gfx);
		edges.Remove (edge);

		return ep;
	}



	public void OnSelectRoomTemplate(int index)
	{
		Debug.Log ("OnSelectRoomTemplate");
		activeRoom = rooms [index];
	}

	public void placeRoom(Vector3 point)
	{
		Debug.Log ("placeRoom");

		// update active room
		if (activeRoom != null)
		{
			Vector3 targetPos = targetGameObj.transform.position;
			
			for (int i = 0; i <  activeRoom.endpoints.Count; i ++)
			{
				activeRoom.endpoints[i].point += targetPos;

				EndPoint startEndPoint = new EndPoint();
				startEndPoint.point = activeRoom.endpoints[i].point;
				startEndPoint.edges = activeRoom.endpoints[i].edges;


				if ( MakeEndPoint(startEndPoint.point, ref startEndPoint))
				{	// new endpoint
					Edge nearestEdge = EdgeInProximity(startEndPoint.point);
					if (nearestEdge!= null)
					{
						Vector3 edgePoint =  NearestPointOnEdge( startEndPoint.point, nearestEdge);
						startEndPoint = SplitEdge( nearestEdge, edgePoint);

						foreach (Edge edge in activeRoom.endpoints[i].edges)
						{
							if (edge.points[0] == activeRoom.endpoints[i])
							{
								edge.points[0] = startEndPoint;
							}
							if (edge.points[1] == activeRoom.endpoints[i])
							{
								edge.points[1] = startEndPoint;
							}
							startEndPoint.edges.Add(edge);
						}
						activeRoom.endpoints[i] = startEndPoint;
					}
					else
					{
						foreach (Edge edge in activeRoom.endpoints[i].edges)
						{
							if (edge.points[0] == activeRoom.endpoints[i])
							{
								edge.points[0] = startEndPoint;
							}
							if (edge.points[1] == activeRoom.endpoints[i])
							{
								edge.points[1] = startEndPoint;
							}
							startEndPoint.edges.Add(edge);
						}
	
					
						activeRoom.endpoints[i] = startEndPoint;

						endpoints.Add( activeRoom.endpoints[i] );
					}
				}
				else
				{
					foreach (Edge edge in activeRoom.endpoints[i].edges)
					{
						if (edge.points[0] == activeRoom.endpoints[i])
						{
							edge.points[0] = startEndPoint;
						}
						if (edge.points[1] == activeRoom.endpoints[i])
						{
							edge.points[1] = startEndPoint;
						}
						startEndPoint.edges.Add(edge);
					}

					activeRoom.endpoints[i] = startEndPoint;
				}
			}

			foreach (Edge edge in activeRoom.edges)
			{
				edges.Add(edge);

				LineRenderer lr = edge.gfx.GetComponent<LineRenderer>();
				
				Vector3 startPos = edge.points[0].point;
				startPos.y += 0.5f;
				lr.SetPosition (0, startPos);
				
				Vector3 endPos = edge.points[1].point;
				endPos.y += 0.5f;
				lr.SetPosition (1, endPos);
			}
			
		}

		rooms.Clear ();
		CreateRooms ();
		activeRoom = null;
	}

	public void placePoint( Vector3 point) 
	{
		Debug.Log ("PP");
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
					Edge nearestEdge = EdgeInProximity(startPoint);
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
					Edge nearestEdge = EdgeInProximity(startEndPoint.point);
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
					Edge nearestEdge = EdgeInProximity(point);
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
				endPos.y += 0.5f;
				lr.SetPosition (1, endPos);

				// Add new Edge to global edge list
				edges.Add(activeEdge);

				// Update new Edge
				activeEdge = new Edge();
				activeEdge.gfx = Object.Instantiate( LineGFXPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
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
								pointPos.y += 0.5f;
								lr.SetPosition (epIndex, pointPos);

								endPoint.edges.Add(edge);
							}
						}

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
						edges.Remove (edge);
					}
				}
			}
				
			endpoints.Remove (activeEndPoint);
			activeEndPoint = null;

			Debug.Log ("Edges " + edges.Count);
			Debug.Log ("EndPoints " + endpoints.Count);
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

			//Debug.Log ("nearest edge  " +  mindist.ToString());

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
				edges.Remove (nearestEdge);
			}
		}
	}


		
}

