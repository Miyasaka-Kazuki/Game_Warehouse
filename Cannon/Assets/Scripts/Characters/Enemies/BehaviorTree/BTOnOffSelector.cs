using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ランダムで子ノードを選択するデコレーターノードクラス
public class BTOnOffSelector : BaseBTNode {
    private List<BaseBTNode> m_nodes;
    private BaseBTCondition m_conditionNode;
    private List<BaseBTNode> m_failuerNodes; //ランダムから省くために使う
    private BaseBTNode m_preNode; //始めにランダムから省くため

    public BTOnOffSelector(List<BaseBTNode> nodes, BaseBTCondition condNode = null) {
        m_nodes = nodes;
        m_conditionNode = condNode;
        m_failuerNodes = new List<BaseBTNode>();
        m_preNode = null;
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

        if (m_preNode != null) {
            m_failuerNodes.Add(m_preNode);
            m_nodes.Remove(m_preNode);
        }
        while (m_nodes.Count > 0) {
            int selectNode = UnityEngine.Random.Range(0, m_nodes.Count);
            switch (m_nodes[selectNode].Evaluate()) {
                case NodeStatus.STATUS_SUCCESS:
                    m_nodeStatus = NodeStatus.STATUS_SUCCESS;
                    m_preNode = m_nodes[selectNode];
                    //元に戻すため
                    foreach (BaseBTNode node in m_failuerNodes)
                        m_nodes.Add(node); //m_nodesにはないはず
                    m_failuerNodes.Clear();
                    return m_nodeStatus;
                case NodeStatus.STATUS_FAILURE:
                    //ランダムに選ぶものから外すため
                    m_failuerNodes.Add(m_nodes[selectNode]);
                    m_nodes.RemoveAt(selectNode);
                    continue;
                case NodeStatus.STATUS_RUNNING:
                    m_nodeStatus = NodeStatus.STATUS_RUNNING;
                    //元に戻すため
                    foreach (BaseBTNode node in m_failuerNodes)
                        m_nodes.Add(node); //m_nodesにはないはず
                    m_failuerNodes.Clear();
                    return m_nodeStatus;
                default:
                    continue;
            }
        }
        //元に戻すため
        m_preNode = null;
        foreach (BaseBTNode node in m_failuerNodes)
            m_nodes.Add(node); //m_nodesにはないはず
        m_failuerNodes.Clear();
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        return m_nodeStatus;
    }

    public override void SetFailuer() {
        m_preNode = null;
        foreach (BaseBTNode node in m_failuerNodes)
            m_nodes.Add(node); //m_nodesにはないはず
        m_failuerNodes.Clear();
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        BaseBTNode childNode = computeRunningNode(m_nodes);
        if (childNode != null) {
            childNode.SetFailuer();
        }
    }
}
