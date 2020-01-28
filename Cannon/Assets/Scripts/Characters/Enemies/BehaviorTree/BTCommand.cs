using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//待機コマンド
public class BTWaitingCommand : BaseBTCommand {
    private float waitingTimer;
    private float needingWaitingTime;

    public override void Initialize(float time) {
        needingWaitingTime = time;
    }

    public override void Enter() {
        waitingTimer = 0;
    }

    public override BaseBTNode.NodeStatus Activate() {
        waitingTimer += Time.fixedDeltaTime;
        if (waitingTimer > needingWaitingTime)
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }
}

//追跡コマンド
public class BTPersuitCommand : BaseBTCommand {
    private float speed = 3;
    private float playerSpeed;
    private BaseEnemyTree eneTree;
    private EnemyStatus status;
    

    public override void Initialize (GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        playerSpeed = status.player.GetComponent<PlayerStatus>().GetMoveSpeed();
	}

    public override BaseBTNode.NodeStatus Activate() {
        Vector3 directionToPlayer = (status.player.position - transform.position).normalized;

        Vector3 movePosition = Vector3.zero;
            movePosition = directionToPlayer * speed * Time.fixedDeltaTime;
        eneTree.SetMovePosition(movePosition);
        eneTree.SetLookAtPos(transform.position + movePosition);

        if ((status.player.transform.position - transform.position).magnitude < status.GetCloseToPlayerDistance())
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;

        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }
}

//攻撃コマンド
public class BTAttackingCommand : BaseBTCommand {
    private EnemyStatus status;
    private BaseEnemyTree eneTree;
    private Animator anim;
    private BoxCollider attackCol;

    public override void Initialize(GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        anim = usingObj.GetComponent<Animator>();
    }

    public override void Enter() {
        Vector3 plane_playerPos = status.player.position;
        plane_playerPos.y = 0;
        Vector3 plane_enemyPos = transform.position;
        plane_enemyPos.y = 0;
        Vector3 dir = plane_playerPos - plane_enemyPos;

        eneTree.SetLookAtPos(transform.position + dir);

        if (attackCol == null) 
            attackCol = status.GetAttackCol();
        attackCol.enabled = true;
        anim.SetBool("Attack", true);
    }

    public override BaseBTNode.NodeStatus Activate() {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float animTime = info.normalizedTime;
        if (animTime >= 1  && info.IsTag("Attack")) {
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }

    public override void Exit() {
        attackCol.enabled = false;
        anim.SetBool("Attack", false);
    }

    public override void ExitForSetFailuer() {
        attackCol.enabled = false;
        anim.SetBool("Attack", false);
    }
}

//ランダムさまよいコマンド
public class BTWanderingCommand : BaseBTCommand {
    private EnemyStatus status;
    private BaseEnemyTree eneTree;
    private Vector2 cMinInsideCircle;
    private Vector3 velocity;
    private Vector3 nextPosition;
    private Animator anim;

    public override void Initialize(GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        cMinInsideCircle.Set(1, 1);
        anim = GetComponent<Animator>();
    }

    public override void Enter() {
        //ドーナツ型のランダムな値を設定する
        Vector2 nextPos2 = 0.1f * UnityEngine.Random.insideUnitCircle;
        if (nextPos2.x >= 0) nextPos2.x += cMinInsideCircle.x;
        else nextPos2.x -= cMinInsideCircle.x;
        if (nextPos2.y >= 0) nextPos2.y += cMinInsideCircle.y;
        else nextPos2.y -= cMinInsideCircle.y;

        nextPosition = new Vector3(nextPos2.x, 0, nextPos2.y) + transform.position;
        velocity = (nextPosition - transform.position).normalized * status.GetWanderSpeed();

        velocity.y = 0;
        eneTree.SetLookAtPos(transform.position + velocity);

        anim.SetBool("Run", true);
    }

    public override BaseBTNode.NodeStatus Activate() {
        Vector3 movePosition = velocity * Time.fixedDeltaTime;
        eneTree.SetMovePosition(movePosition);

        if (Mathf.Abs((nextPosition - transform.position).magnitude) < 0.1f)
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }

    public override void Exit() {
        anim.SetBool("Run", false);
    }

    public override void ExitForSetFailuer() {
        anim.SetBool("Run", false);
    }
}

//逃走コマンド
public class BTRunningAwayCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private EnemyStatus status;
    private Animator anim;
    private NavMeshPath awayPath;
    private int curIndex = 0;

    public override void Initialize(GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        anim = GetComponent<Animator>();
    }

    public override void Enter() {
        curIndex = 0;

        awayPath = new NavMeshPath();
        Vector3 destination = status.GetAwayDestination();
        NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, awayPath);

        anim.SetBool("Run", true);
    }

    public override BaseBTNode.NodeStatus Activate() {
        if (awayPath.corners.Length == 0)
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;

        Vector3 curTargetPos = awayPath.corners[curIndex];

        if (Vector3.Distance(transform.position, curTargetPos) < 0.5f) {

            if (curIndex + 1 < awayPath.corners.Length)
                curIndex++;
            else
                return BaseBTNode.NodeStatus.STATUS_SUCCESS;

        }

        Vector3 direction = (curTargetPos - transform.position).normalized;
        eneTree.SetMovePosition(direction * status.GetMoveSpeed() * Time.deltaTime);
        direction.y = 0;
        eneTree.SetLookAtPos(transform.position + direction);

        return BaseBTNode.NodeStatus.STATUS_RUNNING;

    }

    public override void Exit() {
        anim.SetBool("Run", false);
    }

	public override void ExitForSetFailuer() {
        anim.SetBool("Run", false);
    }
}

//弾を撃つコマンド
public class BTShootingCommand : BaseBTCommand {
    private BaseEnemyTree eneTree;
    private EnemyStatus status;
    private WeaponStatus weaponStat;
    private GameObject bulletPrefab;
    private int shootCount; //3回まで撃つ
    private float intervalTimer;
    private float thresholdTime;
    private Animator anim;

    public override void Initialize(GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        weaponStat = usingObj.GetComponent<WeaponStatus>();
        bulletPrefab = weaponStat.enemyBulletPrefab;
        shootCount = 0;
        intervalTimer = 0;
        thresholdTime = weaponStat.gunIntervalTime;
        anim = GetComponent<Animator>();
    }
		
    public override void Enter() {
        shootCount = 0;
        intervalTimer = 0;
        anim.SetBool("GunStart", true);
    }

    public override BaseBTNode.NodeStatus Activate() {
        if (status.GetNotUseWeapon())
            return BaseBTNode.NodeStatus.STATUS_RUNNING;

        if (intervalTimer > thresholdTime) {
            anim.SetBool("Attack", true);
            anim.SetTrigger("Attacking");
            shootCount++;
            intervalTimer = 0;
            Vector3 dir = status.player.position - transform.position;
            dir.y = 0;

            eneTree.SetLookAtPos(transform.position + dir);
            GameObject ins = Instantiate(bulletPrefab, weaponStat.weaponInstancePosition.position,
                this.gameObject.transform.rotation);
        }
        intervalTimer += Time.fixedDeltaTime;

        if (shootCount >= 3)
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;

        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }

    public override void Exit() {

        anim.SetBool("GunStart", false);
        anim.SetBool("Attack", false);
    }

    public override void ExitForSetFailuer() {

        anim.SetBool("GunStart", false);
        anim.SetBool("Attack", false);
    }
}

//くらいノックバックコマンド
public class BTNockBackingCommand : BaseBTCommand {
    private EnemyStatus status;
    private BaseEnemyTree eneTree;
    private Animator anim;
    private int count = 0;
    private float timer;
    private float thresholdTime = 2;

    public override void Initialize(GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        anim = usingObj.GetComponent<Animator>();
        timer = 0;
    }

    public override void Enter() {
        anim.SetTrigger("Damage");
        anim.ResetTrigger("Wait");
        count++;
        timer = 0;
    }

    public override BaseBTNode.NodeStatus Activate() {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float animTime;
        if (info.IsName("DownToUp"))
            animTime = info.normalizedTime;
        else
            animTime = 0;

        timer += Time.fixedDeltaTime;
        if ((animTime > 1 && info.IsName("DownToUp")) ||
            timer > thresholdTime) {
            //ダメージモーションが終わったらfalseにする
            anim.SetTrigger("Wait");
            status.SetFlownDamagedWithFalse();
            status.ResetSuperArmorValue();
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }
}

//死亡コマンド
public class BTDyingCommand : BaseBTCommand {
    private EnemyStatus status;
    private BelgianAI belgianAI;
    private Animator anim;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<EnemyStatus>();
        belgianAI = usingObj.GetComponentInParent<BelgianAI>();
        anim = GetComponent<Animator>();
    }

    public override void Enter() {
        status.ResetSuperArmorValue();

		//BelgianAIから外す
        belgianAI.RemoveFromBelgianAI(this.gameObject);
        anim.SetTrigger("Dead");
    }

    public override BaseBTNode.NodeStatus Activate() {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float animTime = info.normalizedTime;
        if (animTime > 1 && info.IsTag("Dead")) {
            status.SetAfforTriUsedForFalse();
            Destroy(this.gameObject);
        }
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }
}

//プレイヤー発見コマンド
public class BTFindingReactionCommand : BaseBTCommand {
    private EnemyStatus status;
    private BaseEnemyTree eneTree;
    private Animator anim;

    public override void Initialize(GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        anim = GetComponent<Animator>();
    }

    public override void Enter() {
    }

    public override BaseBTNode.NodeStatus Activate() {
        anim.Play("findingPlayer");
       float endTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (endTime >= 1) {
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }
}

//BelgianAIへの追加待機コマンド
public class BTBelgianWaitingCommand : BaseBTCommand {
    private EnemyStatus status;
    private BaseEnemyTree eneTree;
    private BelgianAI belgianAI;
    private const float delayThresholdTime = 0.5f;
    private float delayTimer;
    private Animator anim;

    public override void Initialize(GameObject usingObj) {
        eneTree = usingObj.GetComponent<BaseEnemyTree>();
        status = usingObj.GetComponent<EnemyStatus>();
        belgianAI = transform.root.GetComponentInChildren<BelgianAI>();
        delayTimer = 0;
        anim = GetComponent<Animator>();
    }

    public override void Enter() {
        if (!belgianAI.IsInFindEnemies(this.gameObject))
            belgianAI.AddFindEnemies(this.gameObject);
        delayTimer = 0;
        anim.SetBool("Run", false);
    }

    public override BaseBTNode.NodeStatus Activate() {
        if (belgianAI.IsInGridEnemies(this.gameObject)) {

            //gridPositionから離れて2秒経っているなら追いかける
            if (belgianAI.IsInCloseToGriPos(this.gameObject)) {

                anim.SetBool("Run", false); //近いなら止まる
                delayTimer = 0;
            } else {

                bool can_chase = false;
                if (delayTimer > delayThresholdTime) {

                    can_chase = true;
                    anim.SetBool("Run", true); //近くなく遅延時間が終わったら走る
                } else {

                    delayTimer += Time.fixedDeltaTime;
                }

                if (can_chase) {
                    Vector3 gridPlanePos = belgianAI.ComputeRegisteredGridPosition(this.gameObject);
                    gridPlanePos.y = 0;
                    Vector3 enemyPlanePos = transform.position;
                    enemyPlanePos.y = 0;
                    Vector3 dir = (gridPlanePos - enemyPlanePos).normalized;
                    Vector3 movePosition = dir * status.GetMoveSpeed() * Time.fixedDeltaTime;
                    eneTree.SetMovePosition(movePosition);

                    eneTree.SetLookAtPos(transform.position + dir);
                }
            }
        }

        if (belgianAI.CanAttackPlayer(this.gameObject)) {
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        }
        return BaseBTNode.NodeStatus.STATUS_RUNNING;
    }

    public override void Exit() {
        anim.SetBool("Run", false);
    }

    public override void ExitForSetFailuer() {
        belgianAI.RemoveFromBelgianAI(this.gameObject);
        anim.SetBool("Run", false);
    }
}
