using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー行動状態遷移のベースクラス(それぞれの状態変数の初期化を含む)
public abstract class PlayerStateEntry : MonoBehaviour {
    //Main状態
    protected static PlayerMovingStateMachine moving; //移動状態
    protected static PlayerJumpingStateMachine jumping; //ジャンプ状態
    protected static PlayerAttackingStateMachine attacking; //攻撃状態
    protected static PlayerDamagingStateMachine damaging; //ダメージ状態

    //moving状態
    protected static Waiting waiting; //待機状態
    protected static Running running; //走行状態
    protected static SlideRunning slideRunning; //横移動状態

    //Jumping状態
	protected static JumpingFirst jumpingFirst; //ジャンプ状態(１回目)
	protected static JumpingSecond jumpingSecond; //ジャンプ状態(２回目)
	protected static JumpingWithUpToDown jumpingWithUpToDown; //飛び降り状態
    protected static SlideJumping slideJumping; //横ジャンプ状態
    protected static WallJumping wallJumping; //壁ジャンプ状態
    protected static CliffHolding cliffHolding; //崖捕まり状態

    //Attacking状態
	protected static AttackingFirst attackingFirst; //直接攻撃状態(１回目)
	protected static AttackingSecond attackingSecond; //直接攻撃状態(２回目)
	protected static AttackingThird attackingThird; //直接攻撃状態(３回目)
	protected static AttackingInAirlial attackingInAir; //直接空中攻撃状態

    //Damaging状態
    protected static NockBacking nockBacking; //ノックバック状態
    protected static Dying dying; //死亡状態

    //プレイヤーのコマンド
    protected static WaitingCommand             waitingCommand;
    protected static RunningCommand             runningCommand;
    protected static SlideRunningCommand        slideRunningCommand;
    protected static JumpingFirstCommand        jumpingFirstCommand;
    protected static JumpingSecondCommand       jumpingSecondCommand;
    protected static JumpingWithUpToDownCommand jumpingWithUpToDownCommand;
    protected static SlideJumpingCommand        slideJumpingCommand;
    protected static WallJumpingCommand         wallJumpingCommand;
    protected static CliffHoldingCommand        cliffHoldingCommand;
    protected static AttackingFirstCommand      attackingFirstCommand;
    protected static AttackingSecondCommand     attackingSecondCommand;
    protected static AttackingThirdCommand      attackingThirdCommand;
    protected static AttackingInAirlialCommand  attackingInAirCommand;
    protected static NockBackingCommand         nockBackingCommand;
    protected static DyingCommand               dyingCommand;

    private static int instanceCount = 0;

    //行動状態変数を初期化
    public void Initialize(GameObject usingObj) {
        PlayerCommandBase[] command = new PlayerCommandBase[15];
        waitingCommand = usingObj.AddComponent<WaitingCommand>();
        runningCommand = usingObj.AddComponent<RunningCommand>();
        slideRunningCommand = usingObj.AddComponent<SlideRunningCommand>();
        jumpingFirstCommand = usingObj.AddComponent<JumpingFirstCommand>();
        jumpingSecondCommand = usingObj.AddComponent<JumpingSecondCommand>();
        jumpingWithUpToDownCommand = usingObj.AddComponent<JumpingWithUpToDownCommand>();
        slideJumpingCommand = usingObj.AddComponent<SlideJumpingCommand>();
        wallJumpingCommand = usingObj.AddComponent<WallJumpingCommand>();
        cliffHoldingCommand = usingObj.AddComponent<CliffHoldingCommand>();
        attackingFirstCommand = usingObj.AddComponent<AttackingFirstCommand>();
        attackingSecondCommand = usingObj.AddComponent<AttackingSecondCommand>();
        attackingThirdCommand = usingObj.AddComponent<AttackingThirdCommand>();
        attackingInAirCommand = usingObj.AddComponent<AttackingInAirlialCommand>();
        nockBackingCommand = usingObj.AddComponent<NockBackingCommand>();
        dyingCommand = usingObj.AddComponent<DyingCommand>();
        command[0] = waitingCommand;
        command[1] = runningCommand;
        command[2] = slideRunningCommand;
        command[3] = jumpingFirstCommand;
        command[4] = jumpingSecondCommand;
        command[5] = jumpingWithUpToDownCommand;
        command[6] = slideJumpingCommand;
        command[7] = wallJumpingCommand;
        command[8] = cliffHoldingCommand;
        command[9] = attackingFirstCommand;
        command[10] = attackingSecondCommand;
        command[11] = attackingThirdCommand;
        command[12] = attackingInAirCommand;
        command[13] = nockBackingCommand;
        command[14] = dyingCommand;
        for (int i = 0; i < command.Length; i++) {
            command[i].Initialize(usingObj);
        }

        PlayerStateEntry[] move = new PlayerStateEntry[3];
        waiting = usingObj.AddComponent<Waiting>();
        running = usingObj.AddComponent<Running>();
        slideRunning = usingObj.AddComponent<SlideRunning>();
        move[0] = waiting;
        move[1] = running;
        move[2] = slideRunning;
        for (int i = 0; i < move.Length; i++) {
            move[i].SubInitialize(usingObj);
        }

        PlayerStateEntry[] jump = new PlayerStateEntry[6];
        jumpingFirst = usingObj.AddComponent<JumpingFirst>();
        jumpingSecond = usingObj.AddComponent<JumpingSecond>();
        jumpingWithUpToDown = usingObj.AddComponent<JumpingWithUpToDown>();
        slideJumping = usingObj.AddComponent<SlideJumping>();
        wallJumping = usingObj.AddComponent<WallJumping>();
        cliffHolding = usingObj.AddComponent<CliffHolding>();
        jump[0] = jumpingFirst;
        jump[1] = jumpingSecond;
        jump[2] = jumpingWithUpToDown;
        jump[3] = slideJumping;
        jump[4] = wallJumping;
        jump[5] = cliffHolding;
        for (int i = 0; i < jump.Length; i++) {
            jump[i].SubInitialize(usingObj);
        }

        PlayerStateEntry[] damage = new PlayerStateEntry[2];
        nockBacking = usingObj.AddComponent<NockBacking>();
        dying = usingObj.AddComponent<Dying>();
        damage[0] = nockBacking;
        damage[1] = dying;
        for (int i = 0; i < damage.Length; i++) {
            damage[i].SubInitialize(usingObj);
        }

        PlayerStateEntry[] attack = new PlayerStateEntry[4];
        attackingFirst = usingObj.AddComponent<AttackingFirst>();
        attackingSecond = usingObj.AddComponent<AttackingSecond>(); ;
        attackingThird = usingObj.AddComponent<AttackingThird>(); ;
        attackingInAir = usingObj.AddComponent<AttackingInAirlial>(); ;
        attack[0] = attackingFirst;
        attack[1] = attackingSecond;
        attack[2] = attackingThird;
        attack[3] = attackingInAir;
        for (int i = 0; i < attack.Length; i++) {
            attack[i].SubInitialize(usingObj);
        }

        PlayerStateEntry[] main = new PlayerStateEntry[4];
        moving = usingObj.AddComponent<PlayerMovingStateMachine>();
        jumping = usingObj.AddComponent<PlayerJumpingStateMachine>();
        attacking = usingObj.AddComponent<PlayerAttackingStateMachine>();
        damaging = usingObj.AddComponent<PlayerDamagingStateMachine>();
        main[0] = moving;
        main[1] = jumping;
        main[2] = attacking;
        main[3] = damaging;
        for (int i = 0; i < main.Length; i++) {
            main[i].SubInitialize(usingObj);
        }

        usingObj.GetComponent<PlayerStateMachine>().SubInitialize(usingObj);
    }

    //初期化関数
    protected virtual void SubInitialize(GameObject usingObj) { }

    public abstract Vector3 Activate(ref Vector3 lookAtPos); //実行処理関数
    public virtual void Enter(PlayerStateEntry upperState) { } //その状態になった瞬間
    public virtual void Exit(PlayerStateEntry upperState) { } //その状態が終了した瞬間
    public virtual void ChangeState(PlayerStateEntry nextState) { } //状態遷移関数
    public virtual void IsChanging(PlayerStateEntry upperState) { } //状態遷移判定関数
    public virtual PlayerStateEntry GetPreState() { return null; } //前の状態取得関数
}
