using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DispMsg : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    public static int lengthMsg;            //表示する文字列の長さ
    public static bool flgDisp = false; //表示フラグ
    public static float waitTime = 0;

    // Update is called once per frame
    void Update()
    {

    }

    public static string dispMsg;

    public static void dispMessage(string msg)
    {

        dispMsg = msg;      //メッセージを代入
        lengthMsg = 0;      //０文字目にリセット
        flgDisp = true;     //表示
        waitTime = 0;
    }

    public GUIStyle msgWnd;

    void OnGUI()
    {

        //基準となる画面の幅
        const float screenWidth = 1136;

        //基準サイズに対するウインドウサイズと座標
        const float msgwWidth = 800;
        const float msgwHeight = 200;
        const float msgwPosX = (screenWidth - msgwWidth) / 2;
        const float msgwPosY = 390;

        //画面の幅から１ピクセルを算出
        float factorSize = Screen.width / screenWidth;

        float msgwX;
        float msgwY;
        float msgwW = msgwWidth * factorSize;
        float msgwH = msgwHeight * factorSize;

        //フォントのスタイル
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = (int)(30 * factorSize);

        //メッセージ表示
        if (flgDisp == true)
        {

            //ウィンドウ
            msgwX = msgwPosX * factorSize;
            msgwY = msgwPosY * factorSize;
            GUI.Box(new Rect(msgwX, msgwY, msgwW, msgwH), "ウインドウ", msgWnd);

            //メッセージ影用
            myStyle.normal.textColor = Color.black;

            msgwX = (msgwPosX + 22) * factorSize;
            msgwY = (msgwPosY + 22) * factorSize;
            GUI.Label(new Rect(msgwX, msgwY, msgwW, msgwH), dispMsg.Substring(0, lengthMsg), myStyle);

            //メッセージ
            myStyle.normal.textColor = Color.white;

            msgwX = (msgwPosX + 20) * factorSize;
            msgwY = (msgwPosY + 20) * factorSize;
            GUI.Label(new Rect(msgwX, msgwY, msgwW, msgwH), dispMsg.Substring(0, lengthMsg), myStyle);
        }

    }
}
