using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {
	//カメラの移動速度
	private static float cameraSpeed = 2.0f;

	//カメラのx,y方向の移動限界
	private static float maxRangeX = 4.0f;
	private static float minRangeX = -4.0f;
	private static float maxRangeY = 1.4f;
	private static float minRangeY = 0.5f;

    public Camera mainCamera;
    public Vector3 cameraPosition;
    private string sceneName;

	void Start () {
        sceneName = SceneManager.GetActiveScene().name;
    }

	void Update () {
		//HMD装着モード
		if(GameSystem.VRmode) {
            switch (sceneName)
            {
                case "Stage1":
                    cameraPosition.x = -InputTracking.GetLocalPosition(XRNode.CenterEye).x;
                    cameraPosition.y = InputTracking.GetLocalPosition(XRNode.CenterEye).y - 1.4f;
                    cameraPosition.z = -InputTracking.GetLocalPosition(XRNode.CenterEye).z + 5.0f;
                    this.transform.position = cameraPosition;
                    break;
                case "Stage2":
                    cameraPosition.x = InputTracking.GetLocalPosition(XRNode.CenterEye).x * (-1.5f);
                    cameraPosition.y = InputTracking.GetLocalPosition(XRNode.CenterEye).y - 1.5f;
                    cameraPosition.z = InputTracking.GetLocalPosition(XRNode.CenterEye).z * (-2);
                    this.transform.position = cameraPosition;
                    break;
            }
		} 
		//デバッグモード
		else {
			DebugMoveCamera();
		}
	}


	//入力したx,y方向にカメラを移動させる
	void DebugMoveCamera() {
		Vector3 direction = new Vector3(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		transform.position += direction * cameraSpeed * Time.deltaTime;

		//カメラが端より外側に行かないようにする
		if (transform.position.x > maxRangeX)
			transform.position = new Vector3(maxRangeX, transform.position.y, transform.position.z);
		else if (transform.position.x < minRangeX)
			transform.position = new Vector3(minRangeX, transform.position.y, transform.position.z);
		if (transform.position.y > maxRangeY)
			transform.position = new Vector3(transform.position.x, maxRangeY, transform.position.z);
		else if (transform.position.y < minRangeY)
			transform.position = new Vector3(transform.position.x, minRangeY, transform.position.z);
	}
}
