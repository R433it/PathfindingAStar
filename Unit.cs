using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public Transform target;
	private Vector3 trgt;
	float speed = 5;
	Vector3[] path;
	int targetIndex;

	void Start(){
		trgt = target.position;
		PathRequestManager.RequestPath (transform.position, trgt, OnPathFound);
		//PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);

	}

	void Update(){

		if (Input.GetKey(KeyCode.Space)){
			SetDestination(target.position);
			Debug.Log ("Pressed");
		}
		//SetDestination (GameObject.Find ("Target").transform.position);
		//PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
	}

	public void OnPathFound(Vector3[] newPath,bool pathSuccessful){
		if (pathSuccessful) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	IEnumerator FollowPath(){
		Vector3 currentWaypoint = path [0];

		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}

			transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos(){
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path [i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				} else {
					Gizmos.DrawLine (path [i - 1], path [i]);
				}
			}
		}
	}

	public bool SetDestination( Vector3 _target){
		Vector3 endPos;
		if(_target != null){
			endPos = _target;
			PathRequestManager.RequestPath (transform.position, endPos, OnPathFound);
			return true;
		}
		else{
			return false;
		}
	}

	public void Stop(){
		StopCoroutine ("FollowPath");
	}

	public void Resume(){
		StartCoroutine ("FollowPath");
	}
}
