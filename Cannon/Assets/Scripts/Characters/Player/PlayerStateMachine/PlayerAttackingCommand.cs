using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//直接攻撃の１回目コマンド
public class AttackingFirstCommand : PlayerCommandBase {
    PlayerStatus status; //プレイヤーステータス

    public void Start() {
        status = GetComponent<PlayerStatus>();
        status.GetAttackCol1().enabled = false;
    }

	public override void Enter() {
        status.GetAttackCol1().enabled = true;
    }

	public override Vector3 Activate(ref Vector3 lookAtPos) {
        return Vector3.zero;
    }

	public override void Exit() {
        status.GetAttackCol1().enabled = false;
    }
}

//直接攻撃の２回目コマンド
public class AttackingSecondCommand : PlayerCommandBase {
    PlayerStatus status;

    public void Start() {
        status = GetComponent<PlayerStatus>();
        status.GetAttackCol2().enabled = false;
    }

	public override void Enter() {
        status.GetAttackCol2().enabled = true;
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        return Vector3.zero;
    }

    public override void Exit() {
        status.GetAttackCol2().enabled = false;
    }
}

//直接攻撃の３回目コマンド
public class AttackingThirdCommand : PlayerCommandBase {
    PlayerStatus status;

    public void Start() {
        status = GetComponent<PlayerStatus>();
        status.GetAttackCol3().enabled = false;
    }

	public override void Enter() {
        status.GetAttackCol3().enabled = true;
    }

	public override Vector3 Activate(ref Vector3 lookAtPos) {
        return Vector3.zero;
    }

	public override void Exit() {
        status.GetAttackCol3().enabled = false;
    }
}

//空中攻撃コマンド
public class AttackingInAirlialCommand : PlayerCommandBase {
    PlayerStatus status;

    public void Start() {
        status = GetComponent<PlayerStatus>();
        status.GetAttackCol1().enabled = false;
    }

	public override void Enter() {
        status.GetAttackCol1().enabled = true;
    }

	public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        moveDirection =  (transform.up * -1).normalized;

        movePosition += moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime;

        lookAtPos = transform.position + transform.forward;
        return movePosition;
    }
    public override void Exit() {
        status.GetAttackCol1().enabled = false;
    }
}

//武器攻撃コマンド
public class WeaponAttackingCommand : PlayerCommandBase {
    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Debug.Log("Weapon");
        return Vector3.zero;
    }
}
