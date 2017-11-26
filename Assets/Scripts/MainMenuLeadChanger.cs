using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLeadChanger : MonoBehaviour {

	//Prefabs for each lead dancer
	public GameObject DiscoLead;
	public GameObject RockLead;
	public GameObject PopLead;
	public GameObject SwingLead;
	public GameObject HiphopLead;

	//The currently selected lead dancer
	private int CurrentLeadNum = 0;

	//Pointer to the current object
	private GameObject CurrentLead;

	//Animation controller
	public RuntimeAnimatorController animator;

	// Use this for initialization
	void Start () {
		ChangeLead(0);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public GameObject GetCurrentLead() {
		return CurrentLead;
	}

	//Cycles the lead dancer for testing purposes
	public void CycleLead() {
		int temp = CurrentLeadNum + 1;

		if(temp >= 5) {
			temp %= 5;
		}

		ChangeLead(temp);
	}

	//Changes the lead dancer
	public void ChangeLead(int NewLead) {
		CurrentLeadNum = NewLead;

		//Deletes current lead
		if(CurrentLead != null) {
			Destroy(CurrentLead);
		}

		//Loads new lead dancer
		switch(CurrentLeadNum) {
		case 0:
			CurrentLead = Instantiate(DiscoLead, new Vector3(0, 0, 0) , Quaternion.identity, transform);
			break;
		case 1:
			CurrentLead = Instantiate(RockLead, new Vector3(0, 0, 0), Quaternion.identity, transform);
			break;
		case 2:
			CurrentLead = Instantiate(PopLead, new Vector3(0, 0, 0), Quaternion.identity, transform);
			break;
		case 3:
			CurrentLead = Instantiate(SwingLead, new Vector3(0, 0, 0), Quaternion.identity, transform);
			break;
		case 4:
			CurrentLead = Instantiate(HiphopLead, new Vector3(0, 0, 0), Quaternion.identity, transform);
			break;

		default:
			//
			break;
		}

		//Deletes unnecessary components from the dancer prefabs
		Destroy(CurrentLead.GetComponent<Dancer>());
		Destroy(CurrentLead.GetComponent<Rigidbody>());

		//Sets default layer
		CurrentLead.layer = 0;

		//Ensures the lead is in the correct position
		CurrentLead.transform.localPosition = new Vector3(0, 0, 0);
		CurrentLead.transform.localRotation = Quaternion.identity;
		CurrentLead.transform.localScale = new Vector3(1, 1, 1);

		//Sets the correct animation controller
		CurrentLead.GetComponent<Animator>().runtimeAnimatorController = animator as RuntimeAnimatorController;
	}
}
