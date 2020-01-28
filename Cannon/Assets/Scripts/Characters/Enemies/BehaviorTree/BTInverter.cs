using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子ノードの返すノードの状態を逆にするデコレータノードクラス
public class BTInverter : BaseBTNode {
    private BaseBTNode m_node;

    public BTInverter(BaseBTNode node) {
        m_node = node;
    }

    public override NodeStatus Evaluate() {
        switch (m_node.Evaluate()) {
            case NodeStatus.STATUS_SUCCESS:
                m_nodeStatus = NodeStatus.STATUS_FAILURE;
                return m_nodeStatus;
            case NodeStatus.STATUS_FAILURE:
                m_nodeStatus = NodeStatus.STATUS_SUCCESS;
                return m_nodeStatus;
            case NodeStatus.STATUS_RUNNING:
                m_nodeStatus = NodeStatus.STATUS_RUNNING;
                return m_nodeStatus;
        }
        m_nodeStatus = NodeStatus.STATUS_SUCCESS;
        return m_nodeStatus;
    }

    public override void SetFailuer() {
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        m_node.SetFailuer();
    }
}
