using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//条件によって子ノードを選択するノードクラス
public class BTSelector : BaseBTNode {
    private List<BaseBTNode> m_nodes;
    private BaseBTCondition m_conditionNode;

    public BTSelector(List<BaseBTNode> nodes, BaseBTCondition condNode = null) {
        m_nodes = nodes;
        m_conditionNode = condNode;
    }

    public override NodeStatus Evaluate() {
        if (m_conditionNode != null) {
            NodeStatus status = m_conditionNode.EvaluateCondition(m_nodeStatus);
            if (status == NodeStatus.STATUS_FAILURE) {
                SetFailuer();
                return m_nodeStatus;
            }
        }

        BaseBTNode runningNode = computeRunningNode(m_nodes);
        if (runningNode != null) {
            switch (runningNode.Evaluate()) {
                case NodeStatus.STATUS_SUCCESS:
                    m_nodeStatus = NodeStatus.STATUS_SUCCESS;
                    return m_nodeStatus;
                case NodeStatus.STATUS_FAILURE:
                    break;
                case NodeStatus.STATUS_RUNNING:
                    m_nodeStatus = NodeStatus.STATUS_RUNNING;
                    return m_nodeStatus;
                default:
                    break;
            }
        }

        foreach (BaseBTNode node in m_nodes) {
            if (node == runningNode) continue;
            switch (node.Evaluate()) {
                case NodeStatus.STATUS_SUCCESS:
                    m_nodeStatus = NodeStatus.STATUS_SUCCESS;
                    return m_nodeStatus;
                case NodeStatus.STATUS_FAILURE:
                    continue;
                case NodeStatus.STATUS_RUNNING:
                    m_nodeStatus = NodeStatus.STATUS_RUNNING;
                    return m_nodeStatus;
                default:
                    continue;
            }
        }
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        return m_nodeStatus;
    }

    public override void SetFailuer() {
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        BaseBTNode node = computeRunningNode(m_nodes);
        if (node != null) {
            node.SetFailuer();
        }
    }
}
