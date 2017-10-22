using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jukebox manages audio
/// TODO: Play another song when current ends
/// TODO: Fade music on menu
/// </summary>
public class JukeBox : MonoBehaviour {

    public delegate void Beat();
    public static event Beat OnBeat;

    public int BPM = 60;
    private float timeBetweenBeets;
    private float beetTime;

    private AudioSource _myAudio;

    private bool isMuting;

    // Use this for initialization
    void Awake () {
        timeBetweenBeets = 60f / BPM;
        _myAudio = GetComponent<AudioSource>();
        _myAudio.Play();
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
