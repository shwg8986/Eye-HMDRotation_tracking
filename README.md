# Eye-HMDRotation_tracking

#自分用メモ

HMD vive pro eyeを用いてunityVRゲーム中に眼球運動特性とHMDの回転座標（オイラー角とクォータニオン）を取得するスクリプト。

サンプリングレートを50FPSで固定するためにFixedUpdate関数を使用している。
注意点としてSteamVRを用いるアプリだとデフォでFPSが固定されてしまうため、FPSを自由に変更したい場合はSteamVR_Render.csの

>> Time.fixedDeltaTime = Time.timeScale / vr.hmd_DisplayFrequency;

をコメントアウトまたは削除しておく必要がある。
