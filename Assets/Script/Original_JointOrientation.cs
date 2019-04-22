using UnityEngine;

public class Original_JointOrientation : MonoBehaviour {
	private ThalmicMyo myo = null;
	public Vector3 support = Vector3.zero;
	private Camera cameraObj;

	//複数個存在しないようにする
	private static Original_JointOrientation _instance = null;
	public static Original_JointOrientation instance {
		get { return _instance; }
	}
	void Awake() {
		if (_instance != null) {
			Destroy(this.gameObject);
			return;
		} else {
			_instance = this;
		}
		DontDestroyOnLoad(this);
        support = new Vector3(-1f, -90f, 0f);
	}

	void Update() {
        myo = GameObject.Find("Myo").GetComponent<ThalmicMyo>();
        cameraObj = GameObject.Find("MainCamera").GetComponent<Camera>();
        if (GameSystem.VRmode) {
			//myoの指す先+補正
			if (Input.GetKeyDown("r"))
				support = new Vector3(/*0f,*/	-myo.transform.localRotation.eulerAngles.x,
									/*0f, */-myo.transform.localRotation.eulerAngles.y,
									0f		/*-myo.transform.localRotation.eulerAngles.z*/
									);
			transform.rotation = Quaternion.Euler(myo.transform.localRotation.eulerAngles + support);
			//ワールド座標なので反転
			transform.Rotate(new Vector3(0f, 180f, 0f));
		} else {
			//マウスの指す先
			Ray ray = cameraObj.ScreenPointToRay(Input.mousePosition);
			transform.rotation = Quaternion.LookRotation(ray.direction);
		}
		transform.position = GameObject.Find("Player/Ethan/EthanSkeleton/EthanHips/EthanSpine/EthanSpine1/EthanSpine2/EthanNeck/EthanRightShoulder/EthanRightArm").transform.position;
	}
}
