using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargeBullet : MonoBehaviour {
	//弾が進むスピード
	private float bulletMoveSpeed = 12.0f;
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
		//敵と当たった場合
		if (hitCollider.gameObject.tag == "Enemy") {
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
			if (hitCollider.gameObject.GetComponent<BasicCharacter>().hitFlag != true) {
				hitCollider.gameObject.GetComponent<BasicCharacter>().HP -= 30;
				AudioSource.PlayClipAtPoint(SE_damage, transform.position);
			}
			Destroy(gameObject);
		}
		//自機自身・通常弾と当たった場合
		else if (hitCollider.gameObject.tag == "Player" || hitCollider.gameObject.tag == "Bullet" || hitCollider.gameObject.tag == "DroneBullet") {
		}
		//(チャージ弾含む)それ以外と当たった場合
		else {
			Instantiate(hitEffectPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}
