using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
}