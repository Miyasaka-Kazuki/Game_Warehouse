using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//実際に行動処理を実行する行動ノードクラス
public class BTActionNode : BaseBTNode {
    public BaseBTCommand m_command;

    public BTActionNode(BaseBTCommand command) {
        m_command = command;
    }

    public override NodeStatus Evaluate() {
        if (m_nodeStatus != NodeStatus.STATUS_RUNNING)
            m_command.Enter();
     
        m_nodeStatus = m_command.Activate();

        if (m_nodeStatus == NodeStatus.STATUS_SUCCESS)
            m_command.Exit();

        return m_nodeStatus;
    }

    public override void SetFailuer() {
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        m_command.ExitForSetFailuer();
    }
}
