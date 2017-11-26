using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAnimController:MonoBehaviour {

	private Animator anim;
	public MainMenuLeadChanger leadChanger;


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
	public void ToggleAfroOff() {

		var l = leadChanger.GetCurrentLead();
		l.SetActive(!l.gameObject.activeSelf);
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
