using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChargeBullet : MonoBehaviour {
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
		//移動
		Vector3 vecAddPos = Vector3.forward * bulletMoveSpeed;
		transform.position += ( transform.rotation * vecAddPos ) * Time.deltaTime;

		//消去
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
				hitCollider.gameObject.GetComponent<BasicCharacter>().HP -= 20;
				AudioSource.PlayClipAtPoint(SE_damage, transform.position);
			}
			Destroy(gameObject);
		}
		//敵自身・通常弾と当たった場合
		else if (hitCollider.gameObject.tag == "Enemy" || hitCollider.gameObject.tag == "Bullet") {
		}
		//(チャージ弾含む)それ以外と当たった場合
		else {
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}
