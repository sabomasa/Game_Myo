using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneExplosionShard : MonoBehaviour {

	public GameObject ExplosionPrefab;
	public float ExplosionLifeTime = 10;

	public float LifeTime = 2.0f;
	private float destroyTimer = 0;
		
	// Update is called once per frame
	void Update () {
		if (destroyTimer < LifeTime) {
			destroyTimer += Time.deltaTime;
		} else {
			// Spawn Explosion
			if (ExplosionPrefab != null) {
				GameObject newDroneExplosion = GameObject.Instantiate (ExplosionPrefab, transform.position, transform.rotation) as GameObject;
				Destroy (newDroneExplosion, ExplosionLifeTime);				
			}

			Destroy (gameObject);
		}
	}
}
