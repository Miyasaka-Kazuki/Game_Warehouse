using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーの状態クラス
public class PlayerStatus : BaseStatus {
	//ボタン判定
	private bool runButton;
	private bool jumpButtonDown;
	private bool jumpButton;
	private bool attackButtonDown;
	private bool weaponAttackButton;
	private bool slideButton;

	//無敵時間処理
    private float mutekiTimer;
    private float mutekiThresholdTime = 3;

	//直接攻撃のダメージ量
    private const int   attackFirst      = 2;
    private const int   attackSecond     = 3;
    private const int   attackThird      = 5;
//    private const int   jumpAttack       = 5;

	private Vector3 flownDirection; //攻撃で吹き飛ぶ方向ベクトル

	public float wallJumpPower    = 800; //壁ジャンプのパワー
	private const float wallJumpedDistance = 1; //壁ジャンプと判定される高さ
    private const float nockBackSpeed = 5; //ノックバックの時のスピード


	//状態判定変数
	private CharacterController charCon; //プレイヤーの当たり判定

	private bool dead; //死亡状態か
	private bool damaged; //ダメージを受けた状態か
	private bool preDamaged; //前フレームにダメージを受けた状態か
	private bool wallJumped; //壁ジャンプした状態か
	private bool cliffHeld; //崖捕まりした状態か

	private bool isNearForWallJumped; //壁ジャンプ時、地面から距離があるか
	private bool wallJumpedThanTwo; //２回目以降の壁ジャンプ待機か
	private bool wallJumpedWithEndOne;

	private bool jumpOffWithCliffHolding; //崖つかまりの判定ブール値
    private bool jumpOnWithCliffHolding; //崖つかまりの判定ブール値
    private float moveWithCliffHolding; //崖つかまりの判定ブール値
    private bool jumpedOff; //崖から飛び降りたかどうか

	private bool maxJumped; //最大までジャンプしたか
    private bool isGrounded; //地面に接地してるか
    private bool preGrounded; //前フレームに地面に接地してるか

    private RaycastHit wallRayCast; //壁判定
    private bool firstDamage; //ダメージを受けたか

	private struct AttackCollider { //直接攻撃の当たり判定
		public BoxCollider aC1;
		public BoxCollider aC2;
		public BoxCollider aC3;
	}
	private AttackCollider attackCol;
	private Collider enemyAttackColl; //エネミーの攻撃判定

    private int playerMask; //プレイヤーと衝突するオブジェクト値
    private float curJumpDistance; //地面との距離
    private float preJumpDistance; //前フレームの地面との距離

	private Transform downWallStartRayPoint; //壁判定ブール値
	private Transform downWallEndRayPoint; //壁判定ブール値
	private Transform upWallStartRayPoint; //壁判定ブール値
	private Transform upWallEndRayPoint; //壁判定ブール値
    private Transform floorStartRayPoint; //崖飛び降り判定ブール値
    private Transform floorEndRayPoint; //崖飛び降り判定ブール値

	private struct GroundCheck { //接地判定
        public Transform gCStartPointC;
        public Transform gCEndPointC;
        public Transform gCStartPointF;
        public Transform gCEndPointF;
        public Transform gCStartPointB;
        public Transform gCEndPointB;
        public Transform gCStartPointL;
        public Transform gCEndPointL;
        public Transform gCStartPointR;
        public Transform gCEndPointR;
    }
    private GroundCheck groundCheck;

	//初期化
    public void Start() {
        if (PlayerPrefs.HasKey("HP")) {
            health = PlayerPrefs.GetInt("HP");
            PlayerPrefs.DeleteKey("HP");
        } else {
            health = maxHealth;
        }
        horizontalInput = 0;
        verticalInput = 0;
        runButton = false;
        jumpButtonDown = false;
        jumpButton = false;
        attackButtonDown = false;
        weaponAttackButton = false;
        slideButton = false;
        damaged = false;
        preDamaged = false;
        dead = false;
        wallJumped = false;
        isNearForWallJumped = false;
        cliffHeld = false;
        jumpOffWithCliffHolding = false;
        jumpOnWithCliffHolding = false;
        moveWithCliffHolding = 0;
        jumpedOff = false;
        maxJumped = false;
        isGrounded = false;
        preGrounded = false;
        enemyAttackColl = null;
        flownDirection = Vector3.zero;
        playerMask = LayerMask.NameToLayer("Player");
        playerMask = ~(playerMask);
        curJumpDistance = 0;
        preJumpDistance = 0;
        downWallStartRayPoint = transform.Find("DownWallStartRayPoint");
        downWallEndRayPoint   = transform.Find("DownWallEndRayPoint");
        upWallStartRayPoint   = transform.Find("UpWallStartRayPoint");
        upWallEndRayPoint     = transform.Find("UpWallEndRayPoint");
        floorStartRayPoint    = transform.Find("FloorOffStartRayPoint");
        floorEndRayPoint      = transform.Find("FloorOffEndRayPoint");
        Transform gC          = transform.Find("GroundCheck");
        groundCheck.gCStartPointC = gC.Find("GCStartPointC");
        groundCheck.gCEndPointC   = gC.Find("GCEndPointC"  );
        groundCheck.gCStartPointF = gC.Find("GCStartPointF");
        groundCheck.gCEndPointF   = gC.Find("GCEndPointF"  );
        groundCheck.gCStartPointB = gC.Find("GCStartPointB");
        groundCheck.gCEndPointB   = gC.Find("GCEndPointB"  );
        groundCheck.gCStartPointL = gC.Find("GCStartPointL");
        groundCheck.gCEndPointL   = gC.Find("GCEndPointL"  );
        groundCheck.gCStartPointR = gC.Find("GCStartPointR");
        groundCheck.gCEndPointR   = gC.Find("GCEndPointR"  );
        Transform aC = transform.Find("AttackCollider");
        attackCol.aC1 = aC.Find("Attack1").GetComponent<BoxCollider>();
        attackCol.aC2 = aC.Find("Attack2").GetComponent<BoxCollider>();
        attackCol.aC3 = aC.Find("Attack3").GetComponent<BoxCollider>();
        charCon = GetComponent<CharacterController>();
        wallJumpedThanTwo = false;
        wallJumpedWithEndOne = false;
        muteki = false;
        mutekiTimer = 0;
    }
		
	//更新関数
    public void Update() {
        if (preDamaged && !damaged)
            muteki = true;
        if (muteki) { //ダメージを受けた後の無敵時間
            mutekiTimer += Time.deltaTime;
            if (mutekiTimer > mutekiThresholdTime) {
                muteki = false;
                mutekiTimer = 0;
            }
        }
        preDamaged = damaged;

        if (health <= 0)
            dead = true;
    }

	//物理更新関数
    public void FixedUpdate() {

        IsMaxJumped();
        HandleInput();
        IsGrounded();
        IsWallHit();
        IsJumpingOffHit();
        IsBoolWithCliffHolding();

        //２回目の壁ジャンプかの判断
        if (wallJumpedWithEndOne) {
            if (isGrounded) { //地面についたら初めから
                wallJumpedThanTwo = false;
                wallJumpedWithEndOne = false;
            }
        }

        firstDamage = false;
    }

	//接地判定関数
    void IsGrounded() {

        preGrounded = isGrounded;
        isGrounded = false;
        bool[] isGroundeds = new bool[5];
        //LineCastはマスクに指定したもののみと衝突判定
        isGroundeds[0] = Physics.Linecast(groundCheck.gCStartPointC.position, groundCheck.gCEndPointC.position, playerMask, QueryTriggerInteraction.Ignore);
        isGroundeds[1] = Physics.Linecast(groundCheck.gCStartPointF.position, groundCheck.gCEndPointF.position, playerMask, QueryTriggerInteraction.Ignore);
        isGroundeds[2] = Physics.Linecast(groundCheck.gCStartPointB.position, groundCheck.gCEndPointB.position, playerMask, QueryTriggerInteraction.Ignore);
        isGroundeds[3] = Physics.Linecast(groundCheck.gCStartPointL.position, groundCheck.gCEndPointL.position, playerMask, QueryTriggerInteraction.Ignore);
        isGroundeds[4] = Physics.Linecast(groundCheck.gCStartPointR.position, groundCheck.gCEndPointR.position, playerMask, QueryTriggerInteraction.Ignore);
        foreach(bool isGouondLocal in isGroundeds) {
            if (isGouondLocal) {
                isGrounded = true;
                break;
            }
        }
    }

	//プレイヤー操作受付関数
    void HandleInput() {

        runButton = false;
        jumpButtonDown   = false;
        jumpButton = false;
        attackButtonDown = false;
        weaponAttackButton = false;
        slideButton = false;
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if ((Input.GetAxis("Horizontal") != 0) || (Input.GetAxis("Vertical") != 0))
            runButton = true;
        else
            runButton = false;
        if (Input.GetButtonDown("Jump")) jumpButtonDown = true;
        if (Input.GetButton("Jump")) jumpButton = true;
        if (Input.GetButtonDown("Attack")) attackButtonDown = true;
        if (Input.GetButton("Weapon")) weaponAttackButton = true;
        if (Input.GetButton("Slide")) slideButton = true;
    }

	//壁際判定関数
    void IsWallHit() {

        wallJumped = false;
        cliffHeld = false;
        isNearForWallJumped = false;
        if (charCon.isGrounded) return;

        RaycastHit downHit;
        Vector3 startPosition = downWallStartRayPoint.position;
        float maxDistance = (downWallEndRayPoint.position - downWallStartRayPoint.position).magnitude;
        bool IsHitOnDownPoint = Physics.Raycast(startPosition, transform.forward,
                        out downHit, maxDistance, playerMask, QueryTriggerInteraction.Ignore);
        float distanceToGround;
        if (IsHitOnDownPoint) {

            RaycastHit upHit;
            maxDistance = (upWallEndRayPoint.position - upWallStartRayPoint.position).magnitude;
            bool IsHitOnUpPoint = Physics.Raycast(upWallStartRayPoint.position, transform.forward,
                out upHit, maxDistance, playerMask, QueryTriggerInteraction.Ignore);

            if (IsHitOnUpPoint) {

                Physics.Raycast(groundCheck.gCStartPointC.position, transform.up*-1,
                    out upHit, Mathf.Infinity, playerMask, QueryTriggerInteraction.Ignore);
                /*float */distanceToGround = (groundCheck.gCStartPointC.position - upHit.point).magnitude;
                //地面から十分離れているか
                if (distanceToGround > wallJumpedDistance)
                    wallJumped = true;
            } else {

                cliffHeld = true;
            }

            wallRayCast = downHit;
            if (wallRayCast.transform.gameObject.tag == "Weapon" ||
                wallRayCast.transform.gameObject.tag == "EnemyWeapon") {
                wallJumped = false;
                cliffHeld = false;
            }
        }

        //壁ジャンプできる距離かどうか
        RaycastHit upH;
        Physics.Raycast(groundCheck.gCStartPointC.position, transform.up * -1,
    out upH, Mathf.Infinity, playerMask, QueryTriggerInteraction.Ignore);

		distanceToGround = (groundCheck.gCStartPointC.position - upH.point).magnitude;

		//地面から十分離れているか
        if (distanceToGround < wallJumpedDistance)
            isNearForWallJumped = true;
    }

	//崖捕まり判定関数
    void IsBoolWithCliffHolding() {

        jumpOnWithCliffHolding = false;
        jumpOffWithCliffHolding = false;
        if (!cliffHeld) return;

        float ver = Input.GetAxis("Vertical");
        if (ver < 0) jumpOffWithCliffHolding = true;
        moveWithCliffHolding = Input.GetAxis("Horizontal");
    }

	//ジャンプした瞬間判定関数
    void IsJumpingOffHit() {

        jumpedOff = false;
        if (!(!isGrounded && preGrounded)) return; //地面から離れた瞬間じゃなければ
        bool IsHitOnFloor = Physics.Linecast(floorStartRayPoint.position, floorEndRayPoint.position,
                                playerMask, QueryTriggerInteraction.Ignore);
        if (!IsHitOnFloor) jumpedOff = true;
    }

	//最大ジャンプ判定関数
    public void IsMaxJumped() {

        maxJumped = false;
        if (isGrounded) return;
        if (preGrounded) { //ジャンプした瞬間の値が初期値
            curJumpDistance = transform.position.y;
        }

        preJumpDistance = curJumpDistance; //前フレームの地面からの高さ
        curJumpDistance = transform.position.y; //今フレームの地面からの高さ
        //地面からの高さが前フレームより低くなったら
        if (curJumpDistance < preJumpDistance) {
            maxJumped = true;
        }
    }

	//物理接触判定関数
    public void OnControllerColliderHit(ControllerColliderHit hit) {

        if (damaged || muteki) return;
        if (hit.gameObject.tag == "Enemy" || hit.gameObject.tag == "EnemyWeapon") {
            damaged = true;
            enemyAttackColl = hit.collider;
            SubHealth(1);

            muteki = true;
            firstDamage = true;
            GameDirector.Instance().ui_Director.InstanceUI("HP", gameObject);
        }
    }

	//非物理接触判定関数
    public void OnTriggerEnter(Collider other) {

        if (damaged || muteki) return;
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "EnemyWeapon" ||
            other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            damaged = true;
            enemyAttackColl = other;
            flownDirection = transform.position - enemyAttackColl.transform.position;
            flownDirection.y = 0;
            flownDirection = flownDirection.normalized;
            SubHealth(1);

			if (health <= 0) dead = true;

            muteki = true;
            firstDamage = true;
            GameDirector.Instance().ui_Director.InstanceUI("HP", gameObject);
        }
    }

	//HP追加関数
    public void AddHealth(int h) {

        health += h;
        if (health > maxHealth) health = maxHealth;
    }

	//HP削減関数
    public void SubHealth(int h) {

        health -= h;
        if (health < 0) health = 0;
    }


    public float      GetWallJumpPower()    { return wallJumpPower; }
    public bool       GetRunButton()        { return runButton; }
    public bool       GetJumpButtonDown()   { return jumpButtonDown; }
    public bool       GetJumpButton()       { return jumpButton; }
    public bool       GetAttackButtonDown() { return attackButtonDown; }
    public bool       GetWeaponAttackButton() { return weaponAttackButton; }
    public bool       GetSlideButton()      { return slideButton; }
    public bool       GetDamaged()          { return damaged; }
    public bool       GetWallJumped() { return wallJumped; }
    public bool       GetIsNearForWallJumped() { return isNearForWallJumped; }
    public bool       GetCliffHeld()        { return cliffHeld; }
    public bool       GetJumpOffWithCliffHolding() { return jumpOffWithCliffHolding; }
    public bool       GetJumpOnWithCliffHolding()  { return jumpOnWithCliffHolding; }
    public float      GetMoveWithCliffHolding()    { return moveWithCliffHolding; }
    public bool       GetJumpingOff()       { return jumpedOff; }
    public bool       GetMaxJumped()        { return maxJumped; }
    public bool       GetIsGrounded()       { return isGrounded; }
    public int GetAttackFirstValue() { return attackFirst; }
    public int GetAttackSecondValue() { return attackSecond; }
    public int GetAttackThirdValue() { return attackThird; }
    public BoxCollider GetAttackCol1() { return attackCol.aC1; }
    public BoxCollider GetAttackCol2() { return attackCol.aC2; }
    public BoxCollider GetAttackCol3() { return attackCol.aC3; }
    public Vector3 GetFlownDirectionWithDamage() { return flownDirection; }
    public RaycastHit GetWallRayCast()      { return wallRayCast; }
    public bool GetWallJumpedThanTwo() { return wallJumpedThanTwo; }
    public float      GetNockBackSpeed() { return nockBackSpeed; }
    public void  WallJumpedWithEndOne() { wallJumpedWithEndOne = true; wallJumpedThanTwo = true; }
    public void       SetDamaged(bool d)    { damaged = d; }
    public void SetDead(bool d) { dead = d; }
    public bool GetDead() { return dead; }
    public bool GetFirstDamege() { return firstDamage; }
}
