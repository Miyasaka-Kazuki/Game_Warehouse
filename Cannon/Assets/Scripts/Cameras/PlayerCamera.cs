using UnityEngine;
using System.Collections;

//カメラモード
public enum CameraMode {
    MainCamera,
    OtherCamera,
}

//プレイヤーを追いかけるカメラ
public class PlayerCamera : MonoBehaviour {
    public Transform target;
    public float distance = 6; //維持しようとするターゲットからの距離(XとZ)
    public float height = 3; //維持しようとするターゲットからの高さ(Y)
    public float angleWidthSpeed = 100; //1秒に変わる角度
    public float angleHeightSpeed = 100; //1秒に変わる角度

    private float angleWidth; //プレイヤーが横に回せる角度
    private float angleHeight; //プレイヤーが縦に回せる角度
    private Vector3 originForward; //回転の基準方向ベクトル

	//初期化関数
    void Awake() {
        angleWidth = 0;
        angleHeight = 0;
        originForward = transform.forward;
    }

	//更新関数
    public Vector3 Activate(ref Vector3 lookAtPos) {
        float hor = Input.GetAxis("CameraHorizontal");
        float ver = Input.GetAxis("CameraVertical");
        if (hor > 0) {
            angleWidth += angleWidthSpeed * Time.deltaTime;
        } else if (hor < 0) {
            angleWidth += -angleWidthSpeed * Time.deltaTime;
        }

        if (ver > 0) {
            angleHeight += angleHeightSpeed * Time.deltaTime;
        } else if (ver < 0) {
            angleHeight += -angleHeightSpeed * Time.deltaTime;
        }

        //角度制限
        angleHeight = Mathf.Clamp(angleHeight, -3f, 4f);

        //望ましいターゲットからの距離、高さ
        Vector3 wantedPos = target.position -
            Quaternion.Euler(0, angleWidth, 0) * originForward * distance;
        float wantedHeight = target.position.y + height + angleHeight; //目標高さ

        //新しい座標をセット
        wantedPos.y = wantedHeight;

        lookAtPos = target.position;

        return wantedPos;
    }
}

