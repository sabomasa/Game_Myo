using UnityEngine;
using UnityEngine.UI;

public class EnemyDrone : BasicCharacter {
	public DroneController droneController;

	protected override void Start() {
		base.Start();

		//パラメータの設定
		HP = droneController.DroneHitPoints;
		shootInterval = 2.0f;
		hitInterval = 5.0f;

		//HP表示のキャンバスとテキストの取得
		hitPointCanvas = GameObject.Find("EnemyDrone/HPCanvas");
		hitPointText = hitPointCanvas.GetComponentInChildren<Text>();

		//ターゲットを自機に設定
		droneController.CurrentTarget = playerObject.transform;
	}

	protected override void Update() {
		base.Update();
        if(gameSystem.sceneState == SceneState.battle)
    		MoveDrone();

		//HP管理と向き調整
		hitPointText.text = HP.ToString();
		hitPointCanvas.transform.LookAt(cameraObject.transform);
		hitPointCanvas.transform.Rotate(0f, 180f, 0f);

		//死んだら敵オブジェクトごと消去
		if (droneController.droneDeathTimer > 0.7f)
			Destroy(this.gameObject);
	}

	//弾にヒットした場合の動作   
	private void OnTriggerEnter(Collider hitCollider) {
		//前回の被弾から一定時間以内に当たったら何もしない
		if (!hitFlag) {
            if (hitCollider.gameObject.tag == "ChargeBullet")
                droneController.TakeHitDamage(30);
            else
    			droneController.TakeHitDamage(15);
			//HPが0になった時の処理
			if (HP <= 0) {
				hitPointCanvas.GetComponent<Canvas>().enabled = false;
				gameObject.GetComponent<CapsuleCollider>().enabled = false;
                gameSystem.sceneState = SceneState.win;
			}
			//被弾時間の更新
			hitFlag = true;
			lastHitTime = gameSystem.timeElapsed;
		}
	}

	//移動エリアのサイズ
	private float areaXmin = -7.5f;
	private float areaXmax = 7.5f;
	private float areaYmin = 1.5f;
	private float areaYmax = 4.5f;
	private float areaZmin = -12f;
	private float areaZmax = -8f;
	//移動周期
	private int movePattern = 0;
	private bool attackFlag = false;
	//1秒に1回移動
	private float s_time = 3.0f;
	//球面補間用
	private Vector3 s_startPosition;
	private Vector3 s_targetPosition;
	//両面爆撃用フラグ
	private bool bombFlag = false;

	//ランダムな移動と攻撃
	private void MoveDrone() {
		s_time += Time.deltaTime;
		transform.LookAt(playerObject.transform);

		//はじめの1.5秒は移動
		if (s_time < 1.5f) {
			if(movePattern % 3 == 0) {
				transform.position = Vector3.Slerp(transform.position, s_targetPosition, Time.deltaTime);
				s_time -= Time.deltaTime / 3f;
			} else {
				transform.position = Vector3.Slerp(transform.position, s_targetPosition, Time.deltaTime * 2);
			}
		}
		//次は停止し攻撃 
		else if (s_time < 3.0f){
			if (!attackFlag) {
				switch (movePattern) {
					case 3:
						//3回目は強攻撃1
						if ((int)(100 * gameSystem.timeElapsed % 2) == 0)
							droneController.AttackAreaBombingLeft();
						else
							droneController.AttackAreaBombingRight();
						s_time -= 2.5f;
						break;
					case 6:
						//6回目は強攻撃2
						if (!bombFlag) {
							droneController.AttackAreaBombingRightSlow();
							s_time -= 1.5f;
							bombFlag = true;
						} else if (bombFlag) {
							droneController.AttackAreaBombingLeftSlow();
							s_time -= 2.5f;
							bombFlag = false;
						}
						break;
					default:
						//1,4回目は遅いの、2,5回目は複数way
						if (movePattern % 3 == 1)
							droneController.AttackSlow();
						else if (movePattern % 3 == 2)
							droneController.AttackWay();
						break;
				}
				if(!bombFlag)
					attackFlag = true;
			}
		}
		//その後は次の移動先を決めてループ
		else {
			float moveX = 0f, moveY = 0f, moveZ = 0f;
			if (movePattern != 6)
				movePattern++;
			else if (movePattern == 6)
				movePattern = 1;

			switch (movePattern) {
				case 3:
					//3回目は特定位置へ
					moveX = 0f; moveY = 5.5f; moveZ = -20f;
					break;
				case 6:
					//6回目は特定位置へ
					moveX = 0f; moveY = 3.0f; moveZ = -20f;
					break;
				default:
					//1,2,4,5回目はランダムな移動
					moveX = Random.Range(areaXmin, areaXmax);
					moveY = Random.Range(areaYmin, areaYmax);
					moveZ = Random.Range(areaZmin, areaZmax);
					break;
			}
			s_targetPosition = new Vector3(moveX, moveY, moveZ);
			s_startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			s_time = 0.0f;
			attackFlag = false;
			droneController.StayCool();
		}
	}
}
