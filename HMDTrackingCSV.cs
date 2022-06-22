using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using Valve.VR;


public class HMDTrackingCSV : MonoBehaviour
{

    //csv
    public string filename = "HMD_Tracking";
    StreamWriter sw;

    //HMDの位置座標格納用
    private Vector3 HMDPosition;
    //HMDの回転座標格納用（クォータニオン）
    private Quaternion HMDRotationQ;
    //HMDの回転座標格納用（オイラー角）
    private Vector3 HMDRotation;

    //時間格納
    float Time_;
    float FPS;

    // Start is called before the first frame update
    void Start()
    {
        sw = new StreamWriter(@"" + filename + ".csv", false);
        string[] s1 = { "HMDRotation.x", "HMDRotation.y", "HMDRotation.z", "HMDRotationQ.x","HMDRotationQ.y","HMDRotationQ.z","HMDRotationQ.w",
                        "time","fps" };

        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Time_ = UnityEngine.Time.time;

        //Head（ヘッドマウンドディスプレイ）の情報を一時保管-----------
        //位置座標を取得
        HMDPosition = InputTracking.GetLocalPosition(XRNode.Head);
        //回転座標をクォータニオンで値を受け取る
        HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head);
        //取得した値をクォータニオン → オイラー角に変換
        HMDRotation = HMDRotationQ.eulerAngles;
        //--------------------------------------------------------------

        FPS = 1 / UnityEngine.Time.deltaTime;

        string[] str = { "" + HMDRotation.x, "" + HMDRotation.y, "" +HMDRotation.z, "" + HMDRotationQ.x, "" + HMDRotationQ.y, "" +HMDRotationQ.z, "" + HMDRotationQ.w, "" +
                        Time_,""+ FPS};
        string str2 = string.Join(",", str);

        sw.WriteLine(str2);


        if (Input.GetKeyDown(KeyCode.F))
        {
            sw.Close();
        }

    }
}
