using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAnimController:MonoBehaviour {

	private Animator anim;
	public GameObject LeadDancerContainer;
	private GameObject LeadDancer;

	public GameObject CreditsBook;
	private bool CreditsCurrentState = false;

	// Use this for initialization
	void Awake() {
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {

	}

	private void OnGameStart() {
		anim.SetBool("GameActive", true);
		anim.SetBool("MenuCreditsActive", false);
	}

	void OnMainMenu() {
		anim.SetBool("GameActive", false);
		anim.SetBool("MenuCreditsActive", false);
	}

	void OnTutorial() {
		anim.SetBool("GameActive", true);
		anim.SetBool("MenuCreditsActive", false);
	}

	void OnCredits() {
		anim.SetBool("GameActive", false);
		anim.SetBool("MenuCreditsActive", true);
	}

	//Animation event
	public void ToggleLeadOff() {
		//LeadDancer = LeadDancerContainer.GetCurrentLead();
		LeadDancer.SetActive(!LeadDancer.gameObject.activeSelf);
	}

	public void ToggleCreditsOpen() {
		if(CreditsCurrentState) {
			CreditsCurrentState = false;
		} else {
			CreditsCurrentState = true;
		}

		CreditsBook.GetComponent<Animator>().SetBool("CreditsBookOpen", CreditsCurrentState);
	}

}
