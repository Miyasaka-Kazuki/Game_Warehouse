using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//コンストラクタで指定した時間分、処理を遅らせる（待機させる）デコレーターノードクラス
public class BTRepeater : BaseBTNode {
    private BaseBTNode m_node;
    private float m_chargeTimer;
    private float m_maxChargeTime;

    public BTRepeater(BaseBTNode node, int maxChargeTime) {
        m_node = node;
        m_maxChargeTime = maxChargeTime;
        m_chargeTimer = 0;
    }

    public override NodeStatus Evaluate() {
        if (m_nodeStatus != NodeStatus.STATUS_RUNNING) m_chargeTimer = 0;

        m_chargeTimer += Time.fixedDeltaTime;
        if (m_chargeTimer >= m_maxChargeTime) {
            m_nodeStatus = m_node.Evaluate();
            return m_nodeStatus;
        } else
            m_nodeStatus = NodeStatus.STATUS_RUNNING;

        return m_nodeStatus;
    }

    public override void SetFailuer() {
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        m_node.SetFailuer();
    }
}
