using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour {

    public UH unlimitedHand;

	// Use this for initialization
	void Start () {
        unlimitedHand.startAnglePR();
    }

    //update関数が何回実行されたか
    int count_low = 0;
    int count_high = 0;

    //腕を下げているときのAngleとフォトリフレクターの値
    public float[] uhAngle_low = new float[3];
    public float[] uhPr_low = new float[7];

    //腕を上げているときのAngleとフォトリフレクターの値
    public float[] uhAngle_high = new float[3];
    public float[] uhPr_high = new float[7];

    bool calibrationlowflag = false;
    bool calibrationhighflag = false;
    bool nextstep = false;
    
	// Update is called once per frame
	void Update () {
        DispMsg.dispMessage("UHを正しく接続してね");

        //UHが正常に動いていればキャリブレーションを開始する
        if (calibrationlowflag == false && unlimitedHand.UHAngle[0] != 0.0f)
        {
            Debug.Log("OK2");
            //update関数の実行回数分だけAngleの値を足していき、最後に実行回数で割ることで、平均値を算出する
            for (int i = 0; i < 3; i++)
            {
                uhAngle_low[i] += unlimitedHand.UHAngle[i];
            }
            for (int i = 0; i < 7; i++)
            {
                uhPr_low[i] += unlimitedHand.UHPR[i];
            }
            count_low++;

            DispMsg.dispMessage("キャリブレーション中だよ。腕を下げて待っていてね。キャリブレーションを終了するには左クリックを押してね。");
            //左クリックが押されたら腕を下げているときのキャリブレーションを終了する
            if (Input.GetMouseButtonDown(0))
            {
                calibrationlowflag = true;

                //Angle,PRそれぞれの平均値を算出して、それぞれの配列に格納する
                for (int i = 0; i < 3; i++)
                {
                    uhAngle_low[i] = uhAngle_low[i] / count_low;
                }
                for (int i = 0; i < 7; i++)
                {
                    uhPr_low[i] = uhPr_low[i] / count_low;
                }
                // Debug.Log(uhAngle_low[i]);

                nextstep = true;
            }
        }

        //腕を下げた状態のキャリブレーションが終了していたら
        if (nextstep == true)
        {
            DispMsg.dispMessage("次は腕を上げてね。腕を上げたらスペースキーを押してね。");
            //腕を上げてからスペースキーを押す
            if (Input.GetKeyDown(KeyCode.Space))
            {
                calibrationhighflag = true;
            }
            //スペースキーが押されたら
            if (calibrationhighflag == true)
            {

                //腕を上げた状態のキャリブレーションを開始する
                for (int i = 0; i < 3; i++)
                {
                    uhAngle_high[i] += unlimitedHand.UHAngle[i];
                }
                for (int i = 0; i < 7; i++)
                {
                    uhPr_high[i] += unlimitedHand.UHPR[i];
                }
                count_high++;

                DispMsg.dispMessage("キャリブレーション中だよ。腕を上げておいてね。キャリブレーションを終了するには右クリックを押してね。");
                //右クリックが押されたらキャリブレーションを終了する
                if (Input.GetMouseButtonDown(1))
                {
                    //値を足していくのをやめる
                    nextstep = false;

                    //Angle,PRそれぞれの平均値を算出して、それぞれの配列に格納する
                    for (int i = 0; i < 3; i++)
                    {
                        uhAngle_high[i] = uhAngle_high[i] / count_high;
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        uhPr_high[i] = uhPr_high[i] / count_high;
                    }

                    DispMsg.dispMessage("キャリブレーションはこれで終わりだよ。お疲れ様！");

                    //2.0秒待ってからシーンを切り替える
                    Invoke("SceneChange", 2.0f);
                }
            }
        }
    }

    void SceneChange()
    {
        Application.LoadLevel("SystemSeisaku");
    }
}
