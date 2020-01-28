using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//コンストラクタで指定した回数分、子ノードの処理を繰り返すデコレーターノードクラス
public class BTLimiter : BaseBTNode {
    private BaseBTNode m_node;
    private int m_counter;
    private int m_maxCount;

    public BTLimiter(BaseBTNode node, int maxCount) {
        m_node = node;
        m_maxCount = maxCount;
        m_counter = 0;
    }

    public override NodeStatus Evaluate() {
        if (m_nodeStatus != NodeStatus.STATUS_RUNNING) m_counter = 0;
        NodeStatus status = m_node.Evaluate();

        if (status == NodeStatus.STATUS_FAILURE) {
            m_nodeStatus = NodeStatus.STATUS_FAILURE;
            return m_nodeStatus;
        }
        m_counter++;
        if (m_counter == m_maxCount) m_nodeStatus = NodeStatus.STATUS_SUCCESS;
        else                          m_nodeStatus = NodeStatus.STATUS_RUNNING;
        return m_nodeStatus;
    }

    public override void SetFailuer() {
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        m_node.SetFailuer();
    }
}
