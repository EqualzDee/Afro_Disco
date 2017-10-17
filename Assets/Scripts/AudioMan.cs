using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMan : MonoBehaviour {

    public delegate void Beat();
    public static event Beat OnBeat;

    public int BPM = 60;
    private float timeBetweenBeets;
    private float beetTime;

    private bool isMuting;

    // Use this for initialization
    void Start () {
        timeBetweenBeets = 60f / BPM;
        GetComponent<AudioSource>().Play();
        //if (OnBeat != null) OnBeat();
    }
	
	// Update is called once per frame
	void Update () {
        beetTime += Time.deltaTime;
        if(beetTime > timeBetweenBeets) //Groove the beet mon
        {
            beetTime -= timeBetweenBeets;	//Stops the beat from drifting in the long term
            if (OnBeat != null) OnBeat();
        }
    }

    public void Mute()
    {
        isMuting = !isMuting;
        GetComponent<AudioSource>().mute = isMuting;
    }
}
