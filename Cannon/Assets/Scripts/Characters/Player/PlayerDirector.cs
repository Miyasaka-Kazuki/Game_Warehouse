using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー全体管理クラス
public class PlayerDirector : CharacterDirector {

    private PlayerStateEntry stateMachine; //行動の状態変数
	private PlayerStatus status; //ステータス変数
	private CharacterController charCon; //移動管理
	private Animator anim; //アニメーション管理
	private bool initialized = false;

	private WeaponStateMachine weaponStateMachine; //武器の状態変数
	private bool nUW; //武器が使える状態か(notUseWeapon)

	private SlideMoveCamera obCamera; //カメラの視点変更オブジェクト
    private Vector3 lookAtPosition; //カメラの視点方向ベクトル
    private bool useWarp; //カメラをワープ移動させるか

	//初期化関数
    void Start () {
        type = DirectorType.Character;
        stateMachine = GetComponent<PlayerStateMachine>();
        weaponStateMachine = GetComponentInChildren<WeaponStateMachine>();
        charCon = GetComponent<CharacterController>();
        status = GetComponent<PlayerStatus>();
        baseObserver = new List<BaseObserver>();
        anim = GetComponent<Animator>();
        nUW = false;
        initialized = false;
        obCamera = GetComponent<SlideMoveCamera>();
        useWarp = false;
    }

	//更新関数
    public void Activate () {
        //ゲーム全体の初期化後の初期化処理
        if (!initialized) {
            initialized = true;
            stateMachine.Initialize(gameObject);
            weaponStateMachine.Initialize(gameObject);
            GameDirector.Instance().ui_Director.InstanceUI("HP", gameObject);
        }

        lookAtPosition = transform.position + transform.forward;
        Vector3 movePosition = stateMachine.Activate(ref lookAtPosition);

        //ギミックなどによる移動制限
        movePosition = OtherMovePosition(movePosition);

        //スライド移動用のカメラにする
        if (status.GetSlideButton()) {
            Camera.main.GetComponent<CameraDirector>().AddObserver(obCamera);
            Vector3 vecCameraPlaneForward = Camera.main.transform.forward;
            vecCameraPlaneForward.y = 0;

            lookAtPosition = transform.position + vecCameraPlaneForward;
        } else {
            Camera.main.GetComponent<CameraDirector>().RemoveObserver(obCamera);
        }

        //武器の状態更新
        if (!nUW && !status.GetDead())
            weaponStateMachine.Activate();

		//HPが０になったらゲームオーバー
        if (status.GetDead()) {
            GameDirector.Instance().toGameOver.toGameOver();
        }
			
		//カメラ視点更新
        transform.LookAt(lookAtPosition); 

		//プレイヤー移動更新
        if (useWarp)
            transform.position = movePosition;
        else
            charCon.Move(movePosition);
   }


	//他オブジェクトによるプレイヤーの状態を制限する関数
    private Vector3 OtherMovePosition(Vector3 movePosition) {
        //他オブジェクトにより変更されるプレイヤーの移動、状態変数
        Vector3 otherMove = Vector3.zero;
        Vector3 otherLookAt = transform.position + transform.forward;
        bool notUseCharacterMove = false; //FSMの移動を使わないかどうか
        bool notUseWeapon = false; //FSMの移動を使わないかどうか
        bool notUseCharaLookAt = false;
        bool isInMuteki = false;
        bool useWarpInLocal = false;
        bool isFreezed = false;

		//他オブジェクト処理を逐次実行
        foreach (BaseObserver co in baseObserver) {
            bool anyNUPM = false;
            bool anyNUW = false;
            bool anyNUCLA = false;
            bool anyMuteki = false;
            bool anyWarp = false;
            
            Vector3 otherM = co.ActivateCharaObserver(ref anyNUPM, ref anyNUW, 
                ref anyNUCLA, ref anyMuteki, ref otherLookAt, ref anyWarp, 
                true, movePosition, anim);

            otherMove += otherM;
            if (anyNUPM) notUseCharacterMove = true;
            if (anyNUW) notUseWeapon = true;
            if (anyNUCLA) notUseCharaLookAt = true;
            if (anyMuteki) isInMuteki = true;
            if (anyWarp) useWarpInLocal = true;

            if (otherM == Vector3.zero) isFreezed = true;
        }

        //プレイヤー移動を上書きするか
        if (notUseCharacterMove) movePosition = otherMove;
        else movePosition += otherMove;

		//カメラの視点を上書きするか
        if (notUseCharaLookAt)
            lookAtPosition = otherLookAt;

		useWarp = useWarpInLocal;
		nUW = notUseWeapon;

        //無敵にする
        if (isInMuteki) status.SetMuteki(true);
        if (isFreezed) movePosition = Vector3.zero;

        return movePosition;
    }
}
