﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Conditionの条件を満たした時、NotToEnd(途中で終了する可能性あり)として振る舞い、
//満たさない時、ToEnd(最後まで実行)として振る舞うboolセレクタークラス
public class BTNTEandTEBoolSelector : BaseBTNode {
    private BaseBTNode[] m_nodes;
    private BTConditionNotToEnd m_conditionNode;
    private BaseBTNode m_preNode;
    private bool m_firstSearch; //ToEndNodeの実行が終了したときtrueになる

    public BTNTEandTEBoolSelector(BaseBTNode notToEndNodeWithTrue, BaseBTNode toEndNodeWithFalse, BTConditionNotToEnd condNode) {
        m_nodes = new BaseBTNode[2];
        m_nodes[0] = notToEndNodeWithTrue;
        m_nodes[1] = toEndNodeWithFalse;
        m_conditionNode = condNode;
        m_preNode = m_nodes[1];
        m_firstSearch = true;
    }

    public override NodeStatus Evaluate() {
        if (m_conditionNode != null) {
            NodeStatus status = m_conditionNode.EvaluateCondition(m_nodeStatus);
            switch (status) {
                case NodeStatus.STATUS_SUCCESS:
                    //前がToEnd実行完了のとき
                    //前がNotToEnd実行中・NotToEnd実行完了のとき
                    if ((m_firstSearch && m_preNode == m_nodes[1]) ||
                        (!m_firstSearch && m_preNode == m_nodes[0])) {
                        m_firstSearch = false;
                        if (m_nodes[0] != m_preNode) m_preNode.SetFailuer();
                        m_nodeStatus = m_nodes[0].Evaluate();
                        m_preNode = m_nodes[0];
                        return m_nodeStatus;
                    } else {
                        //前がToEnd実行中のとき
                        m_nodeStatus = m_nodes[1].Evaluate();
                        m_preNode = m_nodes[1];
                        if (m_nodeStatus != NodeStatus.STATUS_RUNNING)
                            m_firstSearch = true;
                        return m_nodeStatus;
                    }
                case NodeStatus.STATUS_FAILURE:
                    if (m_firstSearch) m_firstSearch = false;
                    if (m_nodes[1] != m_preNode) m_preNode.SetFailuer();
                    m_nodeStatus = m_nodes[1].Evaluate();
                    m_preNode = m_nodes[1];
                    if (m_nodeStatus != NodeStatus.STATUS_RUNNING)
                        m_firstSearch = true;
                    return m_nodeStatus;
                case NodeStatus.STATUS_RUNNING:
                    m_nodeStatus = m_preNode.Evaluate();
                    if (m_nodeStatus != NodeStatus.STATUS_RUNNING &&
                        m_preNode == m_nodes[0])
                        m_firstSearch = true;
                    return m_nodeStatus;
            }
        }
        //ここにくるのはおかしい
        return NodeStatus.STATUS_FAILURE;
    }

    public override void SetFailuer() {
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        m_preNode.SetFailuer();
        m_firstSearch = true;
        m_preNode = m_nodes[1];
    }
}
