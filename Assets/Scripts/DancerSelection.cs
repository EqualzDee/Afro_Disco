using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerSelection:MonoBehaviour {

	//Tracks whether the menu is open
	private bool IsOpen = false;

	//The start and end positions for the menu
	public Vector3 StartPos;
	public Vector3 EndPos;

	//How long the movement should take
	public float MovementTime = 1.0f;

	// Use this for initialization
	void Start() {
		//This is set like this to ensure that it is in the right place
		IsOpen = false;
		transform.localPosition = StartPos;
	}

	// Update is called once per frame
	void Update() {

	}

	//Opens the menu
	public void OpenMenu() {
		IsOpen = true;
		StartCoroutine(MoveBetweenPoints(StartPos, EndPos));
	}

	//Closes the menu
	public void CloseMenu() {
		IsOpen = false;
		StartCoroutine(MoveBetweenPoints(EndPos, StartPos));
	}

	//Toggles the state of the menu and moves it appropriately
	public void ToggleState() {
		if(IsOpen) {
			CloseMenu();
		} else {
			OpenMenu();
		}
	}

	//Lerp function
	private IEnumerator MoveBetweenPoints(Vector3 v1, Vector3 v2) {

		//Tracks total time elapsed
		float ElapsedTime = 0;

		while(ElapsedTime < MovementTime) {
			//Moves between two points
			transform.localPosition = Vector3.Lerp(v1, v2, ElapsedTime / MovementTime);

			ElapsedTime += Time.deltaTime;

			yield return null;
		}
	}
}
