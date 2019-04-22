using UnityEngine;
using UnityEngine.UI;

public class EnemyUnity : BasicCharacter {
	//チャージ弾のプレハブ
	public GameObject chargeBulletPrefab;

	//オーディオ関連
	//声を出させるためのAudioSource
	private AudioSource univoice;
	//それぞれのボイス
	public AudioClip voice_damage;
	public AudioClip voice_down;
	public AudioClip voice_smallbullet;
	public AudioClip voice_chargebullet;

	protected override void Start() {
		base.Start();
		univoice = GetComponent<AudioSource>();
		//パラメータの設定
		HP = 50f;
		moveSpeed = 1.0f;
		shootInterval = 2.0f;
		hitInterval = 2.0f;
		//HP表示のキャンバスとテキストの取得
		hitPointCanvas = GameObject.Find("EnemyUnity/HPCanvas");
		hitPointText = hitPointCanvas.GetComponentInChildren<Text>();
	}

	protected override void Update() {
		base.Update();
		//現在がHit中またはDown中で無ければ移動と射撃
		stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (!stateInfo.IsName("Damage") && !stateInfo.IsName("Down") && gameSystem.sceneState == SceneState.battle) {
			MoveCharacter();
			ShootBullet();
		}

		//HP管理
		hitPointText.text = HP.ToString();
		hitPointCanvas.transform.LookAt(cameraObject.transform);
	}


	//移動処理のチェック
	private void MoveCharacter() {
		//x軸方向でプレイヤーに追従するためのベクトルを生成
		Vector3 direction = new Vector3(0, 0, 0);
		if (transform.position.x < playerObject.transform.position.x - 0.1f)
			direction = Vector3.right;
		else if (transform.position.x > playerObject.transform.position.x + 0.1f)
			direction = Vector3.left;

		//移動方向に向きを変える.移動してない場合正面を向く.
		Vector3 forward;
		if (direction.sqrMagnitude > 0) {
			forward = Vector3.Slerp(
				transform.forward,
				direction,
				(360 / Vector3.Angle(transform.forward, direction)) * Time.deltaTime);
		} else {
			forward = Vector3.Slerp(
				transform.forward,
				Vector3.forward,
				(360 / Vector3.Angle(transform.forward, direction)) * Time.deltaTime);
		}
		transform.LookAt(transform.position + forward);

		//移動とアニメーション
		characterController.Move(direction * moveSpeed * Time.deltaTime);
		if (characterController.velocity.magnitude > 0)
			animator.SetBool("Move", true);
		else
			animator.SetBool("Move", false);
	}

	//shootInterval秒ごとに射撃する
	private void ShootBullet() {
		if (!shootFlag) {
			//プレイヤーの方を向かせる
			//transform.LookAt(playerObject.transform);
			//正面を向く
			transform.rotation = Quaternion.Euler(0f, 0f, 1f);


			//弾を生成する位置
			Vector3 vecBulletPos = bulletStartPosition.position;
			//進行方向にちょっと前へ
			vecBulletPos += transform.forward.normalized * 1.5f;

			//チャージ弾か、通常弾を発射するか乱数を用いて決定する
			if (Random.Range(0.0f, 10.0f) < 7.0f) {
				//通常弾生成
				Instantiate(bulletPrefab, vecBulletPos, transform.rotation);
				//通常弾を発射するときの声をセットする
				univoice.clip = voice_smallbullet;
			} else {
				//チャージ弾生成
				Instantiate(chargeBulletPrefab, vecBulletPos, transform.rotation);
				//チャージ弾を発射するときの声をセットする
				univoice.clip = voice_chargebullet;
			}
		
			univoice.Play();
			animator.SetTrigger("Shoot");

			shootFlag = true;
			lastShootTime = gameSystem.timeElapsed;
		}
	}

	//弾にヒットした場合の動作   
	private void OnTriggerEnter(Collider hitCollider) {
		//前回の被弾から一定時間以内に当たったら何もしない
		if (!hitFlag) {
			//ヒットアクション
			animator.SetTrigger("Hit");
			univoice.clip = voice_damage;
			univoice.Play();

			//HPが0になった時の処理
			if (HP <= 0) {
				//やられたときのアニメーションを出す
				animator.SetBool("Down", true);
				univoice.clip = voice_down;
				univoice.Play();
				//シーン変遷のフラグ立て
				gameSystem.sceneState = SceneState.win;
			}

			//被弾時間の更新
			hitFlag = true;
			lastHitTime = gameSystem.timeElapsed;
		}
	}
}