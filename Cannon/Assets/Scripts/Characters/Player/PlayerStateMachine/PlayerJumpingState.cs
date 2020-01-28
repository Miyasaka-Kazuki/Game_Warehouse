using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//２段ジャンプの１回目状態クラス
public class JumpingFirst : PlayerStateEntry {
    private Animator anim;
    private PlayerStatus status;
    private float jumpingNextTimer;
    private const float thresholdTime = 0.15f;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }
		
    public override void Enter(PlayerStateEntry upperState) {
        anim.SetBool("JumpToTop", true);
        jumpingNextTimer = 0;
        jumpingFirstCommand.Enter();
        GameDirector.Instance().audioDirector.PlaySE("jump", 3);
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        jumpingNextTimer += Time.fixedDeltaTime;
        Vector3 movePosition = Vector3.zero;
        movePosition = jumpingFirstCommand.Activate(ref lookAtPos);
        return movePosition;
    }
		
    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("JumpToTop", false);
    }

    public override void IsChanging(PlayerStateEntry upperState) {
        if (status.GetJumpButtonDown() && jumpingNextTimer > thresholdTime)
            upperState.ChangeState(jumpingSecond);
        else if (status.GetWallJumped()) {
            upperState.ChangeState(wallJumping);
        } else if (status.GetCliffHeld())
            upperState.ChangeState(cliffHolding);
        else if (status.GetMaxJumped() && jumpingNextTimer > thresholdTime) {
            upperState.ChangeState(jumpingWithUpToDown);
        }
    }
}

//２段ジャンプの２回目状態クラス
public class JumpingSecond : PlayerStateEntry {
    private Animator anim;
    private PlayerStatus status;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        anim.SetBool("JumpSecond", true);
        jumpingSecondCommand.Enter();
        GameDirector.Instance().audioDirector.PlaySE("jump", 3);
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = jumpingSecondCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("JumpSecond", false);
    }
		
    public override void IsChanging(PlayerStateEntry upperState) {
        if (status.GetWallJumped())
            upperState.ChangeState(wallJumping);
        else if (status.GetCliffHeld())
            upperState.ChangeState(cliffHolding);
    }
}

//飛び降り状態クラス
public class JumpingWithUpToDown : PlayerStateEntry {
	private Animator anim;
	private PlayerStatus status;

	protected override void SubInitialize(GameObject usingObj) {
		anim = usingObj.GetComponent<Animator>();
		status = usingObj.GetComponent<PlayerStatus>();
	}
		
	public override void Enter(PlayerStateEntry upperState) {
		anim.SetBool("JumpOfTop", true);
		jumpingWithUpToDownCommand.Enter();
	}
		
	public override Vector3 Activate(ref Vector3 lookAtPos) {
		Vector3 movePosition = Vector3.zero;
		movePosition = jumpingWithUpToDownCommand.Activate(ref lookAtPos);
		return movePosition;
	}
		
	public override void Exit(PlayerStateEntry upperState) {
		anim.SetBool("JumpOfTop", false);
	}

	public override void IsChanging(PlayerStateEntry upperState) {
		if (status.GetJumpButtonDown())
			upperState.ChangeState(jumpingSecond);
		else if (status.GetWallJumped())
			upperState.ChangeState(wallJumping);
		else if (status.GetCliffHeld())
			upperState.ChangeState(cliffHolding);
	}
}

//横移動ジャンプ状態クラス
public class SlideJumping : PlayerStateEntry {
    private Animator anim;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        slideJumpingCommand.Enter();
    }
		
    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = slideJumpingCommand.Activate(ref lookAtPos);
        return movePosition;
    }
}
	
//壁ジャンプ状態クラス
public class WallJumping : PlayerStateEntry {
    private Animator anim;
    private PlayerStatus status;
    private float firstTimer; //すぐに飛び降りジャンプに移行させないため
    private const float thresholdTime = 0.6f;
    private bool preWallJumped; //このステートの間に次の壁に
    private bool curWallJumped; //ついた時に使う
    private bool wallJumped; //壁ジャンプしたかどうか

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }
		
    public override void Enter(PlayerStateEntry upperState) {
        firstTimer = 0;
        curWallJumped = status.GetWallJumped();
        preWallJumped = curWallJumped;
        wallJumped = false;
        wallJumpingCommand.Enter();
        GameDirector.Instance().audioDirector.PlaySE("jump", 3);
    }
		
    public override Vector3 Activate(ref Vector3 lookAtPos) {
        preWallJumped = curWallJumped;
        curWallJumped = status.GetWallJumped();
        if (status.GetJumpButtonDown())
            wallJumped = true;
        if (wallJumped)
            firstTimer += Time.fixedDeltaTime;

        Vector3 movePosition = Vector3.zero;
        movePosition = wallJumpingCommand.Activate(ref lookAtPos);
        return movePosition;
    }
		
    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("WallJumpRight", false);
        anim.SetBool("WallJumpLeft", false);
        anim.SetBool("JumpOfTop", true);
        wallJumpingCommand.Exit();
    }
		
    public override void IsChanging(PlayerStateEntry upperState) {
        if (status.GetCliffHeld())
            upperState.ChangeState(cliffHolding);
        //次の壁についた瞬間
        else if (!preWallJumped && curWallJumped) {
            upperState.ChangeState(wallJumping);
        }
        //ジャンプが最大距離に達していて、ジャンプした後の時間が閾値を超えたら
        else if (status.GetMaxJumped() &&
              firstTimer > thresholdTime) {
            upperState.ChangeState(jumpingWithUpToDown);
        } else if (status.GetIsNearForWallJumped())
            upperState.ChangeState(jumpingWithUpToDown);
    }
}

//崖捕まり状態クラス
public class CliffHolding : PlayerStateEntry {
    private Animator anim;
    private PlayerStatus status;
    private bool preCliffHeld;
    private bool curCliffHeld;
    private bool jumpOff;
    private bool jumpOn;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
        preCliffHeld = false;
        curCliffHeld = false;
        jumpOn = false;
    }
		
    public override void Enter(PlayerStateEntry upperState) {
        anim.SetBool("CliffHold", true);
        cliffHoldingCommand.Enter();
        jumpOff = false;
        jumpOn = false;
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        preCliffHeld = curCliffHeld;
        curCliffHeld = status.GetCliffHeld();
        jumpOn = cliffHoldingCommand.GetJumpOn();
        if (status.GetJumpOffWithCliffHolding())
            jumpOff = true;
  
        Vector3 movePosition = Vector3.zero;
        movePosition = cliffHoldingCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("CliffHold", false);
        anim.SetBool("CliffHoldUp", false);
        cliffHoldingCommand.Exit();
    }

    public override void IsChanging(PlayerStateEntry upperState) {
        if (jumpOff) //下に落ちるなら
            upperState.ChangeState(jumpingWithUpToDown);
		else if (curCliffHeld && !preCliffHeld) //このステートの中で崖つかまりした瞬間
            upperState.ChangeState(cliffHolding);
		else if (!curCliffHeld && preCliffHeld && !jumpOn) //離れた瞬間
            upperState.ChangeState(jumpingWithUpToDown);
    }
}