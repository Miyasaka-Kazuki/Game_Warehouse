using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子ノードを順番に実行するシーケンスノードクラス
public class BTSequence : BaseBTNode {
    private List<BaseBTNode> m_nodes; //子ノードリスト
    private BaseBTCondition m_conditionNode; //ノードの状態
    private bool m_isRunningNode; //子ノードの状態を全てRunningにしているか

	//コンストラクタ
    public BTSequence(List<BaseBTNode> nodes, BaseBTCondition condNode = null) {
        m_nodes = nodes;
        m_conditionNode = condNode;
        m_isRunningNode = false;
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
            m_isRunningNode = true;
        }

		//子ノードの処理を順番に実行
        foreach (BaseBTNode node in m_nodes) {
            if (m_isRunningNode) {
                if (node != runningNode) continue;
                else {
                    m_isRunningNode = false;
                }
            }
            switch (node.Evaluate()) {
                case NodeStatus.STATUS_SUCCESS:
                    continue;
                case NodeStatus.STATUS_FAILURE:
                    m_nodeStatus = NodeStatus.STATUS_FAILURE;
                    return m_nodeStatus;
                case NodeStatus.STATUS_RUNNING:
                    m_nodeStatus = NodeStatus.STATUS_RUNNING;
                    return m_nodeStatus;
                default:
                    m_nodeStatus = NodeStatus.STATUS_SUCCESS;
                    return m_nodeStatus;
            }
        }
        m_nodeStatus = NodeStatus.STATUS_SUCCESS;
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
