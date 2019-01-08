using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node {

	public int gridX;
	public int gridY;

	public bool walkable;
	public Vector3 worldPosition;
	public Node parent;

	public int gCost;
	public int hCost;
	public int fCost;
	public int cost;

	//public int mCost;
	/*
	public int fCost{
		get{
			return gCost + hCost;
		}
	}*/

	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
	//public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _cost) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		//cost = _cost;
		//mCost = _cost;
	}
}
	
