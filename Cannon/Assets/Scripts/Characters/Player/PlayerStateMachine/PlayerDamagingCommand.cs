using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃を受けた時のノックバックコマンド
public class NockBackingCommand : PlayerCommandBase {
    private PlayerStatus status;
    private float flownSpeed;
    private float upSpeed;
    private Vector3 flownDirection;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<PlayerStatus>();
        flownDirection = Vector3.zero;
    }

    public override void Enter() {
        flownSpeed = status.GetNockBackSpeed() * Mathf.Cos(Mathf.PI / 3.0f);
        upSpeed = status.GetNockBackSpeed() * Mathf.Sin(Mathf.PI / 3.0f);
        flownDirection = status.GetFlownDirectionWithDamage();
    }

	public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;

        //ノックバック
        movePosition += flownDirection * flownSpeed * Time.fixedDeltaTime;
        float velocity = upSpeed *Time.fixedDeltaTime - status.GetGravity() * Time.fixedDeltaTime;
        upSpeed = velocity;
        movePosition += transform.up * velocity;

        lookAtPos = transform.position + -flownDirection;
        return movePosition;
    }
}

//死亡コマンド
public class DyingCommand : PlayerCommandBase {
    public override Vector3 Activate(ref Vector3 lookAtPos) {
        return Vector3.zero;
    }
}
