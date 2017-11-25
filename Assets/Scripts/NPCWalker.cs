using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalker : MonoBehaviour {

	public float WalkSpeedMin = 0.5f;
	public float WalkSpeedMax = 1.5f;
	private float WalkSpeed;

	private int WalkCycle;

	private Vector3 WalkDirection = Vector3.forward;
	private int WalkingSpeedHash = Animator.StringToHash("WalkingSpeed");
	private int BlendHash = Animator.StringToHash("Blend");

	private Animator myAnimator;

	// Use this for initialization
	void Start () {
		myAnimator = GetComponentInChildren<Animator>();
		WalkSpeed = Random.Range(WalkSpeedMin, WalkSpeedMax);
		myAnimator.SetFloat(WalkingSpeedHash, WalkSpeed);

		WalkCycle = Random.Range(0, 2);
		myAnimator.SetFloat(BlendHash, WalkCycle);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(WalkDirection * Time.deltaTime * WalkSpeed);
	}
}
