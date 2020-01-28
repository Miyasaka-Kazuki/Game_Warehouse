using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスを管理するクラス
public class BossDirector : CharacterDirector {
    private BaseEnemyTree behaviorTree; //ボスの行動管理クラス
    private BossStatus status; //ボスステータス
    private Transform[] footsPosition; //ボスの一部の位置
    private Vector3[] movePosition; //1フレームの偏移距離
    private Vector3[] lookAtPos; //視点方向
    private bool nUW;
    private Vector3[] freezePos;
    private bool isFrozen;

	//初期化関数
    void Start() {
        type = DirectorType.Character;
        behaviorTree = GetComponent<BaseEnemyTree>();
        status = GetComponent<BossStatus>();
        footsPosition = new Transform[4];
        SABoneColliderBuilder[] footCol = GetComponentsInChildren<SABoneColliderBuilder>();
        for (int i = 0; i < footCol.Length; i++)
            footsPosition[i] = footCol[i].transform;
        baseObserver = new List<BaseObserver>(10);

        freezePos = new Vector3[4];
        nUW = false;
        isFrozen = false;

        lookAtPos = new Vector3[4];
    }

	//更新関数
    public void Activate() {
        if (!isFrozen) {
            movePosition = behaviorTree.ActivateBoss(lookAtPos);

            //攻撃中や敵出し中じゃないなら上書き(追いかけるだけ)
            if (status.GetCanbePersuit()) {
                SetMovePosition();
                SetLookAt(lookAtPos);
            }
        }

        //ダメージを受けた時、赤く点滅させる
        if (status.GetDamaged()) {
            status.DamageExpression();
        }

        //向きの処理
        for (int i = 0; i < status.GetFoots().Length; i++) {
            status.GetFoots()[i].LookAt(lookAtPos[i]);
        } 
        //上書き
        if (isFrozen) {
            for (int i = 0; i < footsPosition.Length; i++) {
                footsPosition[i].position = freezePos[i];
            }
        } else {
            for (int i = 0; i < footsPosition.Length; i++) {
                footsPosition[i].position = movePosition[i];
            }
        }
    }

	//一時停止関数
    public void BossFreeze() {
        isFrozen = true;
        for (int i = 0; i < movePosition.Length; i++)
            freezePos[i] = footsPosition[i].position;

        Animator[] anim = status.GetComponentsInChildren<Animator>();
        for (int i = 0; i < anim.Length; i++)
            anim[i].speed = 0;
    }

	//一時停止解除関数
    public void ReleaseBossFreeze() {
        isFrozen = false;
        Animator[] anim = status.GetComponentsInChildren<Animator>();
        for (int i = 0; i < anim.Length; i++)
            anim[i].speed = 1;
    }


    private void SetMovePosition() {
    
        //ワールド座標のz軸が0度、時計回りが正、
        //Inspectorでは負でもプログラムでは正になる(-90→270)
        float[] wantedAngle = new float[4];
        wantedAngle[0] = status.GetFootCenter().rotation.eulerAngles.y - 45f;
        wantedAngle[1] = status.GetFootCenter().rotation.eulerAngles.y + 45f;
        wantedAngle[2] = status.GetFootCenter().rotation.eulerAngles.y - 90f;
        wantedAngle[3] = status.GetFootCenter().rotation.eulerAngles.y + 90f;

        //AngleからPositionに変換する
        //外積で正負を求める回る方向を決めればいい
        //(真逆の2方向のどちらがいいかを2択で決める場合は外積を使えばいい)
        Vector3[] wantedPos = new Vector3[4];
        for (int i = 0; i < wantedAngle.Length; i++) {
            wantedPos[i] = status.GetFootCenter().position;
            wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i] = Vector3.Lerp(status.GetFoots()[i].position, wantedPos[i], Time.fixedDeltaTime);
            movePosition[i] = wantedPos[i];
            Vector3 bodyPos = status.GetFootCenter().position;
            bodyPos.y += 3;
            status.GetBody().position = bodyPos;
        }
    }


    //現在の位置で自動で向きを変える
    private void SetLookAt(Vector3[] lookAtPos) {
        Vector3[] vecfromCenter = new Vector3[4];
        Transform[] foot = status.GetFoots();
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = status.GetFoots()[i].position - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAtPos[i] = foot[i].position + vecfromCenter[i];
        }

        Vector3 toPlayerFromCenter = status.GetPlayer().position - status.GetFootCenter().position;
        toPlayerFromCenter.y = 0;
        status.GetBody().rotation = Quaternion.Lerp(status.GetBody().rotation, 
            status.GetFootCenter().rotation, Time.fixedDeltaTime);
    }
}
