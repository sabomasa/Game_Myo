using UnityEngine;

public class EnemyBullet : MonoBehaviour {
	//弾が進むスピード
	public float bulletMoveSpeed = 2.0f;
	//弾の生存時間
	private float timeLimit = 5.0f;

	//オーディオ関連
	//AudioSourceと被弾SE
	private AudioSource SE;
	public AudioClip SE_damage;
	public AudioClip SE_bulletStart;

	void Start() {
		SE = GetComponent<AudioSource>();
		SE.PlayOneShot(SE_bulletStart);
	}

	void Update() {
		Vector3 vecAddPos = Vector3.forward * bulletMoveSpeed;
		transform.position += ( transform.rotation * vecAddPos ) * Time.deltaTime;

		//一定時間経過で消去
		timeLimit -= Time.deltaTime;
		if (timeLimit < 0f)
			Destroy(gameObject);
	}

	//ヒットエフェクトのオブジェクト
	public GameObject hitEffectPrefab;

	private void OnTriggerEnter(Collider hitCollider) {
		//自機と当たった場合
		if (hitCollider.gameObject.tag == "Player") {
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
			if (hitCollider.gameObject.GetComponent<BasicCharacter>().hitFlag != true) {
				hitCollider.gameObject.GetComponent<BasicCharacter>().HP -= 10;
				AudioSource.PlayClipAtPoint(SE_damage, transform.position);
			}
			Destroy(gameObject);
		}
		//敵自身と当たった場合
		else if (hitCollider.gameObject.tag == "Enemy") {
		}
		//それ以外と当たった場合
		else {
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}