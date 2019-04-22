using UnityEngine;
using Pose = Thalmic.Myo.Pose;
using VibrationType = Thalmic.Myo.VibrationType;

public class Player : BasicCharacter {
	//チャージ弾のプレハブ
	public GameObject chargeBulletPrefab;

	//チャージ弾関連
	public static float chargeGage = 100f;
	public static float _charge = 100.0f;

    public Camera mainCamera;

	//オーディオ関連
	private AudioSource SE;
	public AudioClip SE_damage;

	protected override void Start() {
		base.Start();
		SE = GetComponent<AudioSource>();
		SE.clip = SE_damage;
		//パラメータ設定
		HP = 100f;
		shootInterval = 1.0f;
		hitInterval = 2.0f;
		//HP表示のキャンバスとテキストの取得
		hitPointCanvas = GameObject.Find("Canvas");
	}

	protected override void Update() {
		base.Update();
        bulletStartPosition = GameObject.Find("BulletStartPos").GetComponent<Transform>();
		//移動
		MovePlayer();
        //射撃
        if (gameSystem.sceneState == SceneState.battle)
            ShootBullet();
	}


	//キャラをカメラに追随させる
	void MovePlayer() {
		//座標をあわせる
		transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y-1.4f, mainCamera.transform.position.z+0.05f);
		//向きの調整
		playerObject.transform.rotation = Quaternion.Euler(0, cameraObject.transform.eulerAngles.y, 0);
	}

	//腕の角度で弾発射かチャージかを分ける
	void ShootBullet() {
		//腕を上げていて前回発射のフラグが立っていなければ弾発射、腕を上げていなければチャージ
		if (gameSystem.myo.transform.localEulerAngles.x < 45 || gameSystem.myo.transform.localEulerAngles.x > 300) {
			if (!shootFlag) {
				//チャージ弾を打つ条件が整っているならチャージ弾発射し、ゲージを0にする
				if (_charge >= 30 && ( /*gameSystem.myo.pose == Pose.FingersSpread || */gameSystem.myo.pose == Pose.WaveOut/* || gameSystem.myo.pose == Pose.WaveIn */)) {
					Instantiate(chargeBulletPrefab, bulletStartPosition.position, bulletStartPosition.rotation);
                    _charge -= 30; ;
				//条件を満たしていなければ通常弾発射
				} else if(_charge >= 10){
					Instantiate(bulletPrefab, bulletStartPosition.position + bulletStartPosition.transform.forward.normalized, bulletStartPosition.rotation);
                    //_charge -= 10;
				}
				shootFlag = true;
				lastShootTime = gameSystem.timeElapsed;
			}
		} else {
			//時間経過でチャージが溜まっていく処理
			_charge += 30f * Time.deltaTime;
			//100がMAXなので100以上は溜まらないようにする
			if (_charge >= 100f)
				_charge = 100f;
		}
	}

	//弾に当たった時の処理
	void OnTriggerEnter(Collider hitCollider) {
        if (hitCollider.gameObject.tag == "ChargeBullet" || hitCollider.gameObject.tag == "Bullet"/* || gameSystem.sceneState != SceneState.battle*/)
            return;
		//前回の被弾から一定時間以内に当たったら何もしない
		if (!hitFlag) {
			//Drone専用
			if(hitCollider.gameObject.tag == "DroneBullet") {
				this.HP -= 10f;
				SE.Play();
			}
			//HPが0になった時の処理
			if (HP <= 0) {
                gameSystem.sceneState = SceneState.lose;
			}
            //被弾時に振動させる
            gameSystem.myo.Vibrate(VibrationType.Short);
			//被弾時間の更新
			hitFlag = true;
			lastHitTime = gameSystem.timeElapsed;
		}
	}
}