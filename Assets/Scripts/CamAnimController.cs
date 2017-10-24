using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAnimController : MonoBehaviour {

    private Animator anim;
    public GameObject Afro;

	// Use this for initialization
	void Awake ()
    {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGameStart()
    {
        anim.SetBool("GameActive", true);
    }

    void OnMainMenu()
    {
        anim.SetBool("GameActive", false);
    }

    void OnTutorial()
    {
        anim.SetBool("GameActive", true);
    }

    //Animation event
    public void ToggleAfroOff()
    {
        Afro.SetActive(!Afro.gameObject.activeSelf);
    }



}
