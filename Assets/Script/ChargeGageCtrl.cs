using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeGageCtrl : MonoBehaviour {

    Slider _chargeGage;

	// Use this for initialization
	void Start () {
        _chargeGage = GameObject.Find("ChargeGage").GetComponent<Slider>();
        _chargeGage.maxValue = Player.chargeGage;
	}

	// Update is called once per frame
	void Update () {
        _chargeGage.value = Player._charge;		
	}
}
