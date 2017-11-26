using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerSelection : MonoBehaviour {

	//Tracks whether the menu is open
	private bool IsOpen = false;

	//The start and end positions for the menu
	///Will make this more robust later on, just want to test on android
	public Vector3 StartPos;
	public Vector3 EndPos;

	// Use this for initialization
	void Start () {
		//This is set like this to ensure that it is in the right place
		///The CloseMenu() method is not called here because that will eventually lerp, this should set it immediately
		IsOpen = false;
		transform.localPosition = StartPos;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Opens the menu
	public void OpenMenu() {
		IsOpen = true;
		transform.localPosition = EndPos;
	}

	//Closes the menu
	public void CloseMenu() {
		IsOpen = false;
		transform.localPosition = StartPos;
	}

	//Toggles the state of the menu and moves it appropriately
	public void ToggleState() {
		if(IsOpen) {
			CloseMenu();
		} else {
			OpenMenu();
		}
	}

	//Lerp function (Not functional atm)
	private void MoveBetweenPoints(Vector3 v1, Vector3 v2) {
		transform.localPosition = Vector3.Lerp(v1, v2, Time.deltaTime);
	}
}
