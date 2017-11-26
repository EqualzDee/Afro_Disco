using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalker : MonoBehaviour {

	//Walk speed
	public float WalkSpeedMin = 0.5f;
	public float WalkSpeedMax = 1.5f;
	private float WalkSpeed;

	//Which animation will be played
	private int WalkCycle;

	//Sets the walking direction (default to forward)
	private Vector3 WalkDirection = Vector3.forward;

	//Makes the animation play at the right speed
	private int WalkingSpeedHash = Animator.StringToHash("WalkingSpeed");
	private int BlendHash = Animator.StringToHash("Blend");

	//Pointer to animator component
	private Animator myAnimator;

	// Use this for initialization
	void Start () {
		myAnimator = GetComponentInChildren<Animator>();

		//Gets a random walk speed and set the animation to play at the appropriate speed
		WalkSpeed = Random.Range(WalkSpeedMin, WalkSpeedMax);
		myAnimator.SetFloat(WalkingSpeedHash, WalkSpeed);

		//Chooses a random walk cycle
		WalkCycle = Random.Range(0, 2);
		myAnimator.SetFloat(BlendHash, WalkCycle);
	}
	
	// Update is called once per frame
	void Update () {
		//Moves them forward
		transform.Translate(WalkDirection * Time.deltaTime * WalkSpeed);
	}
}
