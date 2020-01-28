using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃を食らっているかの条件クラス
public class BTIsAttackedCondition : BaseBTConditionReal {
    private EnemyStatus status;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<EnemyStatus>();
    }

    public override BaseBTNode.NodeStatus EvaluateCondition() {
        if (status.GetDamaged())
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        else
            return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }
}

//吹っ飛ぶ条件が耐久値が閾値を超えたときとDeadがtrueになったときの、
//攻撃を食らっているかの条件クラス
public class BTIsFlownAttackedCondition : BaseBTConditionReal {
    private EnemyStatus status;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<EnemyStatus>();
    }

    public override BaseBTNode.NodeStatus EvaluateCondition() {
        if (status.GetFlownDamaged() || status.GetIsDying())
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        else
            return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }
}

//死んでいるかの条件クラス
public class BTIsDyingCondition : BaseBTConditionReal {
    private EnemyStatus status;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<EnemyStatus>();
    }

    public override BaseBTNode.NodeStatus EvaluateCondition() {
        if (status.GetIsDying())
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        else
            return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }
}

//ボスが死んでいるかの条件クラス
public class BTIsDyingBossCondition : BaseBTConditionReal {
    private BossStatus status;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<BossStatus>();
    }

    public override BaseBTNode.NodeStatus EvaluateCondition() {
        if (status.GetIsDying())
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        else
            return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }
}

//体力が半分減ったかの条件クラス
public class BTIsHealthHalfDownCondition : BaseBTConditionReal {
    private BossStatus status;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<BossStatus>();
    }

    public override BaseBTNode.NodeStatus EvaluateCondition() {
        if (status.GetHealth() <= status.GetMaxHealth()/2.0f)
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        else
            return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }
}

//すでに死んでいるかの条件クラス
public class BTIsAfterDyingCondition : BaseBTConditionReal {
    private BossStatus status;
    private bool afterDying;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<BossStatus>();
        afterDying = false;
    }

    public override BaseBTNode.NodeStatus EvaluateCondition() {

        if (afterDying)
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        else {
            if (status.GetIsDying()) afterDying = true;
            return BaseBTNode.NodeStatus.STATUS_FAILURE;
        }
    }
}

//プレイヤーを発見できるかの条件クラス
public class BTIsFindingPlayerCondition : BaseBTConditionReal {
    private EnemyStatus status;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<EnemyStatus>();
    }

    public override BaseBTNode.NodeStatus EvaluateCondition() {
        float lengthToPlayer = (status.player.transform.position - transform.position).magnitude;
        if (lengthToPlayer < status.GetVisualForPlayerDistance())
            return BaseBTNode.NodeStatus.STATUS_SUCCESS;
        else
            return BaseBTNode.NodeStatus.STATUS_FAILURE;
    }
}