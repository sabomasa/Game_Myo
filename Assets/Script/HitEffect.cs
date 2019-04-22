using UnityEngine;

public class HitEffect : MonoBehaviour {
	//生存時間
	private float timeLimit = 0.3f;

	void Start () {	
	}
	
	void Update () {
		//消去
		timeLimit -= Time.deltaTime;
		if (timeLimit < 0f)
			Destroy(gameObject);
	}
}
