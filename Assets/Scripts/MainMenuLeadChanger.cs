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

	//These all need to be in the same order as they are above
	public GameObject[] Backups;

	//The currently selected lead dancer
	private int CurrentLeadNum = 0;

	//Pointer to the current object
	private GameObject SpawnedLead;
	private GameObject CurrentLead;
	private GameObject CurrentBackup;

	//Animation controller
	public RuntimeAnimatorController animator;

	public Board myBoard;

	// Use this for initialization
	void Start () {
		ChangeLead(0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject GetCurrentLead() {
		return SpawnedLead;
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
		if(SpawnedLead != null) {
			Destroy(SpawnedLead);
		}

		//Loads new lead dancer
		switch(CurrentLeadNum) {
		case 0:
			CurrentLead = DiscoLead;
			break;
		case 1:
			CurrentLead = RockLead;
			break;
		case 2:
			CurrentLead = PopLead;
			break;
		case 3:
			CurrentLead = SwingLead;
			break;
		case 4:
			CurrentLead = HiphopLead;
			break;

		default:
			//
			break;
		}

		//Spawns the lead dancer
		SpawnedLead = Instantiate(CurrentLead, new Vector3(0, 0, 0), Quaternion.identity, transform);

		//Sets up the appropriate backup dancer
		CurrentBackup = Backups[CurrentLeadNum];

		//Switches the dancers in the board
		myBoard.SetPlayerDancers(1, CurrentLead, CurrentBackup);

		//Deletes unnecessary components from the dancer prefabs
		Destroy(SpawnedLead.GetComponent<Dancer>());
		Destroy(SpawnedLead.GetComponent<Rigidbody>());

		//Sets default layer
		SpawnedLead.layer = 0;

		//Ensures the lead is in the correct position
		SpawnedLead.transform.localPosition = new Vector3(0, 0, 0);
		SpawnedLead.transform.localRotation = Quaternion.identity;
		SpawnedLead.transform.localScale = new Vector3(1, 1, 1);

		//Sets the correct animation controller
		SpawnedLead.GetComponent<Animator>().runtimeAnimatorController = animator as RuntimeAnimatorController;
	}
}
