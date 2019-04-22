using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBarCtrl : MonoBehaviour {

    Slider _enemyHpBar;
    EnemyUnity enemy;

	// Use this for initialization
	void Start () {
        enemy = GetComponent<EnemyUnity>();
        _enemyHpBar = GameObject.Find("EnemyHpBar").GetComponent<Slider>();
        _enemyHpBar.maxValue = enemy.HP;
	}
	
	// Update is called once per frame
	void Update () {
        _enemyHpBar.value = enemy.HP;
	}
}
