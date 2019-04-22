using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public enum SceneState {
	start,
	battle,
	win,
	lose
}

public class GameSystem : MonoBehaviour {
	//ゲームのモード
	public static bool VRmode = true;

	//ゲームの経過時間を示すタイマー
	public float timeElapsed;

	//UnlimitedHandとMyoを指すオブジェクト
	public UH unlimitedHand;
	public ThalmicMyo myo;
	public ThalmicHub myoHub;

	//敵のプレハブ
	public GameObject turretPrefab;

	//UIの表示
	public GameObject clearText;
	public GameObject overText;
	public GameObject startText;

	//シーンの状態
	public SceneState sceneState;

	//現在のシーン名
	public string sceneName;

	//オーディオ関連
	//AudioSourceと被弾SE
	private AudioSource audioSource;
	public AudioClip bgm_battle;
	public AudioClip bgm_win;
	public AudioClip bgm_lose;

	void Start () {
		//初期化
		timeElapsed	= 0.0f;
		unlimitedHand = GameObject.Find("UnlimitedHand").GetComponent<UH>();
		myo	= GameObject.Find("Myo").GetComponent<ThalmicMyo>();
		audioSource = GetComponent<AudioSource>();
		sceneName = SceneManager.GetActiveScene().name;
		sceneState = SceneState.start;
		ShowStart();

		//戦闘bgmを鳴らす
		audioSource.clip = bgm_battle;
		audioSource.loop = true;
		audioSource.Play();

		switch (sceneName) {
			case "Stage1":
				//エネミー召喚
				Instantiate(turretPrefab, new Vector3(0, 0, -10), Quaternion.Euler(0, 0, 0));
				//開始から10秒で戦闘開始
				Invoke("ChangeStateBattle", 10.0f);
				break;

			case "Stage2":
				//開始から10秒で戦闘開始
				Invoke("ChangeStateBattle", 10.0f);
				break;
		}
	}

	//ゲームが終わった時に一度だけ処理を行うためのフラグ
	private bool endFlag = false;

	//ゲームの進行を管理する
	void Update () {
		timeElapsed += Time.deltaTime;

		switch (sceneName) {
			case "Stage1":
				//勝敗判定
				if (sceneState == SceneState.win) {
					//勝利時の一度切りの処理
					if (!endFlag) {
						ShowClear();
						audioSource.clip = bgm_win;
						audioSource.loop = false;
						audioSource.Play();
						Invoke("ChangeSceneStage2", 5.0f);
						endFlag = true;
					}
				}
				if (sceneState == SceneState.lose) {
					//敗北時の一度切りの処理
					if (!endFlag) {
						ShowOver();
						audioSource.clip = bgm_lose;
						audioSource.loop = false;
						audioSource.Play();
						Invoke("ChangeSceneStage2", 5.0f);
						endFlag = true;
					}
				}
				break;

			case "Stage2":
				//勝敗判定
				if (sceneState == SceneState.win) {
					//勝利時の一度切りの処理
					if (!endFlag) {
						ShowClear();
						audioSource.clip = bgm_win;
						audioSource.loop = false;
						audioSource.Play();
						Invoke("QuitGame", 10.0f);
						endFlag = true;
					}
				}
				if (sceneState == SceneState.lose) {
					//敗北時の一度切りの処理
					if (!endFlag) {
						ShowOver();
						audioSource.clip = bgm_lose;
						audioSource.loop = false;
						audioSource.Play();
						Invoke("QuitGame", 10.0f);
						endFlag = true;
					}
				}
				break;
		}
	}

	//Stage2へのシーンチェンジャー
	void ChangeSceneStage2() {
		SceneManager.LoadScene("Stage2");
		unlimitedHand.DestroyUH();
		myoHub.DestroyMyo();
	}

	//ゲーム終了
	void QuitGame() {
		Application.Quit();
		//エディタから起動してる場合はこっちで
		if (EditorApplication.isPlaying == true)
			EditorApplication.isPlaying = false;
	}

	//時間経過でバトルに移行する
	void ChangeStateBattle() {
		ShowBattle();
		sceneState = SceneState.battle;
	}

	//表示切り替え
	void ShowStart() {
		startText.SetActive(true);
		clearText.SetActive(false);
		overText.SetActive(false);
	}
	void ShowBattle() {
		startText.SetActive(false);
		clearText.SetActive(false);
		overText.SetActive(false);
	}
	void ShowClear() {
		startText.SetActive(false);
		clearText.SetActive(true);
		overText.SetActive(false);
	}
	void ShowOver() {
		startText.SetActive(false);
		clearText.SetActive(false);
		overText.SetActive(true);
	}
}
