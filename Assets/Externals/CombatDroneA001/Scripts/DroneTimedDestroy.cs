using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTimedDestroy : MonoBehaviour {

	private float lifeTime = 0;
	public float LifeTimeSeconds = 1.5f;
		
	// Update is called once per frame
	void Update () {
		if (lifeTime < LifeTimeSeconds) {
			lifeTime += Time.deltaTime;
		} else {
			Destroy (gameObject);
		}
	}
}
