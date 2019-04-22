using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDemoCamera : MonoBehaviour {

	private Transform myTransform;
	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
	}

	// Update is called once per frame
	void Update () {
		UpdateViewerCamera ();
	}

	// Orbit Camera Zoom and Rotation KeyCodes
	public KeyCode ZoomInKeyCode = KeyCode.E;
	public KeyCode ZoomOutKeyCode = KeyCode.Q;

	public KeyCode RotateUpKeyCode = KeyCode.W;
	public KeyCode RotateDownKeyCode = KeyCode.S;
	public KeyCode RotateLeftKeyCode = KeyCode.A;
	public KeyCode RotateRightKeyCode = KeyCode.D;


	// Viewer Camera Functions and Variables
	private float ViewCameraTargetRotationY = 15;
	private float ViewCameraRotationX = -45;
	private float ViewCameraZoomDistance = 3;
	private float DesiredCameraTargetRotationY = 15;
	private float DesiredCameraRotationX = -45;
	private float DesiredCameraZoomDistance = 8;
	public float ViewCameraRotationYSpeed = 60;
	public float ViewCameraRotationXSpeed = 60;
	public float ViewCameraZoomSpeed = 30;
	public float ViewCameraZoomMIN = 8;
	public float ViewCameraZoomMAX = 28;
	public Transform ViewCameraTargetTransform;
	public Transform ViewCameraHeightTransform;
	public Transform ViewCameraTransform;

	public bool LockToTargetTransform = false;
	public Transform TargetTransform;
	public float TrackingSpeed = 2.0f;
	public float TrackingYOffset = -2;

	public void ResetViewCameraToDefaults() {
		DesiredCameraTargetRotationY = 45;
		DesiredCameraRotationX = -45;
		DesiredCameraZoomDistance = 16;
	}
	public void RotateCameraCounterClockwise() {
		// Rotate Camera CounterClockwise
		DesiredCameraTargetRotationY += ViewCameraRotationYSpeed * 0.1f;
	}
	public void RotateCameraClockwise() {
		// Rotate Camera Clockwise
		DesiredCameraTargetRotationY -= ViewCameraRotationYSpeed * 0.1f;
	}
	public void RotateCameraUp() {
		// Rotate Camera Up
		DesiredCameraRotationX -= ViewCameraRotationXSpeed * 0.1f;
		if (DesiredCameraRotationX < -75)
			DesiredCameraRotationX = -75;
	}
	public void RotateCameraDown() {
		// Rotate Camera Down
		DesiredCameraRotationX += ViewCameraRotationXSpeed * 0.1f;
		if (DesiredCameraRotationX > 75)
			DesiredCameraRotationX = 75;
	}
	public void ZoomCameraIn() {
		// Zoom View Camera In
		DesiredCameraZoomDistance -= ViewCameraZoomMAX * 0.1f;
		if (DesiredCameraZoomDistance < ViewCameraZoomMIN)
			DesiredCameraZoomDistance = ViewCameraZoomMIN;
	}
	public void ZoomCameraOut() {
		// Zoom View Camera Out
		DesiredCameraZoomDistance += ViewCameraZoomMAX * 0.1f;
		if (DesiredCameraZoomDistance > ViewCameraZoomMAX)
			DesiredCameraZoomDistance = ViewCameraZoomMAX;
	}
	public void UpdateViewerCamera () {
		if (Input.GetKey (RotateLeftKeyCode)) {
			// Rotate Camera CounterClockwise
			DesiredCameraTargetRotationY += ViewCameraRotationYSpeed * Time.deltaTime;
		}
		if (Input.GetKey (RotateRightKeyCode)) {
			// Rotate Camera Clockwise
			DesiredCameraTargetRotationY -= ViewCameraRotationYSpeed * Time.deltaTime;
		}
		if (Input.GetKey (RotateUpKeyCode)) {
			// Rotate Camera Up
			DesiredCameraRotationX -= ViewCameraRotationXSpeed * Time.deltaTime;
			if (DesiredCameraRotationX < -75)
				DesiredCameraRotationX = -75;
		}
		if (Input.GetKey (RotateDownKeyCode)) {
			// Rotate Camera Down
			DesiredCameraRotationX += ViewCameraRotationXSpeed * Time.deltaTime;
			if (DesiredCameraRotationX > 75)
				DesiredCameraRotationX = 75;
		}
		if (Input.GetKey (ZoomInKeyCode)) {
			// Zoom View Camera In
			DesiredCameraZoomDistance -= ViewCameraZoomSpeed * Time.deltaTime;
			if (DesiredCameraZoomDistance < ViewCameraZoomMIN)
				DesiredCameraZoomDistance = ViewCameraZoomMIN;
		}
		if (Input.GetKey (ZoomOutKeyCode)) {
			// Zoom View Camera Out
			DesiredCameraZoomDistance += ViewCameraZoomSpeed * Time.deltaTime;
			if (DesiredCameraZoomDistance > ViewCameraZoomMAX)
				DesiredCameraZoomDistance = ViewCameraZoomMAX;
		}

		ViewCameraTargetRotationY = Mathf.Lerp (ViewCameraTargetRotationY, DesiredCameraTargetRotationY, Time.deltaTime);
		ViewCameraRotationX = Mathf.Lerp (ViewCameraRotationX, DesiredCameraRotationX, Time.deltaTime);
		ViewCameraZoomDistance = Mathf.Lerp (ViewCameraZoomDistance, DesiredCameraZoomDistance, Time.deltaTime / 2);

		if (ViewCameraTargetTransform != null) {
			ViewCameraTargetTransform.rotation = Quaternion.Euler (new Vector3 (0, ViewCameraTargetRotationY, 0));
		}
		if (ViewCameraHeightTransform != null) {
			ViewCameraHeightTransform.localRotation = Quaternion.Euler (new Vector3 (ViewCameraRotationX, 0, 0));
		}
		if (ViewCameraTransform != null) {
			ViewCameraTransform.localPosition = new Vector3 (0, 0, ViewCameraZoomDistance);
			ViewCameraTransform.LookAt (ViewCameraTargetTransform.position);
		}

		if (LockToTargetTransform) {
			if (TargetTransform != null) {
				myTransform.position = Vector3.Lerp (myTransform.position, TargetTransform.position + new Vector3(0, TrackingYOffset, 0), TrackingSpeed * Time.deltaTime);
			}
		}
	}
}
