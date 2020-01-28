using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//条件判定をするコンディションノードクラス
public class BTConditionNotToEnd : BaseBTCondition {
    private BaseBTConditionReal m_condition; //条件クラス

	//条件を設定
    public BTConditionNotToEnd(BaseBTConditionReal cond) {
        m_condition = cond;
    }

	//条件式が満たせばSuccess、実行中ならRunningを返す
	//ステータスが失敗なら子ノードをすべて失敗にしてFailuerを返す
    public override NodeStatus EvaluateCondition(NodeStatus upperStatus) {
        NodeStatus status = m_condition.EvaluateCondition();
        if (status != NodeStatus.STATUS_FAILURE ) {
            m_nodeStatus = NodeStatus.STATUS_SUCCESS;
            return m_nodeStatus;
        }
			
        m_nodeStatus = NodeStatus.STATUS_FAILURE;
        return m_nodeStatus;
    }
}
