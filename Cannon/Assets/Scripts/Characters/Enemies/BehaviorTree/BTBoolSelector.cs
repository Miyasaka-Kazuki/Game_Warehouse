using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//二つの子ノードのうちどちらかを必ず実行するセレクターノードクラス
public class BTBoolSelector : BaseBTNode {
    private BaseBTNode[] m_nodes;
    private BaseBTCondition m_conditionNode;
    private BaseBTNode m_preNode;

	//コンストラクタ
    public BTBoolSelector(BaseBTNode trueNode, BaseBTNode falseNode, BaseBTCondition condNode) {
        m_nodes = new BaseBTNode[2];
        m_nodes[0] = trueNode;
        m_nodes[1] = falseNode;
        m_conditionNode = condNode;
        m_preNode = m_nodes[0];
    }

	//ノード状態の評価
    public override NodeStatus Evaluate() {
        if (m_conditionNode != null) {
            NodeStatus status = m_conditionNode.EvaluateCondition(m_nodeStatus);
            switch (status) {
                case NodeStatus.STATUS_SUCCESS:
                    if (m_nodes[0] != m_preNode) m_preNode.SetFailuer();
                    m_nodeStatus = m_nodes[0].Evaluate();
                    m_preNode = m_nodes[0];
                    return m_nodeStatus;
                case NodeStatus.STATUS_FAILURE:
                    if (m_nodes[1] != m_preNode) m_preNode.SetFailuer();
                    m_nodeStatus = m_nodes[1].Evaluate();
                    m_preNode = m_nodes[1];
                    return m_nodeStatus;
                case NodeStatus.STATUS_RUNNING:
                    m_nodeStatus = m_preNode.Evaluate();
                    return m_nodeStatus;
            }
        }
        //ここにくるのはおかしい
        return NodeStatus.STATUS_FAILURE;
    }

    public override void SetFailuer() {
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        m_preNode.SetFailuer();
    }
}
