using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTeamManager : MonoBehaviour {
	public static DroneTeamManager GlobalAccess;

	public List<DroneController> team1Drones;
	public List<DroneController> team2Drones;

	// Use this for initialization
	void Awake () {
		GlobalAccess = this;

		team1Drones = new List<DroneController> ();
		team2Drones = new List<DroneController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Transform GetTarget(int teamToGetTargetFor) {
		if (teamToGetTargetFor == 1) {
			// Get Targets for Team 1
			if (team2Drones.Count > 0) {
				int randomTargetIndex = Random.Range (0, team2Drones.Count);
				return team2Drones [randomTargetIndex].transform;
			} else {
				return null;
			}
		} else if (teamToGetTargetFor == 2) {
			// Get Targets for Team 2
			if (team1Drones.Count > 0) {
				int randomTargetIndex = Random.Range (0, team1Drones.Count);
				return team1Drones [randomTargetIndex].transform;
			} else {
				return null;
			}
		} else {
			return null;
		}
	}

	public void RegisterDrone(int teamToRegisterTo, DroneController droneScriptToAdd) {
		if (teamToRegisterTo == 1) {
			team1Drones.Add (droneScriptToAdd);
		} else if (teamToRegisterTo == 2) {
			team2Drones.Add (droneScriptToAdd);
		}
	}
	public void UnregisterDrone(int teamToUnregisterFrom, DroneController droneScriptToRemove) {
		if (teamToUnregisterFrom == 1) {
			team1Drones.Remove (droneScriptToRemove);
		} else if (teamToUnregisterFrom == 2) {
			team2Drones.Remove (droneScriptToRemove);
		}
	}
}
