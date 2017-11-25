using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSwitcher : MonoBehaviour {
	//Toggle for whether a random environment is selected
	public bool EnableRandomEnvironment;

	//Array of environment prefabs
	public GameObject[] Environments;
	private GameObject SpawnedEnvironment;

	//The offset for spawning
	private Vector3 Offset = new Vector3(3.0f, -0.5f, 5.0f);

	//Current envrionment that is spawnned
	private int CurrentEnvironment;

	// Use this for initialization
	void Start () {
		if(EnableRandomEnvironment) {
			CreateParams();

			//Spawns the selected environment at the right location
			SpawnedEnvironment = Instantiate(Environments[CurrentEnvironment], Offset, Quaternion.identity, transform);
			SpawnedEnvironment.SetActive(true);
		}
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
