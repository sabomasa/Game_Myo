using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class GetRightControllerPosition : MonoBehaviour {

    public Vector3 rightControllerPosition;
    public Quaternion rightControllerRotation;
    public GameObject hosei;

    // Use this for initialization
    void Start () {
        
    }

	// Update is called once per frame
	void Update () {
        rightControllerPosition.x = - InputTracking.GetLocalPosition(XRNode.RightHand).x * 2;
        rightControllerPosition.y = InputTracking.GetLocalPosition(XRNode.RightHand).y-1.5f;
        rightControllerPosition.z = - InputTracking.GetLocalPosition(XRNode.RightHand).z * 2;

        rightControllerRotation = InputTracking.GetLocalRotation(XRNode.RightHand) * hosei.transform.rotation;


        this.transform.position = rightControllerPosition + new Vector3(-0.4f, 1.5f, 0);
        this.transform.rotation = Quaternion.Inverse(rightControllerRotation);

    }
 
}
