using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;


public class RightArmControl : MonoBehaviour {
	private ThalmicMyo myo;
	public Camera cameraObj;
	private GameObject bulletStartPos;

	float compX = -75f;
	float compY = 91f;
	float compZ = 86f;

	public Vector3 vector;

	void Start() {
		myo = GameObject.Find("Myo").GetComponent<ThalmicMyo>();
		bulletStartPos = GameObject.Find("BulletStartPos");
	}
	
	void Update () {
		if (GameSystem.VRmode) {
			transform.rotation = bulletStartPos.transform.rotation;
			transform.Rotate(new Vector3(0f, -20f, 0f));
			vector = transform.rotation.eulerAngles;
			//transform.rotation = Quaternion.Euler(, myo.transform.localEulerAngles.y, -myo.transform.localEulerAngles.z);
			//transform.localRotation = Quaternion.Euler(bulletStartPos.transform.eulerAngles.x + compX, bulletStartPos.transform.eulerAngles.y + compY, bulletStartPos.transform.eulerAngles.z + compZ);
		} else {
			//Ray ray = cameraObj.ScreenPointToRay(Input.mousePosition);
			//transform.rotation = Quaternion.LookRotation(ray.direction);
			transform.rotation = bulletStartPos.transform.rotation;
			transform.Rotate(new Vector3(0f, -20f, 0f));
			vector = transform.rotation.eulerAngles;
		}
	}
}
