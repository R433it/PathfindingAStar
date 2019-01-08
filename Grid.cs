using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour {

	public bool displayGridGizmos;
	public Transform player;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public float distance;
	//public int cost;
	//Node[,] grid;
	Node[,] grid;



	public List<Node> FinalPath;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];

		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = true;
				if (Physics.CheckSphere (worldPoint, nodeRadius, unwalkableMask)) {
					walkable = false;
				}
				//grid[x,y,cost] = new Node(walkable,worldPoint, x, y, cost);
				grid[x,y] = new Node(walkable,worldPoint, x, y);
			}
		}
	}


	 
	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float xpoint = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float ypoint = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		xpoint = Mathf.Clamp01(xpoint);
		ypoint = Mathf.Clamp01(ypoint);

		int x = Mathf.RoundToInt((gridSizeX-1) * xpoint);
		int y = Mathf.RoundToInt((gridSizeY-1) * ypoint);
		return grid[x,y];
	}



	public List<Node> GetNeighbours (Node node){
		List<Node> NeighboursNodes = new List<Node> ();
		int xCheck;
		int yCheck;


		//Right
		xCheck = node.gridX + 1;
		yCheck = node.gridY;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost = 10;
				grid [xCheck, yCheck].cost = 10;
				NeighboursNodes.Add (grid [xCheck, yCheck]);
				//Node nodeGrid = grid [xCheck, yCheck];
				//nodeGrid.cost = 10;
				//NeighboursNodes.Add (nodeGrid);

			}
		}
			
		//Left
		xCheck = node.gridX - 1;
		yCheck = node.gridY;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost = 10;
				grid [xCheck, yCheck].cost = 10;
				NeighboursNodes.Add (grid [xCheck, yCheck]);

			}
		}

		//Top
		xCheck = node.gridX;
		yCheck = node.gridY + 1;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost = 10;
				grid [xCheck, yCheck].cost = 10;
				NeighboursNodes.Add (grid [xCheck, yCheck]);
			}
		}

		//Bottom
		xCheck = node.gridX;
		yCheck = node.gridY - 1;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost = 10;
				grid [xCheck, yCheck].cost = 10;
				NeighboursNodes.Add (grid [xCheck, yCheck]);

			}
		}


		//Diagonal LeftTop
		xCheck = node.gridX - 1;
		yCheck = node.gridY + 1;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost=  14;
				grid [xCheck, yCheck].cost = 14;
				NeighboursNodes.Add (grid [xCheck, yCheck]);

			}
		}

		//Diagonal RightTop
		xCheck = node.gridX + 1;
		yCheck = node.gridY + 1;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost = 14;
				grid [xCheck, yCheck].cost = 14;
				NeighboursNodes.Add (grid [xCheck, yCheck]);

			}
		}

		//Diagonal LeftBottom
		xCheck = node.gridX - 1;
		yCheck = node.gridY - 1;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost = 14;
				grid [xCheck, yCheck].cost = 14;
				NeighboursNodes.Add (grid [xCheck, yCheck]);

			}
		}

		//Diagonal RightBottom
		xCheck = node.gridX + 1;
		yCheck = node.gridY - 1;
		if (xCheck >= 0 && xCheck < gridSizeX) {
			if (yCheck >= 0 && yCheck < gridSizeY) {
				//node.cost =  14;
				grid [xCheck, yCheck].cost = 14;
				NeighboursNodes.Add (grid [xCheck, yCheck]);

			}
		}
		return NeighboursNodes;

	}


	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

		if (grid != null) {
			Node playerNode = NodeFromWorldPoint (player.position);
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				if (playerNode == n) {
					Gizmos.color = Color.green;
					Debug.Log ("Player Node X = " + n.gridX);
					Debug.Log ("Player Node Y = " + n.gridY);
				}

				if (FinalPath != null) {
					if (FinalPath.Contains (n)) {
						Gizmos.color = Color.yellow;
					}
				}
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter- distance));
			}
		}
	}
}
