using UnityEngine;
using UnityEngine.UI;

public class EnemyTurret : BasicCharacter {
	protected override void Start() {
		base.Start();
		//パラメータの設定
		HP = 50f;
		shootInterval = 3.0f;
		hitInterval = 2.0f;

		//HP表示のキャンバスとテキストの取得
		hitPointCanvas = GameObject.Find("Turret(Clone)/HitPoint");
		hitPointText = hitPointCanvas.GetComponentInChildren<Text>();	
	}

	private float deathTimer = 0f;

	protected override void Update() {
		base.Update();
		if(deathTimer == 0f && (gameSystem.sceneState == SceneState.battle))
			CheckMove();

		//HP管理と向き調整
		if (HP > 0) {
			hitPointText.text = HP.ToString();
			hitPointCanvas.transform.LookAt(cameraObject.transform);
		} else {
			deathTimer += Time.deltaTime;
			if (deathTimer >= 1.5f) {
				Destroy(destroyEffect);
				Destroy(gameObject);
			}
		}
	}

	public GameObject destroyEffectPrefab;
	private GameObject destroyEffect;

	//弾にヒットした場合の動作   
	private void OnTriggerEnter(Collider hitCollider) {
		//前回の被弾から一定時間以内に当たったら何もしない
		if (!hitFlag) {
			//HPが0になった時の処理
			if (HP <= 0) {
				//爆発エフェクトを出す
				destroyEffect = Instantiate(destroyEffectPrefab, transform.position, transform.rotation) as GameObject;
			}
			//被弾時間の更新
			hitFlag = true;
			lastHitTime = gameSystem.timeElapsed;
		}
	}

	//自機向き発射用
	private float rotateTime;
	private Transform target;
	private Transform self;

	private void CheckMove() {
		//上下に適当に移動
		transform.position = new Vector3(0f, 2.0f + Mathf.Sin(( 2 * Mathf.PI * gameSystem.timeElapsed ) / 5.0f) * 0.8f, -10f);

		//弾が撃てるようになった時点での自機の位置に向き直り、発射する
		if (!shootFlag) {
			if(gameSystem.timeElapsed - rotateTime <= 1.0f) {
				//徐々に自機の方を向く
				Quaternion start = self.rotation;
				Quaternion end = Quaternion.LookRotation(target.position - self.position);
				float ratio = gameSystem.timeElapsed - rotateTime;
                bulletStartPosition.transform.rotation = Quaternion.Slerp(start,end,ratio);
			} else {
				//弾発射
				ShootBullet();
			}
		} else {
			target = cameraObject.transform;
			self = bulletStartPosition;
			rotateTime = gameSystem.timeElapsed;
		}
	}

	private void ShootBullet() {
		//弾を生成する位置(進行方向にちょっと前へ)
		Vector3 vecBulletPos = bulletStartPosition.position;
		vecBulletPos += bulletStartPosition.transform.forward.normalized * 3f;
		//通常弾生成
		GameObject shoot = Instantiate(bulletPrefab, vecBulletPos, bulletStartPosition.rotation);
		//弾のパラメータの調整
		shoot.GetComponent<EnemyBullet>().bulletMoveSpeed = 8.0f;
		shoot.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		//発射時間の更新
		shootFlag = true;
		lastShootTime = gameSystem.timeElapsed;
	}
}
