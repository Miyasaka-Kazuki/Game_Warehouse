using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスのビヘイビアツリー
public class EnemyTreeOfFirstBoss : BaseEnemyTree {
    //ツリー構造(体力が半分以上の時)
    private BTTEandNTEBoolSelector m_rootNode;
                private BTConditionNotToEnd m_isHealthHalfDownNode;
                private BTSequence m_halfMotionNode;
                            private BTActionNode m_angryMotionNode;
                            private BTActionNode m_playerObserverNode; //墨を出す間空中に放り出しておく
                            private BTActionNode m_inkSpewNode; //後半はインクで滑らす
                private BTSequence m_commonActionNode;
                            private BTActionNode m_instanceEnemyNode;
                            private BTActionNode m_waitingNode; //2秒
                            private BTOnOffSelector m_randomAttackNode;
                                        private BTActionNode m_rotationAttackNode; //回転
                                        private BTActionNode m_crabScissorsNode; //はさみ込み
                                        private BTActionNode m_momentMaulNode; //上から同時に降ろす
                                        private BTActionNode m_separateContinuousAttackNode; //別々連続攻撃
                                        private BTActionNode m_fireBallPreventingEscapeNode; //逃げ道塞いで炎弾攻撃
                            private BTActionNode m_waiting2Node; //1秒

    //ツリー構造(体力が半分未満の時)
    private BTTEandNTEBoolSelector m_rootNode2;
                private BTConditionNotToEnd m_isDyingNode;
                private BTBoolSelector m_afterDyingNode;
                            private BTConditionToEnd m_isAfterDyingNode;
                            private BTActionNode m_diedNode; //ピクピクしてる
                            private BTActionNode m_dyingMotionNode; //死んでる最中
                private BTSequence m_commonActionNode2;
                            private BTActionNode m_instanceEnemyNode2;
                            private BTActionNode m_waiting3Node; //5秒
                            private BTOnOffSelector m_randomTwoSelectionAttack;
                                        private BTOnOffSelector m_oldRandomAttackNode;
                                                    private BTActionNode m_rotationAttackNode2; //回転
                                                    private BTActionNode m_crabScissorsNode2; //はさみ込み
                                                    private BTActionNode m_momentMaulNode2; //上から同時に降ろす
                                                    private BTActionNode m_separateContinuousAttackNode2; //別々連続攻撃
                                                    private BTActionNode m_fireBallPreventingEscapeNode2; //逃げ道塞いで炎弾攻撃
                                        private BTOnOffSelector m_randomAttackNode2;
                                                    private BTActionNode m_stompNode; //連続緩やか振り下ろし攻撃
                                                    private BTActionNode m_upDownCrabScissorsNode; //上下の隙間なしはさみ込み
                                                    private BTActionNode m_seaWavyNode; //遠くから波の波状攻撃
                                                    private BTActionNode m_meteoWithWaterGunNode; //水鉄砲を上に撃ち多数の隕石を落とす
                            private BTActionNode m_waiting4Node; //3秒

	//アクションコマンド
    private BaseBTCommand m_angryMotionCommand;
    private BaseBTCommand m_playerObserverCommand;
    private BaseBTCommand m_inkSpewCommand;
    private BaseBTCommand m_instanceEnemyCommand;
    private BaseBTCommand m_waitingCommand;
    private BaseBTCommand m_rotationAttackCommand;
    private BaseBTCommand m_crabScissorsCommand;
    private BaseBTCommand m_momentMaulCommand;
    private BaseBTCommand m_separateContinuousAttackCommand;
    private BaseBTCommand m_fireBallPreventingEscapeCommand;
    private BaseBTCommand m_waiting2Command;
    private BaseBTCommand m_dyingMotionCommand;
    private BaseBTCommand m_diedCommand;
    private BaseBTCommand m_instanceEnemy2Command;
    private BaseBTCommand m_waiting3Command;
    private BaseBTCommand m_stompCommand;
    private BaseBTCommand m_upDownCrabScissorsCommand;
    private BaseBTCommand m_seaWavyCommand;
    private BaseBTCommand m_meteoWithWaterGunCommand;
    private BaseBTCommand m_waiting4Command;

	//条件クラス
    private BaseBTConditionReal m_isHealthHalfDownCondition;
    private BaseBTConditionReal m_isDyingCondition;
    private BaseBTConditionReal m_isAfterDyingCondition;

    private Vector3[] movePosition; //移動ベクトル
    private Vector3[] lookAtPosition; //視点ベクトル
    private BossStatus status; //ステータス
    private bool isHealthHalfDown; //体力が半分を下回ったか
	private bool isPreHealthHalfDown; //前フレームに体力が半分を下回ったか

    // Use this for initialization
    void Start() {
        //Commandの生成
        m_angryMotionCommand                      = gameObject.AddComponent<BTAngryMotionBossCommand>(); ;
        m_playerObserverCommand                  = gameObject.AddComponent<BTPlayerObserverBossCommand>();
        m_inkSpewCommand                            = gameObject.AddComponent<BTInkSpewBossCommand>();
        m_instanceEnemyCommand                  = gameObject.AddComponent<BTInstanceEnemyBossCommand>(); 
        m_waitingCommand                              = gameObject.AddComponent<BTWaitingCommand>();
        m_rotationAttackCommand                   = gameObject.AddComponent<BTRotationAttackBossCommand>();
        m_crabScissorsCommand                       = gameObject.AddComponent<BTCrabScissorsBossCommand>();     
        m_momentMaulCommand                     = gameObject.AddComponent<BTMomentMaulBossCommand>();
        m_separateContinuousAttackCommand = gameObject.AddComponent<BTSeparateContinuousAttackBossCommand>();
        m_fireBallPreventingEscapeCommand   = gameObject.AddComponent<BTFireBallPreventingEscapeBossCommand>();
        m_waiting2Command                            = gameObject.AddComponent<BTWaitingCommand>();
        m_dyingMotionCommand                      = gameObject.AddComponent<BTDyingMotionBossCommand>();  
        m_diedCommand                                  = gameObject.AddComponent<BTDiedBossCommand>();       
        m_instanceEnemy2Command                 = gameObject.AddComponent<BTInstanceEnemyBossCommand>();     
        m_waiting3Command                            = gameObject.AddComponent<BTWaitingCommand>();   
        m_stompCommand                                = gameObject.AddComponent<BTStompBossCommand>();   
        m_upDownCrabScissorsCommand         = gameObject.AddComponent<BTUpDownCrabScissorsBossCommand>();  
        m_seaWavyCommand                            = gameObject.AddComponent<BTSeaWavyBossCommand>();      
        m_meteoWithWaterGunCommand         = gameObject.AddComponent<BTMeteoWithWaterGunBossCommand>();
        m_waiting4Command                            = gameObject.AddComponent<BTWaitingCommand>();

        //Conditionの生成
        m_isHealthHalfDownCondition = gameObject.AddComponent<BTIsHealthHalfDownCondition>();
        m_isDyingCondition = gameObject.AddComponent<BTIsDyingBossCondition>();
        m_isAfterDyingCondition = gameObject.AddComponent<BTIsAfterDyingCondition>();

        //Commandの初期化
        m_angryMotionCommand.Initialize(this.gameObject);
        m_playerObserverCommand.Initialize(this.gameObject);
        m_inkSpewCommand.Initialize(this.gameObject);
        m_instanceEnemyCommand.Initialize(this.gameObject);
        m_waitingCommand.Initialize(3);
        m_rotationAttackCommand.Initialize(this.gameObject);
        m_crabScissorsCommand.Initialize(this.gameObject);
        m_momentMaulCommand.Initialize(this.gameObject);
        m_separateContinuousAttackCommand.Initialize(this.gameObject);
        m_fireBallPreventingEscapeCommand.Initialize(this.gameObject);
        m_waiting2Command.Initialize(0.1f);
        m_dyingMotionCommand.Initialize(this.gameObject);
        m_diedCommand.Initialize(this.gameObject);
        m_instanceEnemy2Command.Initialize(this.gameObject);
        m_waiting3Command.Initialize(2);
        m_stompCommand.Initialize(this.gameObject);
        m_upDownCrabScissorsCommand.Initialize(this.gameObject);
        m_seaWavyCommand.Initialize(this.gameObject);
        m_meteoWithWaterGunCommand.Initialize(this.gameObject);
        m_waiting4Command.Initialize(0.1f);

        //Conditionの初期化
        m_isHealthHalfDownCondition.Initialize(this.gameObject);
        m_isDyingCondition.Initialize(this.gameObject);
        m_isAfterDyingCondition.Initialize(this.gameObject);


		//Nodeの生成・初期化・連結
        //前半のBT
        //third1
        m_rotationAttackNode = new BTActionNode(m_rotationAttackCommand);
        m_crabScissorsNode = new BTActionNode(m_crabScissorsCommand);
        m_momentMaulNode = new BTActionNode(m_momentMaulCommand);
        m_separateContinuousAttackNode = new BTActionNode(m_separateContinuousAttackCommand);
        m_fireBallPreventingEscapeNode = new BTActionNode(m_fireBallPreventingEscapeCommand);

        //second1
        m_angryMotionNode = new BTActionNode(m_angryMotionCommand);
        m_playerObserverNode = new BTActionNode(m_playerObserverCommand);
        m_inkSpewNode = new BTActionNode(m_inkSpewCommand);

        //second2
        m_instanceEnemyNode = new BTActionNode(m_instanceEnemyCommand);
        m_waitingNode = new BTActionNode(m_waitingCommand);
        List<BaseBTNode> randomAttack = new List<BaseBTNode>();
        randomAttack.Add(m_rotationAttackNode);
        randomAttack.Add(m_crabScissorsNode);
        randomAttack.Add(m_momentMaulNode);
        randomAttack.Add(m_fireBallPreventingEscapeNode);
        m_randomAttackNode = new BTOnOffSelector(randomAttack);
        m_waiting2Node = new BTActionNode(m_waiting2Command);

        //first1
        m_isHealthHalfDownNode = new BTConditionNotToEnd(m_isHealthHalfDownCondition);
        List<BaseBTNode> halfMotion = new List<BaseBTNode>();
        halfMotion.Add(m_angryMotionNode);
        halfMotion.Add(m_playerObserverNode);
        halfMotion.Add(m_inkSpewNode);
        m_halfMotionNode = new BTSequence(halfMotion);
        List<BaseBTNode> commonAction = new List<BaseBTNode>();
        commonAction.Add(m_instanceEnemyNode);
        commonAction.Add(m_waitingNode);
        commonAction.Add(m_randomAttackNode);
        commonAction.Add(m_waiting2Node);
        m_commonActionNode = new BTSequence(commonAction);

        //root
        m_rootNode = new BTTEandNTEBoolSelector(m_halfMotionNode, m_commonActionNode,
            m_isHealthHalfDownNode);

        //後半のBT
        //fourth
        m_stompNode = new BTActionNode(m_stompCommand);
        m_upDownCrabScissorsNode = new BTActionNode(m_upDownCrabScissorsCommand);   
        m_seaWavyNode = new BTActionNode(m_seaWavyCommand);
        m_meteoWithWaterGunNode = new BTActionNode(m_meteoWithWaterGunCommand);

        //third1
        List<BaseBTNode> newRandomAttack = new List<BaseBTNode>();
        newRandomAttack.Add(m_stompNode);
        m_randomAttackNode2 = new BTOnOffSelector(randomAttack);

        //second1
        m_isAfterDyingNode = new BTConditionToEnd(m_isAfterDyingCondition);
        m_dyingMotionNode = new BTActionNode(m_dyingMotionCommand);
        m_diedNode = new BTActionNode(m_diedCommand);

        //second2
        m_instanceEnemyNode2 = new BTActionNode(m_instanceEnemy2Command);
        m_waiting3Node = new BTActionNode(m_waiting3Command);
        List<BaseBTNode> twoSelection = new List<BaseBTNode>();
        twoSelection.Add(m_randomAttackNode);
        twoSelection.Add(m_randomAttackNode2);
        m_randomTwoSelectionAttack = new BTOnOffSelector(twoSelection);
        m_waiting4Node = new BTActionNode(m_waiting4Command);

        //first1
        m_isDyingNode = new BTConditionNotToEnd(m_isDyingCondition);
        m_afterDyingNode = new BTBoolSelector(m_diedNode, m_dyingMotionNode,
            m_isAfterDyingNode);
        List<BaseBTNode> commonAction2 = new List<BaseBTNode>();
        commonAction2.Add(m_instanceEnemyNode2);
        commonAction2.Add(m_waiting3Node);
        commonAction2.Add(m_randomAttackNode);
        commonAction2.Add(m_waiting4Node);
        m_commonActionNode2 = new BTSequence(commonAction2);

        //root
        m_rootNode2 = new BTTEandNTEBoolSelector(m_afterDyingNode, m_commonActionNode2,
            m_isDyingNode);

        status = GetComponent<BossStatus>();
        movePosition = new Vector3[4];
        lookAtPosition = new Vector3[4];
        isHealthHalfDown = false;
        isPreHealthHalfDown = false;
    }


    public override Vector3[] ActivateBoss(Vector3[] lookAtPos) {
        for (int i = 0; i < movePosition.Length; i++)
            movePosition[i] = Vector3.zero;
        SetInitLookAt();

        //体力が半分を下回りビヘイビアツリーが変わった瞬間
        if (isHealthHalfDown && !isPreHealthHalfDown) {
            m_rootNode.SetFailuer();
        }

		if (!isHealthHalfDown) //体力が半分以下じゃないなら
            m_rootNode.Evaluate();
        else {
            m_rootNode2.Evaluate();
        }

        isPreHealthHalfDown = isHealthHalfDown;

        lookAtPosition.CopyTo(lookAtPos, 0);

        return (Vector3[])movePosition.Clone();
    }


    public void SetInitLookAt() {

        Vector3[] vecfromCenter = new Vector3[4];
        Transform[] foot = status.GetFoots();
        Vector3[] lookAtPos = new Vector3[4];
        for (int i = 0; i < vecfromCenter.Length; i++) {
            vecfromCenter[i] = foot[i].position - status.GetFootCenter().position;
            vecfromCenter[i].y = 0;
            lookAtPos[i] = foot[i].position + vecfromCenter[i];
        }

        lookAtPos.CopyTo(lookAtPosition, 0);
    }


    public override void SetMovePosition(Vector3[] pos) {

        movePosition = pos;
    }


    public override void SetLookAtPos(Vector3[] pos) {

        lookAtPosition = pos;
    }


    public void SetIsHealthHalfDownWithTrue() {

        isHealthHalfDown = true;
    }    
}
