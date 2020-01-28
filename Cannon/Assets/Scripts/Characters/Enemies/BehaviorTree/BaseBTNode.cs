using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ビヘイビアツリーのノードのベースクラス
public abstract class BaseBTNode {
	//ノードの状態(成功、失敗、実行中)
    public enum NodeStatus {
        STATUS_SUCCESS,
        STATUS_FAILURE,
        STATUS_RUNNING,
    }
    protected NodeStatus m_nodeStatus; //ノードの状態変数

    public abstract NodeStatus Evaluate(); //ノードの状態を決定する関数

	//ノードリストの全ての状態をRunningにする関数
    public BaseBTNode computeRunningNode(List<BaseBTNode> nodes) { 
        foreach (BaseBTNode node in nodes) {
            if (node.m_nodeStatus == NodeStatus.STATUS_RUNNING)
                return node;
        }
        return null;
    }    

	//ノードの状態の取得関数
    public NodeStatus GetStatus() {
        return m_nodeStatus;
    }

	//ノードの状態をFailureに設定する関数
    public virtual void SetFailuer() { }
}
