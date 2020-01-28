using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//追跡タイプの敵のビヘイビアツリー
public class EnemyTreeOfPursuiter : BaseEnemyTree {
    //ツリー構造
    private BTTEandNTEBoolSelector m_rootNode;
                private BTConditionNotToEnd m_isFlownAttackedNode;
                private BTBoolSelector m_damageExpressionNode;
                            private BTConditionToEnd m_isDyingNode;
                            private BTActionNode m_dyingNode;
                            private BTActionNode m_nockBackingNode;
                private BTBoolSelector m_commonActionNode;
                            private BTConditionNotToEnd m_isFindingPlayerNode;
                            private BTSequence m_attackFlowingNode;
                                        private BTActionNode m_belgianWaitingNode;
                                        private BTActionNode m_attackingNode;
                                        private BTActionNode m_followThroughNode; //1秒
                           private BTSequence m_wanderFlowingNode;
                                       private BTActionNode m_wanderingWaitingNode; //3秒
                                       private BTActionNode m_wanderingNode;

	//アクションコマンド
    private BaseBTCommand m_nockBackingCommand;
    private BaseBTCommand m_dyingCommand;
    private BaseBTCommand m_findingReactionCommand;
    private BaseBTCommand m_belgianWaitingCommand;
    private BaseBTCommand m_attackingCommand;
    private BaseBTCommand m_followThroughWaitingCommand;
    private BaseBTCommand m_wanderingWaitingCommand;
    private BaseBTCommand m_wanderingCommand;

	//条件クラス
    private BaseBTConditionReal m_isFlownAttackedCondition;
    private BaseBTConditionReal m_isDyingCondition;
    private BaseBTConditionReal m_isFindingPlayerCondition;

    private Vector3 movePosition; //移動ベクトル
    private Vector3 lookAtPosition; //視点ベクトル
    private EnemyStatus status; //ステータス
    private float speed; //スピード

	//初期化処理
    void Start() {
        //Commandの生成
        m_nockBackingCommand                = this.gameObject.AddComponent<BTNockBackingCommand>();
        m_dyingCommand                          = this.gameObject.AddComponent<BTDyingCommand>();
        m_findingReactionCommand = this.gameObject.AddComponent<BTFindingReactionCommand>();
        m_belgianWaitingCommand            = this.gameObject.AddComponent<BTBelgianWaitingCommand>();
        m_attackingCommand                     = this.gameObject.AddComponent<BTAttackingCommand>();
        m_followThroughWaitingCommand = this.gameObject.AddComponent<BTWaitingCommand>();
        m_wanderingWaitingCommand       = this.gameObject.AddComponent<BTWaitingCommand>();
        m_wanderingCommand                   = this.gameObject.AddComponent<BTWanderingCommand>();

        //Conditionの生成
        m_isFlownAttackedCondition = this.gameObject.AddComponent<BTIsFlownAttackedCondition>();
        m_isDyingCondition = this.gameObject.AddComponent<BTIsDyingCondition>();
        m_isFindingPlayerCondition           = this.gameObject.AddComponent<BTIsFindingPlayerCondition>();

        //Commandの初期化
        m_nockBackingCommand.Initialize(this.gameObject);
        m_dyingCommand.Initialize(this.gameObject);
        m_findingReactionCommand.Initialize(this.gameObject);
        m_belgianWaitingCommand.Initialize(this.gameObject);
        m_attackingCommand.Initialize(this.gameObject);
        m_followThroughWaitingCommand.Initialize(1);
        m_wanderingWaitingCommand.Initialize(3);
        m_wanderingCommand.Initialize(this.gameObject);

        //Conditionの初期化
        m_isFlownAttackedCondition.Initialize(this.gameObject);
        m_isDyingCondition.Initialize(this.gameObject);
        m_isFindingPlayerCondition.Initialize(this.gameObject);


		//Nodeの生成・初期化・連結
        //third1
        m_belgianWaitingNode = new BTActionNode(m_belgianWaitingCommand);
        m_attackingNode = new BTActionNode(m_attackingCommand);
        m_followThroughNode = new BTActionNode(m_followThroughWaitingCommand);

        //third2
        m_wanderingNode = new BTActionNode(m_wanderingCommand);
        m_wanderingWaitingNode = new BTActionNode(m_wanderingWaitingCommand);

        //second1
        m_isDyingNode = new BTConditionToEnd(m_isDyingCondition);
        m_dyingNode = new BTActionNode(m_dyingCommand);
        m_nockBackingNode = new BTActionNode(m_nockBackingCommand);

        //second2
        m_isFindingPlayerNode = new BTConditionNotToEnd(m_isFindingPlayerCondition);
        List<BaseBTNode> attackFlow = new List<BaseBTNode>();
        attackFlow.Add(m_belgianWaitingNode);
        attackFlow.Add(m_attackingNode);
        attackFlow.Add(m_followThroughNode);
        m_attackFlowingNode = new BTSequence(attackFlow);
        List<BaseBTNode> wanderFlow = new List<BaseBTNode>();
        wanderFlow.Add(m_wanderingNode);
        wanderFlow.Add(m_wanderingWaitingNode);
        m_wanderFlowingNode = new BTSequence(wanderFlow);

        //first
        m_isFlownAttackedNode = new BTConditionNotToEnd(m_isFlownAttackedCondition);
        m_damageExpressionNode = new BTBoolSelector(m_dyingNode, m_nockBackingNode, m_isDyingNode);
        m_commonActionNode = new BTBoolSelector(m_attackFlowingNode, m_wanderFlowingNode, m_isFindingPlayerNode);

        //root
        m_rootNode = new BTTEandNTEBoolSelector(m_damageExpressionNode, m_commonActionNode,
            m_isFlownAttackedNode);

        status = GetComponent<EnemyStatus>();
        status.SetEnemyType(EnemyType.Pursuiter);

        speed = 0;
    }

    //実行処理
    public  override Vector3 Activate(ref Vector3 lookAtPos) {
        movePosition = Vector3.zero;
        m_rootNode.Evaluate();

        if (GetComponent<CharacterController>().isGrounded)
            speed = 0;
        speed -= status.GetGravity() * Time.fixedDeltaTime;
        movePosition.y += speed * Time.fixedDeltaTime;

        lookAtPos = lookAtPosition;
        return movePosition;
    }

    public override void SetMovePosition(Vector3 pos) {
        movePosition = pos;
    }

    public override void SetLookAtPos(Vector3 pos) {
        lookAtPosition = pos;
    }
}
