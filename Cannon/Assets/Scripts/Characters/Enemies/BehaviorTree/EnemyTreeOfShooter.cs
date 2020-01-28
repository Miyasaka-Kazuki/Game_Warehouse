using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シュータータイプの敵のビヘイビアツリー(行動管理)クラス
public class EnemyTreeOfShooter : BaseEnemyTree {
    //ツリー構造
    private BTTEandNTEBoolSelector m_rootNode;
                private BTConditionNotToEnd m_isFlownAttackedNode;
                private BTSequence m_damageExpressionNode;
                            private BTActionNode m_nockBackingNode;
                            private BTBoolSelector m_afterNockBackNode;
                                        private BTConditionToEnd m_isDyingNode;
                                        private BTActionNode m_dyingNode;
                                        private BTActionNode m_flownDamageWaitingNode;
                private BTBoolSelector m_commonActionNode;
                            private BTConditionNotToEnd m_isFindingPlayerNode; 
                            private BTSequence m_attackFlowingNode; 
                                        private BTActionNode m_findingReactionNode; 
                                        private BTActionNode m_runningAwayNode;
                                        private BTActionNode m_forwardSwingWaitingNode; //1秒
                                        private BTActionNode m_shootingNode;
                                        private BTActionNode m_followThroughNode; //1秒
                            private BTSequence m_wanderFlowingNode;
                                        private BTActionNode m_wanderingWaitingNode; //3秒
                                        private BTActionNode m_wanderingNode;

	//アクションコマンド
    private BaseBTCommand m_nockBackingCommand;
    private BaseBTCommand m_dyingCommand;
    private BaseBTCommand m_flownDamageWaitingCommand;
    private BaseBTCommand m_findingReactionCommand;
    private BaseBTCommand m_runningAwayCommand;
    private BaseBTCommand m_forwardSwingWaitingCommand;
    private BaseBTCommand m_shootingCommand;
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
        m_nockBackingCommand = this.gameObject.AddComponent<BTNockBackingCommand>();
        m_dyingCommand = this.gameObject.AddComponent<BTDyingCommand>();
        m_flownDamageWaitingCommand = this.gameObject.AddComponent<BTWaitingCommand>();
        m_findingReactionCommand = this.gameObject.AddComponent<BTFindingReactionCommand>();
        m_runningAwayCommand = this.gameObject.AddComponent<BTRunningAwayCommand>();
        m_forwardSwingWaitingCommand = this.gameObject.AddComponent<BTWaitingCommand>();
        m_shootingCommand = this.gameObject.AddComponent<BTShootingCommand>();
        m_followThroughWaitingCommand = this.gameObject.AddComponent<BTWaitingCommand>();
        m_wanderingWaitingCommand = this.gameObject.AddComponent<BTWaitingCommand>();
        m_wanderingCommand = this.gameObject.AddComponent<BTWanderingCommand>();

        //Conditionの生成
        m_isFlownAttackedCondition = this.gameObject.AddComponent<BTIsFlownAttackedCondition>();
        m_isDyingCondition = this.gameObject.AddComponent<BTIsDyingCondition>();
        m_isFindingPlayerCondition = this.gameObject.AddComponent<BTIsFindingPlayerCondition>();

        //Commandの初期化
        m_nockBackingCommand.Initialize(this.gameObject);
        m_dyingCommand.Initialize(this.gameObject);
        m_flownDamageWaitingCommand.Initialize(0.1f);
        m_findingReactionCommand.Initialize(this.gameObject);
        m_runningAwayCommand.Initialize(this.gameObject);
        m_forwardSwingWaitingCommand.Initialize(1);
        m_shootingCommand.Initialize(this.gameObject);
        m_followThroughWaitingCommand.Initialize(1);
        m_wanderingWaitingCommand.Initialize(3);
        m_wanderingCommand.Initialize(this.gameObject);

        //Conditionの初期化
        m_isFlownAttackedCondition.Initialize(this.gameObject);
        m_isDyingCondition.Initialize(this.gameObject);
        m_isFindingPlayerCondition.Initialize(this.gameObject);


		//Nodeの生成・初期化・連結
        //third1
        m_isDyingNode = new BTConditionToEnd(m_isDyingCondition);
        m_dyingNode = new BTActionNode(m_dyingCommand);
        m_flownDamageWaitingNode = new BTActionNode(m_flownDamageWaitingCommand);

        //third2
        m_findingReactionNode = new BTActionNode(m_findingReactionCommand);
        m_runningAwayNode = new BTActionNode(m_runningAwayCommand);
        m_forwardSwingWaitingNode = new BTActionNode(m_forwardSwingWaitingCommand);
        m_shootingNode = new BTActionNode(m_shootingCommand);
        m_followThroughNode = new BTActionNode(m_followThroughWaitingCommand);

        //third3
        m_wanderingNode = new BTActionNode(m_wanderingCommand);
        m_wanderingWaitingNode = new BTActionNode(m_wanderingWaitingCommand);

        //second1
        m_nockBackingNode = new BTActionNode(m_nockBackingCommand);
        m_afterNockBackNode = new BTBoolSelector(m_dyingNode, m_flownDamageWaitingNode, m_isDyingNode);

        //second2
        m_nockBackingNode = new BTActionNode(m_nockBackingCommand);
        m_afterNockBackNode = new BTBoolSelector(m_dyingNode, m_flownDamageWaitingNode, m_isDyingNode);

        //second3
        m_isFindingPlayerNode = new BTConditionNotToEnd(m_isFindingPlayerCondition);
        List<BaseBTNode> attackFlow = new List<BaseBTNode>();
        attackFlow.Add(m_runningAwayNode);
        attackFlow.Add(m_forwardSwingWaitingNode);
        attackFlow.Add(m_shootingNode);
        attackFlow.Add(m_followThroughNode);
        m_attackFlowingNode = new BTSequence(attackFlow);
        List<BaseBTNode> wanderFlow = new List<BaseBTNode>();
        wanderFlow.Add(m_wanderingNode);
        wanderFlow.Add(m_wanderingWaitingNode);
        m_wanderFlowingNode = new BTSequence(wanderFlow);

        //first
        m_isFlownAttackedNode = new BTConditionNotToEnd(m_isFlownAttackedCondition);
        List<BaseBTNode> isFlownAttackedList = new List<BaseBTNode>();
        isFlownAttackedList.Add(m_nockBackingNode);
        isFlownAttackedList.Add(m_afterNockBackNode);
        m_damageExpressionNode = new BTSequence(isFlownAttackedList);
        m_commonActionNode = new BTBoolSelector(m_attackFlowingNode, m_wanderFlowingNode, m_isFindingPlayerNode);

        //root
        m_rootNode = new BTTEandNTEBoolSelector(m_damageExpressionNode, m_commonActionNode,
            m_isFlownAttackedNode);

        status = GetComponent<EnemyStatus>();
        status.SetEnemyType(EnemyType.Shooter);

        speed = 0;
    }

    //更新関数
    public override Vector3 Activate(ref Vector3 lookAtPos) {
        movePosition = Vector3.zero;
        lookAtPosition = transform.position + transform.forward;

        m_rootNode.Evaluate();

        lookAtPos = lookAtPosition;

        if (GetComponent<CharacterController>().isGrounded)
            speed = 0;
        speed -= status.GetGravity() * Time.fixedDeltaTime;
        movePosition.y += speed * Time.fixedDeltaTime;

        return movePosition;
    }

    public override void SetMovePosition(Vector3 pos) {
        movePosition = pos;
    }

    public override void SetLookAtPos(Vector3 pos) {
        lookAtPosition = pos;
    }
}
