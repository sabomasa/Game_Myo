using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBarCtrl : MonoBehaviour {

	Slider _playerHpBar;
	Player player;

	// Use this for initialization
	void Start() {
		player = GetComponent<Player>();
		_playerHpBar = GameObject.Find("PlayerHpBar").GetComponent<Slider>();
		_playerHpBar.maxValue = 100f;
	}

	// Update is called once per frame
	void Update() {
		_playerHpBar.value = player.HP;
	}
}
