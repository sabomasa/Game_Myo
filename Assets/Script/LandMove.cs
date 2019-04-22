using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMove : MonoBehaviour {
	//ゲームシステムを指すオブジェクト
	private GameSystem gameSystem;

	//周期[s]
	public float period = 5.0f;
	//移動幅
	public float length = 0.5f;

	void Start () {
		gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
	}
	
	void Update () {
		Vector3 pos = transform.position;
		pos.y += Mathf.Sin(( 2 * Mathf.PI * gameSystem.timeElapsed ) / period) * length * Time.deltaTime;
		transform.position = pos;
	}
}
