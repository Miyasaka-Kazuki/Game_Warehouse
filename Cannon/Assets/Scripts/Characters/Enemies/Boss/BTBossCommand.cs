using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//HP半分下回った時のモーションコマンド
public class BTAngryMotionBossCommand : BaseBTCommand {
    private EnemyTreeOfFirstBoss eneTree;
    private Animator[] footsAnim;
    private Transform[] curPos;
    private Vector3[] wantedPos;
    private BossStatus status;
    private Transform rootPos;
    private float rotationTimer;
    private float rotationTime = 360;
    private int switchCount;
    private InkObserver inkOb;

	//初期化関数
    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<EnemyTreeOfFirstBoss>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        rotationTimer = 0;
        switchCount = 0;
        inkOb = GetComponent<InkObserver>();
    }

    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        curPos = (Transform[])status.GetFoots().Clone();
        switchCount = 0;
        status.SetMuteki(true);
        status.SetIsInMotion(true);

        float[] wantedAngle = new float[4];
        wantedAngle[0] = rootPos.rotation.eulerAngles.y - 70f;
        wantedAngle[1] = rootPos.rotation.eulerAngles.y + 70f;
        wantedAngle[2] = rootPos.rotation.eulerAngles.y - 90f;
        wantedAngle[3] = rootPos.rotation.eulerAngles.y + 90f;

        //AngleからPositionに変換する
        for (int i = 0; i < wantedAngle.Length; i++) {
            wantedPos[i] = status.GetFootCenter().position;
            wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
        }
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Crab", false);
            footsAnim[i].SetBool("Cleave", false);
            footsAnim[i].SetBool("Rotate", false);
            footsAnim[i].SetBool("Stab", false);
            footsAnim[i].SetBool("Ready", false);
            footsAnim[i].SetBool("Maul", false);
            footsAnim[i].SetBool("Return", false);
            footsAnim[i].SetTrigger("Angry");
        }
    }

    public override BaseBTNode.NodeStatus Activate() {
        switch (switchCount) {
            case 0:
                //lerpしてPositionが一致するまで
                int wantCount = 0;
                for (int i = 0; i < wantedPos.Length; i++) {
                    curPos[i].position = Vector3.Lerp(curPos[i].position, wantedPos[i], Time.fixedDeltaTime * 5);
                    if (Mathf.Abs(curPos[i].position.x - wantedPos[i].x) <= 1f)
                        wantCount++;
                }
                if (wantCount == curPos.Length)
                    switchCount++;
                SetMovePosition();
                SetLookAt();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 1:
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("Angry"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length) {
                    switchCount++;
                }
                SetMovePosition();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2:
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("AngryEnd", true);
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("AngryEnd"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length) {
                    switchCount++;
                }
                SetMovePosition();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3:
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("AngryEnd", false);
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("AngryEnd"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length) {
                    switchCount++;
                }
                SetMovePosition();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 4:
                //全て完了
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        //ここに来るのはありえない
        return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }

    public override void Exit() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Rotate", false);
            footsAnim[i].SetBool("Return", false);
        }
    }

    public override void ExitForSetFailuer() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Rotate", false);
            footsAnim[i].SetBool("Return", false);
        }
    }

    public void SetMovePosition() {
        Vector3[] vecPos = new Vector3[4];
        for (int i = 0; i < curPos.Length; i++)
            vecPos[i] = curPos[i].position;
        eneTree.SetMovePosition((Vector3[])vecPos.Clone());
    }
    public void SetLookAt() {
        Vector3[] vecfromCenter = new Vector3[4];
        Transform[] foot = status.GetFoots();
        Vector3[] lookAt = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = curPos[i].position - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAt[i] = foot[i].position + vecfromCenter[i];
        }

        eneTree.SetLookAtPos((Vector3[])lookAt.Clone());
    }
}

//プレイヤーを強制操作させるコマンド
public class BTPlayerObserverBossCommand : BaseBTCommand {
    private EnemyTreeOfFirstBoss eneTree;
    private Animator[] footsAnim;
    private Transform[] curPos;
    private Vector3[] wantedPos;
    private BossStatus status;
    private Transform rootPos;
    private float rotationTimer;
    private float rotationTime = 360;
    private int switchCount;
    private InkObserver inkOb;

    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<EnemyTreeOfFirstBoss>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        rotationTimer = 0;
        switchCount = 0;
        inkOb = GetComponent<InkObserver>();
    }

    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        curPos = (Transform[])status.GetFoots().Clone();
        switchCount = 0;

        float[] wantedAngle = new float[4];
        wantedAngle[0] = rootPos.rotation.eulerAngles.y - 70f;
        wantedAngle[1] = rootPos.rotation.eulerAngles.y + 70f;
        wantedAngle[2] = rootPos.rotation.eulerAngles.y - 90f;
        wantedAngle[3] = rootPos.rotation.eulerAngles.y + 90f;

        //AngleからPositionに変換する
        for (int i = 0; i < wantedAngle.Length; i++) {
            wantedPos[i] = status.GetFootCenter().position;
            wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
        }
        for (int i = 0; i < footsAnim.Length; i++)
            footsAnim[i].SetBool("Ready", true);

    }

    public override BaseBTNode.NodeStatus Activate() {
        switch (switchCount) {
            case 0:
                //lerpしてPositionが一致するまで
                int wantCount = 0;
                for (int i = 0; i < wantedPos.Length; i++) {
                    curPos[i].position = Vector3.Lerp(curPos[i].position, wantedPos[i], Time.fixedDeltaTime * 5);
                    if (Mathf.Abs(curPos[i].position.x - wantedPos[i].x) <= 0.1f)
                        wantCount++;
                }
                if (wantCount == curPos.Length)
                    switchCount++;
                SetMovePosition();
                SetLookAt();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 1:
                for (int i = 0; i < footsAnim.Length; i++) {
                    if (footsAnim[i].GetCurrentAnimatorStateInfo(0).IsName("Ready")) {
                        footsAnim[i].SetBool("Ready", false);
                        footsAnim[i].SetBool("Maul", true);
                    }
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("MomentMaul"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length) {
                    inkOb.AddCharaOb(); //勝手に解除される
                    switchCount++;
                }
                SetMovePosition();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2:
                //回転（やるまえに待機する？）
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Maul", false);
                    footsAnim[i].SetBool("Return", true);
                }
                float diff = 2;
                for (int i = 0; i < curPos.Length; i++) {
                    //                    Vector3 vecPos = curPos[i].position;
                    Vector3 vecPos = rootPos.position;
                    float nowAngle = status.GetFoots()[i].rotation.eulerAngles.y;
                    vecPos.z += status.GetDistanceFromCenter() * Mathf.Cos((nowAngle + diff) * Mathf.Deg2Rad);
                    vecPos.x += status.GetDistanceFromCenter() * Mathf.Sin((nowAngle + diff) * Mathf.Deg2Rad);
                    curPos[i].position = vecPos;
                }
                rotationTimer += diff; //一回転
                if (rotationTimer > rotationTime) {
                    rotationTimer = 0;
                    switchCount++;
                }

                //目の前にインクを生成する
                //RayCastはマスクに指定したものを無視する
                int mask = LayerMask.NameToLayer("Player");
                mask += LayerMask.NameToLayer("Enemy");
                RaycastHit hit;
                bool boolHit = Physics.Raycast(status.GetBody().position - 5 * Vector3.up, status.GetBody().forward,
                    out hit, 100, mask);
                if (boolHit) {
                    MeshRenderer renderer = hit.transform.gameObject.GetComponent<MeshRenderer>();
                    if (renderer != null) {
                        Material floorCol = renderer.material;
                        if (floorCol != status.GetInkMaterial()) {
                            hit.transform.gameObject.GetComponent<MeshRenderer>().material = status.GetInkMaterial();
                        }
                    }
                }

                SetMovePosition();
                SetLookAt();
                status.GetBody().Rotate(0, diff, 0);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3:
                //Returnモーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Return", false);
                }
                SetMovePosition();
                switchCount++;
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 4:
                //全て完了
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        //ここに来るのはありえない
        return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }

    public override void Exit() {
        status.SetMuteki(false);
        //        status.SetIsInMotion(false);
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
        eneTree.SetIsHealthHalfDownWithTrue();

        status.GetInkFloor().SetActive(true);
    }

    public override void ExitForSetFailuer() {
        status.SetMuteki(false);
        //        status.SetIsInMotion(false);
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Rotate", false);
            footsAnim[i].SetBool("Return", false);
        }
        eneTree.SetIsHealthHalfDownWithTrue();

        status.GetInkFloor().SetActive(true);
    }

    public void SetMovePosition() {
        Vector3[] vecPos = new Vector3[4];
        for (int i = 0; i < curPos.Length; i++)
            vecPos[i] = curPos[i].position;
        eneTree.SetMovePosition((Vector3[])vecPos.Clone());
    }
    public void SetLookAt() {
        Vector3[] vecfromCenter = new Vector3[4];
        Transform[] foot = status.GetFoots();
        Vector3[] lookAtPos = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = curPos[i].position - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAtPos[i] = foot[i].position + vecfromCenter[i];
        }

        eneTree.SetLookAtPos((Vector3[])lookAtPos.Clone());
    }
}


public class BTInkSpewBossCommand : BaseBTCommand {
    public override BaseBTNode.NodeStatus Activate() {
        return BaseBTNode.NodeStatus.STATUS_SUCCESS;
    }
}
public class BTInstanceEnemyBossCommand : BaseBTCommand {
    public override BaseBTNode.NodeStatus Activate() {
        return BaseBTNode.NodeStatus.STATUS_SUCCESS;
    }
}

//回転攻撃コマンド
public class BTRotationAttackBossCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private Animator[] footsAnim;
    private Transform[] curPos;
    private Vector3[] wantedPos;
    private BossStatus status;
    private Transform rootPos;
    private float rotationTimer;
    private float rotationTime = 5;
    private int switchCount;

    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<BaseEnemyTree>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        rotationTimer = 0;
        switchCount = 0;
    }

    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        curPos = (Transform[])status.GetFoots().Clone();
        switchCount = 0;

        float[] wantedAngle = new float[4];
        wantedAngle[0] = rootPos.rotation.eulerAngles.y - 45f;
        wantedAngle[1] = rootPos.rotation.eulerAngles.y + 45f;
        wantedAngle[2] = rootPos.rotation.eulerAngles.y - 135f;
        wantedAngle[3] = rootPos.rotation.eulerAngles.y + 135f;

        //AngleからPositionに変換する
        for (int i = 0; i < wantedAngle.Length; i++) {
            wantedPos[i] = status.GetFootCenter().position;
            wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
        }
    }

    public override BaseBTNode.NodeStatus Activate() {
        switch (switchCount) {
            case 0:
                //lerpしてPositionが一致するまで
                int wantCount = 0;
                for (int i = 0; i < wantedPos.Length; i++) {
                    curPos[i].position = Vector3.Lerp(curPos[i].position, wantedPos[i], Time.fixedDeltaTime);
                    if (Mathf.Abs(curPos[i].position.x - wantedPos[i].x) <= 0.1f)
                        wantCount++;
                }
                if (wantCount == curPos.Length)
                    switchCount++;
                SetMovePosition();
                SetLookAt();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 1:
                //Rotateモーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++)
                    footsAnim[i].SetBool("Rotate", true);
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("RotationAttack"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length)
                    switchCount++;
                SetMovePosition();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2:
                //回転（やるまえに待機する？）
                float diff = 1.5f;
                if (status.GetHealth() < status.GetMaxHealth() / 2.0f)
                    diff = 2.0f;

                for (int i = 0; i < curPos.Length; i++) {
                    //                    Vector3 vecPos = curPos[i].position;
                    Vector3 vecPos = rootPos.position;
                    float nowAngle = status.GetFoots()[i].rotation.eulerAngles.y;
                    vecPos.z += status.GetDistanceFromCenter() * Mathf.Cos((nowAngle + diff) * Mathf.Deg2Rad);
                    vecPos.x += status.GetDistanceFromCenter() * Mathf.Sin((nowAngle + diff) * Mathf.Deg2Rad);
                    curPos[i].position = vecPos;
                }
                rotationTimer += Time.fixedDeltaTime;
                if (rotationTimer > rotationTime) {
                    rotationTimer = 0;
                    switchCount++;
                }
                SetMovePosition();
                SetLookAt();
                status.GetBody().Rotate(0, diff, 0);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3:
                //Returnモーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Rotate", false);
                    footsAnim[i].SetBool("Return", true);
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("ReturnBaseFoot"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length)
                    switchCount++;
                SetMovePosition();
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 4:
                //全て完了
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        //ここに来るのはありえない
        return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }

    public override void Exit() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Rotate", false);
            footsAnim[i].SetBool("Return", false);
        }
    }

    public override void ExitForSetFailuer() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Rotate", false);
            footsAnim[i].SetBool("Return", false);
        }
    }

    public void SetMovePosition() {
        Vector3[] vecPos = new Vector3[4];
        for (int i = 0; i < curPos.Length; i++)
            vecPos[i] = curPos[i].position;
        eneTree.SetMovePosition((Vector3[])vecPos.Clone());
    }
    public void SetLookAt() {
        Vector3[] vecfromCenter = new Vector3[4];
        Transform[] foot = status.GetFoots();
        Vector3[] lookAt = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = curPos[i].position - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAt[i] = foot[i].position + vecfromCenter[i];
        }

        eneTree.SetLookAtPos((Vector3[])lookAt.Clone());
    }
}

//挟み込み攻撃コマンド
public class BTCrabScissorsBossCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private Animator[] footsAnim;
    private Vector3[] wantedPos;
    private bool[] endCrab;
    private BossStatus status;
    private Transform rootPos;
    private float timer;
    private float time = 2;
    private int switchCount;
    private Vector3 goalVector;
    private Vector3[] endVectors; //最終的な位置

    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<BaseEnemyTree>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        timer = 0;
        switchCount = 0;
        endVectors = new Vector3[4];
        endCrab = new bool[4];
        for (int i = 0; i < endCrab.Length; i++)
            endCrab[i] = false;
    }

    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        goalVector = status.GetPlayer().position - status.GetFootCenter().position;
        goalVector.y = 0;
        goalVector = goalVector.normalized;
        switchCount = 0;

        float[] wantedAngle = new float[4];
        wantedAngle[0] = rootPos.rotation.eulerAngles.y - 135f;
        wantedAngle[1] = rootPos.rotation.eulerAngles.y + 135f;
        wantedAngle[2] = rootPos.rotation.eulerAngles.y - 135f;
        wantedAngle[3] = rootPos.rotation.eulerAngles.y + 135f;

        //AngleからPositionに変換する
        for (int i = 0; i < wantedAngle.Length; i++) {
            wantedPos[i] = status.GetFootCenter().position;
            wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
        }
        for (int i = 0; i < endCrab.Length; i++)
            endCrab[i] = false;
    }

    public override BaseBTNode.NodeStatus Activate() {
        //135度と-135度にlerpする
        //降ろす
        //回す
        //2秒待つ
        //上げる
        switch (switchCount) {
            case 0: //プレイヤーから離れる
                Vector3[] endPos = new Vector3[4];
                int wantCount = 0;
                for (int i = 0; i < wantedPos.Length; i++) {
                    endPos[i] = Vector3.Lerp(status.GetFoots()[i].position, wantedPos[i], Time.fixedDeltaTime * 3);
                    if (Mathf.Abs(endPos[i].x - wantedPos[i].x) <= 0.1f)
                        wantCount++;
                }
                if (wantCount == endPos.Length) switchCount++;
                // eneTree.SetMovePosition((Vector3[])endPos.Clone());
                SetMovePosition(endPos);
                SetLookAt(endPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 1: //降ろす
                    //少し上にあげてずらす
                Vector3[] endPos2 = new Vector3[4];
                for (int i = 0; i < wantedPos.Length; i++) {
                    endPos2[i] = wantedPos[i];
                    if (i < 2)
                        endPos2[i].y = Mathf.Lerp(status.GetFoots()[i].position.y, status.GetFootCenter().position.y + 2, Time.fixedDeltaTime);
                    else
                        endPos2[i].y = Mathf.Lerp(status.GetFoots()[i].position.y, status.GetFootCenter().position.y + 4, Time.fixedDeltaTime);
                }

                //モーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Crab", true);
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("CrabScissors"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length) {
                    switchCount++;
                    endPos2.CopyTo(wantedPos, 0);
                }
                //                eneTree.SetMovePosition((Vector3[])wantedPos.Clone());
                SetMovePosition(endPos2);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2: //回す（やるまえに待機する？）
                wantCount = 0;
                Vector3[] endPos3 = new Vector3[4];
                for (int i = 0; i < wantedPos.Length; i++) {
                    if (endCrab[i]) continue; //着いたものは動かさない
                    float nowAngle = status.GetFoots()[i].rotation.eulerAngles.y;
                    float diff = 0.5f;
                    if (status.GetHealth() < status.GetMaxHealth() / 2)
                        diff = 0.8f;
                    if (i % 2 != 0) diff *= -1;
                    endPos3[i] = rootPos.position;
                    endPos3[i].y = status.GetFoots()[i].position.y;
                    endPos3[i].z += status.GetDistanceFromCenter() * Mathf.Cos((nowAngle + diff) * Mathf.Deg2Rad);
                    endPos3[i].x += status.GetDistanceFromCenter() * Mathf.Sin((nowAngle + diff) * Mathf.Deg2Rad);
                    Vector3 vecFromCenter = endPos3[i] - status.GetFootCenter().position;
                    vecFromCenter.y = 0;
                    float nowRadian = Mathf.Acos(Vector3.Dot(vecFromCenter.normalized, goalVector));
                    if (nowRadian * Mathf.Rad2Deg <= 0.5f) {
                        endCrab[i] = true;
                        endVectors[i] = endPos3[i];
                    }
                }
                for (int i = 0; i < endCrab.Length; i++) {
                    if (endCrab[i]) wantCount++;
                }
                if (wantCount == endCrab.Length) { //全部入ってるなら
                    switchCount++;
                    endVectors.CopyTo(wantedPos, 0);
                }
                SetMovePosition(endPos3);
                SetLookAt(endPos3);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3: //待つ
                timer += Time.fixedDeltaTime;
                if (timer > time) {
                    timer = 0;
                    switchCount++;
                }
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 4: //上げる
                //モーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Crab", false);
                    footsAnim[i].SetBool("Return", true);
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 0.6f && info.IsName("ReturnBaseFoot"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length)
                    switchCount++;
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 5:
                //全て完了
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        //ここに来るのはありえない
        return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }

    public override void Exit() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }

    public override void ExitForSetFailuer() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }

    public void SetMovePosition(Vector3[] endPos) {
        eneTree.SetMovePosition((Vector3[])endPos.Clone());
    }
    public void SetLookAt(Vector3[] endPos) {
        //方向指定
        Vector3[] vecfromCenter = new Vector3[4];
        Vector3[] lookAt = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = endPos[i] - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAt[i] = status.GetFoots()[i].position + vecfromCenter[i];
        }

        eneTree.SetLookAtPos((Vector3[])lookAt.Clone());
    }
}

//振り下ろし攻撃コマンド
public class BTMomentMaulBossCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private Animator[] footsAnim;
    private Vector3[] wantedPos;
    private BossStatus status;
    private Transform rootPos;
    private float persuitTimer;
    private float persuitTime = 5;
    private int switchCount;
    private float readyTimer;
    private float readyTime = 0.5f;

	//初期化関数
    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<BaseEnemyTree>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        persuitTimer = 0;
        switchCount = 0;
    }


    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        switchCount = 0;

        for (int i = 0; i < footsAnim.Length; i++)
            footsAnim[i].SetBool("Ready", true);
    }


    public override BaseBTNode.NodeStatus Activate() {
        //3秒間追いかける
        //降ろす
        //3秒待つ
        //上げる
        switch (switchCount) {

            case 0:

                float[] wantedAngle = new float[4];
                wantedAngle[0] = rootPos.rotation.eulerAngles.y - 0f;
                wantedAngle[1] = rootPos.rotation.eulerAngles.y + 20f;
                wantedAngle[2] = rootPos.rotation.eulerAngles.y - 20f;
                wantedAngle[3] = rootPos.rotation.eulerAngles.y + 40f;

                //AngleからPositionに変換する
                for (int i = 0; i < wantedAngle.Length; i++) {
                    wantedPos[i] = status.GetFootCenter().position;
                    wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
                    wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
                    wantedPos[i] = Vector3.Lerp(status.GetFoots()[i].position, wantedPos[i], Time.fixedDeltaTime);
                }
                SetMovePosition(wantedPos);

                //方向指定
                Vector3[] vecfromCenter = new Vector3[4];
                Vector3[] lookAt = new Vector3[4];
                for (int i = 0; i < vecfromCenter.Length; i++) {
                    vecfromCenter[i] = wantedPos[i] - status.GetFootCenter().position;
                    vecfromCenter[i].y = 0;
                    lookAt[i] = status.GetFoots()[i].position + vecfromCenter[i];
                }

                eneTree.SetLookAtPos((Vector3[])lookAt.Clone());
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);

                persuitTimer += Time.fixedDeltaTime;
                if (persuitTimer > persuitTime) {
                    persuitTimer = 0;
                    switchCount++;
                }

                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 1:

                readyTimer += Time.fixedDeltaTime;
                if (readyTimer > readyTime)
                    switchCount++;
                SetMovePosition(wantedPos);

                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2:

                //モーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Ready", false);
                    footsAnim[i].SetBool("Maul", true);
                }

                int wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("MomentMaul"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length)
                    switchCount++;

                SetMovePosition(wantedPos);

                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3:

                persuitTimer += Time.fixedDeltaTime;
                if (persuitTimer > persuitTime) {
                    persuitTimer = 0;
                    switchCount++;
                }
                
                SetMovePosition(wantedPos);

                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 4:
                
                //モーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Maul", false);
                    footsAnim[i].SetBool("Return", true);
                }

                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 0.6f && info.IsName("ReturnBaseFoot"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length)
                    switchCount++;
                
                SetMovePosition(wantedPos);

                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 5:

                //全て完了
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;

        }

        //ここに来るのはありえない
        return BaseBTNode.NodeStatus.STATUS_FAILURE;

    }


    public override void Exit() {

        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }


    public override void ExitForSetFailuer() {

        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }


    public void SetMovePosition(Vector3[] endPos) {

        eneTree.SetMovePosition((Vector3[])endPos.Clone());
    }
}
public class BTSeparateContinuousAttackBossCommand : BaseBTCommand {
    public override BaseBTNode.NodeStatus Activate() {
        throw new NotImplementedException();
    }
}

//火の玉攻撃コマンド
public class BTFireBallPreventingEscapeBossCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private Animator[] footsAnim;
    private Vector3[] wantedPos;
    private bool[] endCrab;
    private BossStatus status;
    private Transform rootPos;
    private float timer;
    private float time = 1;
    private int switchCount;
    private Vector3 goalVector;
    private int bulletCount;
    private int bullerMaxCount;
    private WeaponStatus weaponStatus;

	//初期化関数
    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<BaseEnemyTree>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        timer = 0;
        switchCount = 0;
        endCrab = new bool[4];
        for (int i = 0; i < endCrab.Length; i++)
            endCrab[i] = false;
        bulletCount = 0;
        weaponStatus = GetComponent<WeaponStatus>();
    }


    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        goalVector = status.GetPlayer().position - status.GetFootCenter().position;
        goalVector.y = 0;
        goalVector = goalVector.normalized;
        switchCount = 0;
        bulletCount = 0;
        bullerMaxCount = 5;
        if (status.GetHealth() < status.GetMaxHealth() / 2)
            bullerMaxCount = 8;

        float[] wantedAngle = new float[4];
        wantedAngle[0] = rootPos.rotation.eulerAngles.y - 60f;
        wantedAngle[1] = rootPos.rotation.eulerAngles.y + 60f;
        wantedAngle[2] = rootPos.rotation.eulerAngles.y - 60f;
        wantedAngle[3] = rootPos.rotation.eulerAngles.y + 60f;

        //AngleからPositionに変換する
        for (int i = 0; i < wantedAngle.Length; i++) {
            wantedPos[i] = status.GetFootCenter().position;
            wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
        }
        for (int i = 0; i < endCrab.Length; i++)
            endCrab[i] = false;
    }
		
    public override BaseBTNode.NodeStatus Activate() {
        //60度と-60度にlerpする
        //降ろす
        //1秒待つ
        //炎弾３つ(間は1秒)
        //1秒待つ
        //上げる
        switch (switchCount) {

            case 0: //プレイヤーから離れる

                Vector3[] endPos = new Vector3[4];
                int wantCount = 0;
                for (int i = 0; i < wantedPos.Length; i++) {
                    endPos[i] = Vector3.Lerp(status.GetFoots()[i].position, wantedPos[i], Time.fixedDeltaTime * 2);
                    if (Mathf.Abs(endPos[i].x - wantedPos[i].x) <= 0.1f)
                        wantCount++;
                }
                if (wantCount == endPos.Length)
                    switchCount++;

                SetMovePosition(endPos);
                SetLookAt(endPos);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);

                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 1: //降ろす
                    //少し上にあげてずらす
                Vector3[] endPos2 = new Vector3[4];
                for (int i = 0; i < wantedPos.Length; i++) {
                    endPos2[i] = wantedPos[i];
                    if (i < 2)
                        endPos2[i].y = Mathf.Lerp(status.GetFoots()[i].position.y, status.GetFootCenter().position.y + 1, Time.fixedDeltaTime);
                    else
                        endPos2[i].y = Mathf.Lerp(status.GetFoots()[i].position.y, status.GetFootCenter().position.y + 2, Time.fixedDeltaTime);
                }

                //モーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Crab", true);
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("CrabScissors"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length) {
                    switchCount++;
                    endPos2.CopyTo(wantedPos, 0);
                }
                SetMovePosition(endPos2);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2: //待つ
                timer += Time.fixedDeltaTime;
                if (timer > time) {
                    timer = (1 << 30);
                    switchCount++;
                }
                SetMovePosition(wantedPos);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3: //炎弾３回
                timer += Time.fixedDeltaTime;
                if (timer > time) {
                    timer = 0;
                    bulletCount++;

                    //炎弾を放つ
                    GameObject fireBall = Instantiate(weaponStatus.enemyBulletPrefab, weaponStatus.weaponInstancePosition.position,
                        Quaternion.identity);
                    fireBall.transform.LookAt(status.GetPlayer().position);
                    if (status.GetHealth() < status.GetMaxHealth() / 2)
                        fireBall.GetComponent<FireBall>().speed = 12;
                }
                if (bulletCount >= bullerMaxCount)
                    switchCount++;
                SetMovePosition(wantedPos);
                SetLookAt(wantedPos);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 4: //待つ
                timer += Time.fixedDeltaTime;
                if (timer > time) {
                    timer = 0;
                    switchCount++;
                }
                SetMovePosition(wantedPos);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 5: //上げる
                //モーションが完了するまで
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetBool("Crab", false);
                    footsAnim[i].SetBool("Return", true);
                }
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 0.6f && info.IsName("ReturnBaseFoot"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length)
                    switchCount++;
                SetMovePosition(wantedPos);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 6:
                //全て完了
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        //ここに来るのはありえない
        return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }

    public override void Exit() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }

    public override void ExitForSetFailuer() {
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }


    public void SetMovePosition(Vector3[] endPos) {
        eneTree.SetMovePosition((Vector3[])endPos.Clone());
    }


    public void SetLookAt(Vector3[] endPos) {

        //方向指定
        Vector3[] vecfromCenter = new Vector3[4];
        Vector3[] lookAt = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = endPos[i] - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAt[i] = status.GetFoots()[i].position + vecfromCenter[i];
        }

        eneTree.SetLookAtPos((Vector3[])lookAt.Clone());
    }
}

//死亡モーションコマンド
public class BTDyingMotionBossCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private Animator[] footsAnim;
    private Vector3[] wantedPos;
    private BossStatus status;
    private Transform rootPos;
    private int switchCount;

	//初期化関数
    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<BaseEnemyTree>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        switchCount = 0;
    }

    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        switchCount = 0;

        float[] wantedAngle = new float[4];
        wantedAngle[0] = rootPos.rotation.eulerAngles.y - 70f;
        wantedAngle[1] = rootPos.rotation.eulerAngles.y + 70f;
        wantedAngle[2] = rootPos.rotation.eulerAngles.y - 90f;
        wantedAngle[3] = rootPos.rotation.eulerAngles.y + 90f;

        //AngleからPositionに変換する
        for (int i = 0; i < wantedAngle.Length; i++) {
            wantedPos[i] = status.GetFootCenter().position;
            wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
            wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
        }
    }

    public override BaseBTNode.NodeStatus Activate() {
        switch (switchCount) {
            case 0: //プレイヤーから離れる
                Vector3[] endPos = new Vector3[4];
                int wantCount = 0;
                for (int i = 0; i < wantedPos.Length; i++) {
                    endPos[i] = Vector3.Lerp(status.GetFoots()[i].position, wantedPos[i], Time.fixedDeltaTime * 3);
                    if (Mathf.Abs(endPos[i].x - wantedPos[i].x) <= 0.1f)
                        wantCount++;
                }
                if (wantCount == endPos.Length) switchCount++;
 
                SetMovePosition(endPos);
                SetLookAt(endPos);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;
            case 1:
                for (int i = 0; i < footsAnim.Length; i++) {
                    footsAnim[i].SetTrigger("Dead");
                }
                switchCount++;
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2:
                wantCount = 0;
                for (int i = 0; i < footsAnim.Length; i++) {
                    AnimatorStateInfo info = footsAnim[i].GetCurrentAnimatorStateInfo(0);
                    if (info.normalizedTime >= 1.0f && info.IsName("Dead"))
                        wantCount++;
                }
                if (wantCount == footsAnim.Length) {
                    switchCount++;
                }
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3:
                float diff = 1;
                for (int i = 0; i < wantedPos.Length; i++)
                    wantedPos[i].y -= diff;
                Vector3 nextVec = status.GetBody().transform.position;
                nextVec.y -= diff;
                status.GetBody().transform.position = nextVec;
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;
        }
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }

    public override void Exit() {
        status.SetCanBePersuit(true);
    }

    public override void ExitForSetFailuer() {
        status.SetCanBePersuit(true);
    }

    public void SetMovePosition(Vector3[] endPos) {
        eneTree.SetMovePosition((Vector3[])endPos.Clone());
    }

    public void SetLookAt(Vector3[] endPos) {
        //方向指定
        Vector3[] vecfromCenter = new Vector3[4];
        Vector3[] lookAt = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = endPos[i] - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAt[i] = status.GetFoots()[i].position + vecfromCenter[i];
        }
        eneTree.SetLookAtPos(lookAt);
    }
}

public class BTDiedBossCommand : BaseBTCommand {
    public override BaseBTNode.NodeStatus Activate() {
        throw new NotImplementedException();
    }
}

public class BTStompBossCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private Animator[] footsAnim;
    private Vector3[] wantedPos;
    private float[] wantedAngle;
    private BossStatus status;
    private Transform rootPos;
    private float persuitTimer;
    private float persuitTime = 2;
    private int switchCount;
    private float readyTimer;
    private float readyTime = 0.5f;
    private List<Transform> canSelectFoots;
    private Transform preFallFoot;
    private int fallCount;
    private int fallMaxCount = 6;

    public override void Initialize(GameObject usingObj) {
        eneTree = GetComponent<BaseEnemyTree>();
        footsAnim = usingObj.GetComponentsInChildren<Animator>();
        Array.Resize(ref footsAnim, 4);
        wantedPos = new Vector3[4];
        status = GetComponent<BossStatus>();
        persuitTimer = 0;
        switchCount = 0;
        wantedAngle = new float[4];
        canSelectFoots = new List<Transform>(4);
        preFallFoot = null;
    }

    public override void Enter() {
        status.SetCanBePersuit(false);
        rootPos = status.GetFootCenter();
        switchCount = 0;
        for (int i = 0; i < wantedPos.Length; i++)
            canSelectFoots.Add(status.GetFoots()[i]);

        for (int i = 0; i < footsAnim.Length; i++)
            footsAnim[i].SetBool("Ready", true);
        fallCount = 0;
        preFallFoot = null;
    }

    public override BaseBTNode.NodeStatus Activate() {
        //全足の場所をlerp
        //降ろすと同時に２本目が追いかける(前の足を上げる)
        //1秒待つ
        //上げる
        switch (switchCount) {
            case 0:
                float[] diff = new float[4];
                int diffCount = 0;
                //角度と外積でソートしてcanSelectの左から-20, 0, 20, 40って決める？
                diff[0] = 0; diff[1] = 20; diff[2] = -20; diff[3] = 40;
                for (int i = 0; i < wantedAngle.Length; i++) {
                    if (canSelectFoots.Contains(status.GetFoots()[i])) {
                        wantedAngle[i] = rootPos.rotation.eulerAngles.y + diff[diffCount];
                        diffCount++;
                    }
                }

                //AngleからPositionに変換する
                for (int i = 0; i < wantedAngle.Length; i++) {
                    wantedPos[i] = status.GetFootCenter().position;
                    wantedPos[i].z += status.GetDistanceFromCenter() * Mathf.Cos(wantedAngle[i] * Mathf.Deg2Rad);
                    wantedPos[i].x += status.GetDistanceFromCenter() * Mathf.Sin(wantedAngle[i] * Mathf.Deg2Rad);
                    wantedPos[i] = Vector3.Lerp(status.GetFoots()[i].position, wantedPos[i], Time.fixedDeltaTime);
                }
                SetMovePosition(wantedPos);
                SetLookAt(wantedPos);
                status.GetBody().LookAt(status.GetBody().position + rootPos.forward);

                persuitTimer += Time.fixedDeltaTime;
                if (persuitTimer > persuitTime) {
                    persuitTimer = 0;
                    switchCount++;
                }
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 1:
                readyTimer += Time.fixedDeltaTime;
                if (readyTimer > readyTime)
                    switchCount++;
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 2:
                //一番プレイヤーに近い足を落とす
                SortedDictionary<float, Animator> min_dis_map = new SortedDictionary<float, Animator>();
                for (int i = 0; i < wantedPos.Length; i++) {
                    float disToPlayer = (status.GetPlayer().position
                        - status.GetFoots()[i].position).magnitude;
                    //選べる足なら
                    if (canSelectFoots.Contains(footsAnim[i].transform))
                        min_dis_map[disToPlayer] = footsAnim[i];
                }
                foreach (Animator an in min_dis_map.Values) {
                    an.SetBool("Ready", false);
                    an.SetBool("Maul", true);

                    if (preFallFoot != null) {
                        Animator preAnim = preFallFoot.GetComponent<Animator>();
                        preAnim.SetBool("Maul", false);
                        preAnim.SetBool("Return", false);
                        preAnim.SetBool("Ready", true);
                        canSelectFoots.Add(preFallFoot);
                    }
                    preFallFoot = an.transform;
                    canSelectFoots.Remove(an.transform);
                    break;
                }
                fallCount++;
                //超えたら終わり
                if (fallCount >= fallMaxCount) switchCount++;
                else switchCount = 0; //初めから

                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 3:
                persuitTimer += Time.fixedDeltaTime;
                if (persuitTimer > persuitTime) {
                    persuitTimer = 0;
                    switchCount++;
                }
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 4:
                //モーションが完了するまで
                Animator anim = preFallFoot.GetComponent<Animator>();
                anim.SetBool("Maul", false);
                anim.SetBool("Return", true);
                AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                if (info.normalizedTime >= 0.6f && info.IsName("ReturnBaseFoot")) {
                    switchCount++;
                }
                SetMovePosition(wantedPos);
                return BaseBTNode.NodeStatus.STATUS_RUNNING;

            case 5:
                //全て完了
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        //ここに来るのはありえない
        return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }

    public override void Exit() {
        canSelectFoots.Clear();
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }

    public override void ExitForSetFailuer() {
        canSelectFoots.Clear();
        status.SetCanBePersuit(true);
        for (int i = 0; i < footsAnim.Length; i++) {
            footsAnim[i].SetBool("Return", false);
        }
    }

    public void SetMovePosition(Vector3[] endPos) {
        eneTree.SetMovePosition((Vector3[])endPos.Clone());
    }

    public void SetLookAt(Vector3[] endPos) {
        //方向指定
        Vector3[] vecfromCenter = new Vector3[4];
        Vector3[] lookAt = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = endPos[i] - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAt[i] = status.GetFoots()[i].position + vecfromCenter[i];
        }

        eneTree.SetLookAtPos((Vector3[])lookAt.Clone());
    }
}
public class BTUpDownCrabScissorsBossCommand : BaseBTCommand {
    public override BaseBTNode.NodeStatus Activate() {
        throw new NotImplementedException();
    }
}
public class BTSeaWavyBossCommand : BaseBTCommand {
    public override BaseBTNode.NodeStatus Activate() {
        throw new NotImplementedException();
    }
}
public class BTMeteoWithWaterGunBossCommand : BaseBTCommand {
    public override BaseBTNode.NodeStatus Activate() {
        throw new NotImplementedException();
    }
}