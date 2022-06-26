using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Valve.VR;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class EYETrackingCSV : MonoBehaviour
            {
                //csv
                public string filename = "EyeTracking";
                StreamWriter sw;

                //vive pro eyeから眼球情報呼び出し-----------------------------
                //呼び出したデータ格納用の関数
                EyeData eye;
                //-------------------------------------------

                //瞳孔位置--------------------
                //x,y軸
                //左の瞳孔位置格納用関数
                Vector2 LeftPupil;
                //左の瞳孔位置格納用関数
                Vector2 RightPupil;
                //------------------------------

                //瞳孔の直径-------------------------------
                //呼び出したデータ格納用の関数
                float LeftPupiltDiameter;
                float RightPupiltDiameter;
                //-------------------------------------------

                //まぶたの開き具合------------
                //左のまぶたの開き具合格納用関数
                float LeftBlink;
                //右のまぶたの開き具合格納用関数
                float RightBlink;
                //------------------------------


                //視線情報--------------------
                //origin：起点，direction：レイの方向　x,y,z軸
                //両目の視線格納変数
                Vector3 CombineGazeRayorigin;
                Vector3 CombineGazeRaydirection;
                //左目の視線格納変数
                Vector3 LeftGazeRayorigin;
                Vector3 LeftGazeRaydirection;
                //右目の視線格納変数
                Vector3 RightGazeRayorigin;
                Vector3 RightGazeRaydirection;
                //------------------------------

                //焦点情報--------------------
                //両目の焦点格納変数
                Ray CombineRay;
                
                public static FocusInfo CombineFocus;
                //レイの半径
                float CombineFocusradius;
                //レイの最大の長さ
                float CombineFocusmaxDistance;
                //オブジェクトを選択的に無視するために使用されるレイヤー
                int CombinefocusableLayer = 0;
                //------------------------------

                //輻輳距離(まだ実装されていないので常に値が0)
                float convergence_distance;

                //パッドのどこを触っているのかを取得するためのTrackPadという関数にSteamVR_Actions.default_TrackPadを固定
                [SerializeField]
                SteamVR_Action_Vector2 TrackPad = SteamVR_Actions.default_TrackPad;

                //結果の格納用Vector2型関数
                Vector2 posright;
                //どこのcontrollerの領域に触れていたかを格納
                string position;

                //時間とFPS格納
                float Time_;
                float FPS;

                void Start()
                {
                    //Application.targetFrameRate = 120;
                    sw = new StreamWriter(@"" + filename + ".csv", false);
                    string[] s1 = { "Left Pupil.x", "Left Pupil.y", "Right Pupil.x", "Right Pupil.y", " LeftPupiltDiameter", " RightPupiltDiameter" ,"Left Blink", "Right Blink",
                        "COMBINE GazeRayorigin.x", "COMBINE GazeRayorigin.y", "COMBINE GazeRayorigin.z", "COMBINE GazeRaydirection.x","COMBINE GazeRaydirection.y","COMBINE GazeRaydirection.z",
                        "Combine Focus Point.x","Combine Focus Point.y","Combine Focus Point.z","Controller Pad.x", "Controller Pad.y","Pad Position",
                        "time","fps" };

                    string s2 = string.Join(",", s1);
                    sw.WriteLine(s2);
                }

                void FixedUpdate()
                {
                    
                    //時間の取得
                    Time_ = UnityEngine.Time.time;


                    //取得呼び出し-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------


                    //瞳孔位置---------------------
                    //左の瞳孔位置を取得
                    if (SRanipal_Eye.GetPupilPosition(EyeIndex.LEFT, out LeftPupil))
                    {
                        //値が有効なら左の瞳孔位置を表示
                        //Debug.Log("Left Pupil" + LeftPupil.x + ", " + LeftPupil.y);
                    }
                    //右の瞳孔位置を取得
                    if (SRanipal_Eye.GetPupilPosition(EyeIndex.RIGHT, out RightPupil))
                    {
                        //値が有効なら右の瞳孔位置を表示
                        //Debug.Log("Right Pupil" + RightPupil.x + ", " + RightPupil.y);
                    }
                    //------------------------------

                    //瞳孔の直径-------------------------------
                    //左目の瞳孔の直径が妥当ならば取得　目をつぶるとFalse
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        LeftPupiltDiameter = eye.verbose_data.left.pupil_diameter_mm;
                        //Debug.Log("Left Pupilt Diameter：" + LeftPupiltDiameter);
                    }

                    ////右目の瞳孔の直径が妥当ならば取得　目をつぶるとFalse
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        RightPupiltDiameter = eye.verbose_data.right.pupil_diameter_mm;
                        //Debug.Log("Right Pupilt Diameter：" + RightPupiltDiameter);
                    }
                    //-------------------------------------------



                    //まぶたの開き具合
                    //左のまぶたの開き具合を取得
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftBlink, eye))
                    {
                        //値が有効なら左のまぶたの開き具合を表示
                        //Debug.Log("Left Blink" + LeftBlink);
                    }
                    //右のまぶたの開き具合を取得
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out RightBlink, eye))
                    {
                        //値が有効なら右のまぶたの開き具合を表示
                        //Debug.Log("Right Blink" + RightBlink);
                    }
                    //------------------------------



                    //視線情報--------------------
                    //両目の視線情報が有効なら視線情報を表示origin：起点，direction：レイの方向
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out CombineGazeRayorigin, out CombineGazeRaydirection, eye))
                    {
                        //Debug.Log("COMBINE GazeRayorigin" + CombineGazeRayorigin.x + ", " + CombineGazeRayorigin.y + ", " + CombineGazeRayorigin.z);
                        //Debug.Log("COMBINE GazeRaydirection" + CombineGazeRaydirection.x + ", " + CombineGazeRaydirection.y + ", " + CombineGazeRaydirection.z);
                    }

                    //左目の視線情報が有効なら視線情報を表示origin：起点，direction：レイの方向
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out LeftGazeRayorigin, out LeftGazeRaydirection, eye))
                    {
                        //Debug.Log("Left GazeRayorigin" + LeftGazeRayorigin.x + ", " + LeftGazeRayorigin.y + ", " + LeftGazeRayorigin.z);
                        //Debug.Log("Left GazeRaydirection" + LeftGazeRaydirection.x + ", " + LeftGazeRaydirection.y + ", " + LeftGazeRaydirection.z);
                    }


                    //右目の視線情報が有効なら視線情報を表示origin：起点，direction：レイの方向
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out RightGazeRayorigin, out RightGazeRaydirection, eye))
                    {
                        //Debug.Log("Right GazeRayorigin" + RightGazeRayorigin.x + ", " + RightGazeRayorigin.y + ", " + RightGazeRayorigin.z);
                        //Debug.Log("Right GazeRaydirection" + RightGazeRaydirection.x + ", " + RightGazeRaydirection.y + ", " + RightGazeRaydirection.z);
                    }
                    //------------------------------

                    //焦点情報--------------------
                    //radius, maxDistance，CombinefocusableLayerは省略可
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                        //Debug.Log("Combine Focus Point" + CombineFocus.point.x + ", " + CombineFocus.point.y + ", " + CombineFocus.point.z);
                    }
                    //------------------------------


                    //輻輳距離（取得できない）
                    convergence_distance = eye.verbose_data.combined.convergence_distance_mm;


                    //ViveコントローラTrackPadの触れている位置を取得する
                    
                    //結果をGetLastAxisで取得してposrightに格納
                    //SteamVR_Input_Sources.機器名（ここは右コントローラ）
                    posright = TrackPad.GetLastAxis(SteamVR_Input_Sources.RightHand);
                    //posleftの中身を確認

                    if (posright.x == 0 && posright.y == 0)
                    {
                        position = "None";
                    }
                    else if(posright.y / posright.x > 1 || posright.y / posright.x < -1)
                    {
                        if(posright.y > 0)
                        {
                            position = "Top";
                        }
                        else
                        {
                            position = "Bottom";
                        }

                    }
                    else
                    {
                        if (posright.x > 0)
                        {
                            position = "Right";
                        }
                        else
                        {
                            position = "Left";
                        }
                    }

                    //Debug.Log("position : " + position);
                    
                    //FPSをlogで確認する
                    FPS = 1 / UnityEngine.Time.deltaTime;
                    Debug.Log("FPS : " + FPS);

                    string[] str = { "" + LeftPupil.x, "" + LeftPupil.y, "" + RightPupil.x, "" + RightPupil.y, "" +  LeftPupiltDiameter, "" +  RightPupiltDiameter, ""+ LeftBlink, "" + RightBlink,"" +
                        CombineGazeRayorigin.x, "" + CombineGazeRayorigin.y,"" + CombineGazeRayorigin.z,""+ CombineGazeRaydirection.x ,"" + CombineGazeRaydirection.y,"" + CombineGazeRaydirection.z,""+
                        CombineFocus.point.x,"" + CombineFocus.point.y,"" + CombineFocus.point.z,""+ posright.x,""+posright.y,""+position,""+
                        Time_,""+ FPS};
                    string str2 = string.Join(",", str);

                    sw.WriteLine(str2);

                    //ファイルを閉じる
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        sw.Close();
                    }
                }
            }
        }
    }
}
