using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//２段ジャンプの１回目コマンド
public class JumpingFirstCommand : PlayerCommandBase {
    private Transform activeCamera;
    private PlayerStatus status;
    private float acceleration;

    public void Start() {
        activeCamera = Camera.main.transform;
        status = GetComponent<PlayerStatus>();
    }

    public override void Enter() {
        acceleration = status.GetJumpPower() / status.GetMass();
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        //入力取得
        float x = status.GetHorizontalInput();
        float z = status.GetVerticalInput();
        //カメラの向きにプレイヤーを動かす
        //カメラの方向からX-Z平面の単位ベクトルを取得(各成分を乗算して単位ベクトルにする)
        Vector3 cameraForward = Vector3.Scale(
            activeCamera.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = x * activeCamera.right + z * cameraForward;
        moveDirection.y = 0;

        movePosition += moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime;

        movePosition.y += (acceleration * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        acceleration -= status.GetGravity();

        lookAtPos = transform.position + moveDirection;
        return movePosition;
    }
}

//2段ジャンプの２回目コマンド
public class JumpingSecondCommand : PlayerCommandBase {
	private Transform activeCamera;
	private PlayerStatus status;
	private float acceleration;
	private float notFallTimer;
	private const float fallenTime = 0.3f;

	public void Start() {
		activeCamera = Camera.main.transform;
		status = GetComponent<PlayerStatus>();
	}

	public override void Enter() {
		acceleration = 0;
		notFallTimer = 0;
	}

	public override Vector3 Activate(ref Vector3 lookAtPos) {
		Vector3 movePosition = Vector3.zero;
		Vector3 moveDirection = Vector3.zero;

		//入力取得
		float x = status.GetHorizontalInput();
		float z = status.GetVerticalInput();
		//カメラの向きにプレイヤーを動かす
		//カメラの方向からX-Z平面の単位ベクトルを取得(各成分を乗算して単位ベクトルにする)
		Vector3 cameraForward = Vector3.Scale(
			activeCamera.forward, new Vector3(1, 0, 1)).normalized;
		moveDirection = x * activeCamera.right + z * cameraForward;
		moveDirection.y = 0;

		movePosition += moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime;
		notFallTimer += Time.fixedDeltaTime;
		if (notFallTimer < fallenTime) return movePosition;

		movePosition.y += (acceleration * Time.fixedDeltaTime) * Time.fixedDeltaTime;
		acceleration -= status.GetGravity();

		lookAtPos = transform.position + moveDirection;
		return movePosition;
	}
}

//飛び降りコマンド
public class JumpingWithUpToDownCommand : PlayerCommandBase {
    private Transform activeCamera;
    private PlayerStatus status;
    private float acceleration;

    public override void Initialize(GameObject usingObj) {
        activeCamera = Camera.main.transform;
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter() {
        acceleration = 0;
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        //入力取得
        float x = status.GetHorizontalInput();
        float z = status.GetVerticalInput();
        //カメラの向きにプレイヤーを動かす
        //カメラの方向からX-Z平面の単位ベクトルを取得(各成分を乗算して単位ベクトルにする)
        Vector3 cameraForward = Vector3.Scale(
            activeCamera.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = x * activeCamera.right + z * cameraForward;
        moveDirection.y = 0;

        movePosition += moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime;

        movePosition.y += (acceleration * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        acceleration -= status.GetGravity();

        lookAtPos = transform.position + moveDirection;
        return movePosition;
    }

    public override void Exit() {
    }
}
	
//横移動ジャンプコマンド
public class SlideJumpingCommand : PlayerCommandBase {
    private Transform activeCamera;
    private PlayerStatus status;
    private float acceleration;

    public void Start() {
        activeCamera = Camera.main.transform;
        status = GetComponent<PlayerStatus>();
    }

    public override void Enter() {
        acceleration = status.GetJumpPower() / status.GetMass();
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        //入力取得
        float x = status.GetHorizontalInput();
        moveDirection = x * activeCamera.right;

        movePosition += moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime;

        movePosition.y += (acceleration * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        acceleration -= status.GetGravity();

        Vector3 lookAtDirection = activeCamera.forward;
        lookAtDirection.y = 0;

        lookAtPos = transform.position + lookAtDirection;

        return movePosition;
    }
}

//壁ジャンプコマンド
public class WallJumpingCommand : PlayerCommandBase {
	private PlayerStatus status;
	private float accelerationX;
	private float accelerationY;
	private bool blewOff;
	private RaycastHit wallRayCast;
	private const float gravityCoefficient = 3;
	private Animator anim;
	private float preAngle;
	private Vector3 lookAtPosition;

	public void Start() {
		status = GetComponent<PlayerStatus>();
		float acceleration = status.GetWallJumpPower() / status.GetMass();
		accelerationX = acceleration * Mathf.Cos(Mathf.PI/3);
		accelerationY = acceleration * Mathf.Sin(Mathf.PI / 3);
		anim = GetComponent<Animator>();
		preAngle = 0;
	}

	public override void Enter() {
		blewOff = false;
		accelerationY = accelerationX * Mathf.Tan(Mathf.PI / 3);
		wallRayCast = status.GetWallRayCast();

		//プレイヤーの角度を壁に合わせる
		//カメラとぶつかったカメラとの位置でモーションと回る角度を変える
		if (status.GetWallJumpedThanTwo()) {
			preAngle = -preAngle;
		} else {
			Vector3 toPlayer = transform.position - wallRayCast.point;
			toPlayer.y = 0;
			Vector3 enterDir = Vector3.Cross(wallRayCast.normal, toPlayer);
			if (enterDir.y >= 0) {
				preAngle = 90;
			} else {
				preAngle = -90;
			}
		}
		if (preAngle == 90)
			anim.SetBool("WallJumpRight", true);
		else if (preAngle == -90)
			anim.SetBool("WallJumpLeft", true);
		anim.SetBool("JumpOfTop", false);
		anim.SetBool("JumpToTop", false);
	}

	public override Vector3 Activate(ref Vector3 lookAtPos) {

		lookAtPos = transform.position + Quaternion.Euler(0, preAngle, 0) * wallRayCast.normal * -1;

		//        Debug.Log("WallJumping");
		Vector3 movePosition = Vector3.zero;
		Vector3 moveDirection = Vector3.zero;

		if (status.GetJumpButtonDown()) blewOff = true;
		if (!blewOff) {//壁からずり降りるなら
			movePosition.y -= gravityCoefficient *
				status.GetGravity() * Time.fixedDeltaTime * Time.fixedDeltaTime;

			return movePosition;

		}
		//壁ジャンプするなら
		if (preAngle == 90) {
			anim.SetBool("WallJumpRight", false);
		} else if (preAngle == -90)
			anim.SetBool("WallJumpLeft", false);
		anim.SetBool("JumpOfTop", true);

		//吹っ飛ばす処理
		float accToPos = Time.fixedDeltaTime * Time.fixedDeltaTime;
		movePosition += wallRayCast.normal * accelerationX * accToPos;
		movePosition.y += accelerationY * accToPos;
		accelerationY -= status.GetGravity();

		moveDirection = wallRayCast.normal;

		lookAtPos = transform.position + moveDirection;

		return movePosition;

	}

	public override void Exit() {
		if (blewOff) {
			status.WallJumpedWithEndOne();
		}
	}
}

//崖つかまりコマンド
public class CliffHoldingCommand : PlayerCommandBase {
    private PlayerStatus status;
    private RaycastHit wallRayCast;
    private bool jumpOn;
    private float accelerationX;
    private float accelerationY;
    private const float moveCoefficient = 0.05f;
    private const float jumpCoefficient = 0.5f;
    private const float jumpOnPower = 450;
    private bool isCliffHold;
    private Vector3 rightHandPosition;
    private Vector3 leftHandPosition;
    private Animator anim;
    private CharacterController charaCon;

    public void Start() {
        status = GetComponent<PlayerStatus>();
        anim = GetComponent<Animator>();
        charaCon = GetComponent<CharacterController>();
        float acceleration = jumpOnPower / status.GetMass();
        accelerationX = acceleration * Mathf.Cos(60 * Mathf.Deg2Rad);
        accelerationY = acceleration * Mathf.Sin(60 * Mathf.Deg2Rad);
        leftHandPosition = Vector3.zero;
        rightHandPosition = Vector3.zero;
        isCliffHold = false;
    }

    public override void Enter() {
        //プレイヤーの角度を壁に合わせる
        jumpOn = false;
        wallRayCast = status.GetWallRayCast();
        accelerationY = accelerationX * Mathf.Tan(60 * Mathf.Deg2Rad);
        isCliffHold = true;
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {

        lookAtPos = transform.position + wallRayCast.normal * -1;

        SetPosition(); //IKPosition更新

        Vector3 movePosition = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        //上を入力したら前上方向に吹っ飛ばし地面判定をtrueにする
        if (status.GetJumpButtonDown()) {
            anim.SetBool("CliffHold", false);
            anim.SetBool("CliffHoldUp", true);
            jumpOn = true;
        }
        if (jumpOn) {
            float accToPos = Time.fixedDeltaTime * Time.fixedDeltaTime;
            movePosition.y += accelerationY * accToPos;
            accelerationY -= status.GetGravity() * jumpCoefficient;

            if (status.GetMaxJumped()) {
                float moveXZ = accelerationX * accToPos;
                movePosition += wallRayCast.normal * -1 * moveXZ;
            }
            return movePosition;
        }

        //下を入力したら下後ろにずらして崖つかまり検知をfalseにする
        if (status.GetJumpOffWithCliffHolding()) {
            anim.SetBool("CliffHold", false);
            movePosition = wallRayCast.normal * 0.3f;
            movePosition.y -= 0.5f;

            lookAtPos = transform.position + wallRayCast.normal;
            return movePosition;
        }

        float lateralMove = status.GetMoveWithCliffHolding(); //横移動
        movePosition = Quaternion.Euler(0, -90, 0) * wallRayCast.normal * 
            lateralMove * moveCoefficient;
        return movePosition;
    }

    public override void Exit() {
        isCliffHold = false;
    }

	//キャラクター手足ポジション処理関数
    public void SetPosition() {
        wallRayCast = status.GetWallRayCast();
        Vector3 wallTopPoint = wallRayCast.point;
        wallTopPoint.y = wallRayCast.collider.bounds.max.y + Vector3.up.y; //一番上の場所
        Vector3 leftDir = Quaternion.Euler(0, -90, 0) * wallRayCast.normal;
        Vector3 rightDir = Quaternion.Euler(0, 90, 0) * wallRayCast.normal;
        leftHandPosition = wallTopPoint + leftDir * charaCon.radius;
        rightHandPosition = wallTopPoint + rightDir * charaCon.radius;
    }

	//アニメーション処理関数
    void OnAnimatorIK(int layerIndex) {
        if (!isCliffHold) return;
        if (!jumpOn) { //崖つかまり中は完全なIK
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(180, 45, 0)); //180,45,0
            anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(-180, 45, 0)); //180,45,0
        } else { //崖昇り中はIKを小さくする
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(180, 45, 0)); //180,45,0
            anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(-180, 45, 0)); //180,45,0
        }

    }

    public bool GetJumpOn() {
        return jumpOn;
    }
}

