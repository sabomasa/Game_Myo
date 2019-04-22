using UnityEngine;
using UnityEngine.UI;

public class BasicCharacter : MonoBehaviour {
	//ヒットポイント
	public float HP;
	//移動速度
	protected static float moveSpeed;

	//キャラクターコントローラー(制御)、アニメーター(モーション)と現在の状態
	protected CharacterController characterController;
	protected Animator animator;
	protected AnimatorStateInfo stateInfo;

	//発射する弾のプレハブ・発射位置
	public GameObject bulletPrefab;
	public Transform bulletStartPosition;
	//弾を発射する最小間隔
	protected float shootInterval;
	//前回の発射時間
	protected float lastShootTime;

	//被弾する最小間隔
	protected float hitInterval;
	//前回の被弾時間
	public float lastHitTime;
	//被弾中の点滅間隔
	protected float blinkInterval = 0.1f;
	//点滅時間
	protected float blinkTime = 0.0f;

	//弾を発射出来るかどうか
	protected bool shootFlag = true;
	//攻撃を食らえるかどうか
	public bool hitFlag = false;

	//HP表示
	protected GameObject hitPointCanvas;
	protected Text hitPointText;


	//カメラを指すオブジェクト(自機用)
	protected GameObject cameraObject;
	//自機を指すオブジェクト(敵用)
	protected GameObject playerObject;


	//ゲームシステムを指すオブジェクト(ここからtimeElapsedやmyo等にアクセス出来る)
	protected GameSystem gameSystem;

	//共通の初期化処理
	protected virtual void Start () {
		gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
		characterController = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();
		playerObject = GameObject.Find("Player");
		cameraObject = GameObject.Find("CameraController");

		//上記以外は必要分実装すること
	}
	
	protected virtual void Update () {
		//被弾・弾発射間隔の調整
		if (gameSystem.timeElapsed - lastShootTime > shootInterval)
			shootFlag = false;
		if (gameSystem.timeElapsed - lastHitTime > hitInterval)
			hitFlag = false;

		//無敵時間中は点滅
		if (!hitFlag) {
			hitPointCanvas.GetComponent<Canvas>().enabled = true;
		} else {
			blinkTime += Time.deltaTime;
			if (blinkTime > blinkInterval) {
				hitPointCanvas.GetComponent<Canvas>().enabled = !hitPointCanvas.GetComponent<Canvas>().enabled;
				blinkTime = 0;
			}
		}
		//HP0なら消す
		if(HP<=0)
			hitPointCanvas.GetComponent<Canvas>().enabled = false;
	}
}
