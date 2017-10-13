using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//totally not a hackjob ayyyeee

public class FixTransform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {		
		transform.rotation = Quaternion.Euler (90, 0, 0);
	}
}