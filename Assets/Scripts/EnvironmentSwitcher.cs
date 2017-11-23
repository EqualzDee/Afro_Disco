using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSwitcher : MonoBehaviour {
	//Array of environment prefabs
	public GameObject[] Environments;

	//The offset for spawning
	private Vector3 Offset = new Vector3(3.0f, -0.5f, 5.0f);

	//Current envrionment that is spawnned
	private int CurrentEnvironment;


	// Use this for initialization
	void Start () {
		CreateParams();

		//Spawns the selected environment at the right location
		GameObject SpawnedEnvironment = GameObject.Instantiate(Environments[CurrentEnvironment], Offset, Quaternion.identity) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//For setting up all the parameters
	void CreateParams() {
		//Picks a random environment to be spawned
		CurrentEnvironment = Random.Range(0, Environments.Length);
	}
}
