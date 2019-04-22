using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour {

	public int TeamToSpawn = 1;
	public GameObject DronePrefab;

	private GameObject droneSpawned;

	// Use this for initialization
	void Start () {
		if (DronePrefab != null) {
			SpawnDrone ();	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (droneSpawned == null) {
			SpawnDrone ();
		}
	}

	private void SpawnDrone() {
		GameObject newDrone = GameObject.Instantiate (DronePrefab, transform.position, transform.rotation) as GameObject;
		droneSpawned = newDrone;
		DroneController droneScript = newDrone.GetComponent<DroneController> ();
		if (droneScript != null) {
			droneScript.DroneTeam = TeamToSpawn;
			droneScript.IsAIControlled = true;
			DroneTeamManager.GlobalAccess.RegisterDrone (TeamToSpawn, droneScript);
		}
	}
}
