using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;

	public float remainingDistance { get; set; }
	public bool hasPath { get; set; }
	public bool pathPending { get; set; }
	public bool isStopped { get; set; }
	public Vector3 destination { get; set; }
	public float stoppingDistance { get; set; }


	private void Awake() {
		requestManager = GetComponent<PathRequestManager> ();
		grid = GetComponent<Grid> ();
	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos){
		StartCoroutine (FindPath (startPos, targetPos));
	}

	IEnumerator FindPath (Vector3 startPos, Vector3 targetPos){
		
		Stopwatch sw = new Stopwatch ();
		sw.Start ();

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		if (startNode.walkable && targetNode.walkable) {
			List<Node> OpenList = new List<Node>();
			HashSet<Node> ClosedList = new HashSet<Node>();

			startNode.fCost = startNode.gCost + GetEuclideanDistance (startNode, targetNode);
			float d = GetEuclideanDistance (startNode, targetNode);
			//Debug.Log ("Distance = " + d);
			OpenList.Add (startNode);

			while(OpenList.Count > 0 ){
				var lowestF = 0;
				Node CurrentNode = OpenList[lowestF];
				for (int i = 1; i < OpenList.Count;i++){

					if (OpenList[i].fCost < OpenList[lowestF].fCost) {
						lowestF = i;
					}

					OpenList[i].fCost = OpenList[lowestF].gCost +  GetEuclideanDistance(OpenList[lowestF], targetNode);

					if (OpenList[lowestF].fCost <= CurrentNode.fCost ){
						//if (OpenList[lowestF].hCost < CurrentNode.hCost){
						CurrentNode = OpenList[lowestF];
						//}
					}
				}

				if (CurrentNode == targetNode){
					sw.Stop();
					print ("path found : " + sw.ElapsedMilliseconds + "ms");
					pathSuccess = true;
					break;
				}


				OpenList.Remove(CurrentNode);
				ClosedList.Add (CurrentNode);

				foreach ( Node NeighboursNodes in grid.GetNeighbours(CurrentNode)){
					if (!NeighboursNodes.walkable || ClosedList.Contains(NeighboursNodes)){
						continue;
					}

					//int MoveCost = CurrentNode.gCost + GetEuclideanDistance(CurrentNode, NeighboursNodes);

					int MoveCost = CurrentNode.gCost + NeighboursNodes.cost;

					//Debug.Log ("Current Node gCost = "+MoveCost);
					if (MoveCost < NeighboursNodes.gCost || !OpenList.Contains(NeighboursNodes)){
						NeighboursNodes.gCost = MoveCost;
						NeighboursNodes.hCost = GetEuclideanDistance(NeighboursNodes, targetNode);
						NeighboursNodes.fCost = NeighboursNodes.gCost + NeighboursNodes.hCost; //
						NeighboursNodes.parent = CurrentNode;
						if (!OpenList.Contains(NeighboursNodes)){
							OpenList.Add(NeighboursNodes);
						}
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath(startNode, targetNode);
			pathSuccess = waypoints.Length > 0;
		}
		requestManager.FinishedProcessingPath (waypoints, pathSuccess);
	}


	Vector3[] RetracePath( Node StartingNode, Node EndNode){
		List<Node> path = new List<Node> ();
		Node CurrentNode = EndNode;

		while (CurrentNode != StartingNode) {
			path.Add (CurrentNode);
			CurrentNode = CurrentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		//waypoints.Reverse ();
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add (path [i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray ();
	}

	int GetManhattanDistance (Node nodeA, Node nodeB){
		int ix = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int iy = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		return ix + iy;
	}

	//Heuristic Euclidean Distance
	int GetEuclideanDistance (Node nodeA, Node nodeB){
		int ix = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int iy = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		float d = Mathf.Sqrt (ix * ix + iy * iy);
		return Mathf.RoundToInt (d);

	}

	int GetOctileDistance (Node nodeA, Node nodeB){
		int ix = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int iy = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		var F = Mathf.Sqrt(2f) - 1;
		F = Mathf.RoundToInt (F);
		//return (dx < dy) ? F * dx + dy : F * dy + dx;
		return (ix < iy) ? (int)F * ix + iy : (int)F * iy + ix;
	}
		
}