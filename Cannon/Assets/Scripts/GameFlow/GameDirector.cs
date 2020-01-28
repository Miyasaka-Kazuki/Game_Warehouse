using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;

//ゲーム全体の管理
public class GameDirector : MonoBehaviour {

    public ToGameOver toGameOver; //ゲームオーバー処理クラス

    public UIDirector ui_Director; //UIの管理クラス
    public PlayerDirector playerDirector; //プレイヤーの管理クラス
	public BossDirector bossDirector; //ボスの管理(ボスステージ以外はnull)クラス
    public AnimDirector animDirector; //アニメーションの管理クラス
	public AudioDirector audioDirector; //音の管理クラス

	private List<EnemyDirector> enemyDirectors; //敵の管理リスト
    private List<OtherDirector> otherDirectors; //ギミック、アイテム、武器などの動作管理リスト
    private CameraDirector cameraDirector; //カメラの管理

	//動作停止オブジェクト
    private CharacterFreeze charaFreeze; 
    private CameraFreeze cameraFreeze;
    private OtherFreeze otherFreeze;
	private List<GameObject> freezeList; //動作停止するオブジェクトリスト
	private int freezeCount; //動作停止しているオブジェクトの数
	private bool isWeaponFreezed; //武器を動作停止するか
	private bool isPaused; //ポーズしているか

    private WeaponStateMachine weaponStateMachine; //武器の状態管理
    private bool canDoDirector; //管理できる状態か
	private static GameDirector gameDirector; //シングルトン


	void Start () {
        if (ui_Director == null) Debug.Log("ui_Directorがnull");
        if (playerDirector == null) Debug.Log("player_Directorがnull");
        if (bossDirector == null) Debug.Log("boss_Directorがnull");
        if (animDirector == null) Debug.Log("anim_Directorがnull");

        enemyDirectors = new List<EnemyDirector>();
        otherDirectors = new List<OtherDirector>();
        cameraDirector = Camera.main.GetComponent<CameraDirector>();
        charaFreeze = GetComponent<CharacterFreeze>();
        cameraFreeze = GetComponent<CameraFreeze>();
        otherFreeze = GetComponent<OtherFreeze>();
        audioDirector = Camera.main.GetComponent<AudioDirector>();
        gameDirector = this;

        canDoDirector = true;
        isWeaponFreezed = false;
        weaponStateMachine = playerDirector.GetComponentInChildren<WeaponStateMachine>();

        freezeCount = 0;
        freezeList = new List<GameObject>();
        isPaused = false;

		Application.targetFrameRate = 60;
	}

    private void Update() {
        string bgm_name = null;
        switch (Application.loadedLevelName) {
            case "title":
                bgm_name = "title";
                audioDirector.PlayBGM(bgm_name);
                break;
            case "FirstBoss":
                bgm_name = "boss2";
                audioDirector.PlayBGM(bgm_name);
                break;
            case "game over":
                bgm_name = "game over";
                audioDirector.PlaySE(bgm_name);
                break;
            case "Clear":
                audioDirector.PlaySE(bgm_name);
                break;
            case "MainStage":
                bgm_name = "main4";
                audioDirector.PlayBGM(bgm_name);
                break;
        }
			
        //ポーズボタンを押したらオールフリーズ
		bool buttonDown = Input.GetButtonDown("Pause");
		if (buttonDown && !isPaused) {
            AllFreeze(gameObject);
            ui_Director.InstanceUI("PauseUI", gameObject);
            ui_Director.InstanceUI("HP", playerDirector.gameObject);
            isPaused = true;
		} else if (isPaused && !ui_Director.HasInstance("PauseUI")) {
            isPaused = false;
            ReleaseAllFreeze(gameObject);
            ui_Director.DestroyUI("HP");
        }
    }

    //全部のDirectorとWeaponStateMachineをここから呼ぶようにする
    private void FixedUpdate() {
        if (canDoDirector) {

            AdjustDirector();

            //PlayerDirector
            playerDirector.Activate();

            //EnemyDirector
            for (int i = 0; i < enemyDirectors.Count; i++)
                enemyDirectors[i].Activate();

            //BossDirector
            if (bossDirector != null)
                bossDirector.Activate();

            //CameraDirector
            cameraDirector.Activate();

            //OtherDirector
            for (int i = 0; i < otherDirectors.Count; i++)
                otherDirectors[i].Activate();

            //UI操作はフリーズしても出来るようにするため
            weaponStateMachine.SwitchWeapon();
        } else {
            animDirector.Activate();
        }
    }

    //ムービースタート関数
    public void ApplyAnim(GameObject applyObj, string parentAnim_name, List<string> childAnim_name, List<float> childAnim_second,
        FreezeObj freezeObj = FreezeObj.all, bool canSkip_ = false) {

        if (freezeObj == FreezeObj.all)
            canDoDirector = false;
        else if (freezeObj == FreezeObj.applyOnly) {
            BaseDirector director = applyObj.GetComponent<BaseDirector>();
            switch (director.GetDirectorType()) {
                case DirectorType.Character:
                    director.AddObserver(charaFreeze);
                    break;
                case DirectorType.Camera:
                    director.AddObserver(cameraFreeze);
                    break;
                case DirectorType.Other:
                    director.AddObserver(otherFreeze);
                    break;
            }
        }

        ui_Director.TempDestroy();
        if (canSkip_)
            ui_Director.InstanceUI("MovieUI", gameObject);
        animDirector.ApplyAnim(applyObj, parentAnim_name, childAnim_name, childAnim_second, canSkip_);
    }

	//ムービー終了関数
    public void FinishAnim() {
        ui_Director.DestroyUI("MovieUI");
        ui_Director.ReturnInstance();
        canDoDirector = true;
    }

	//全てのオブジェクトの停止関数
	public void AllFreeze(GameObject callObj) {
		if (freezeList.Contains(callObj)) return;
		freezeList.Add(callObj);
		//参照カウント
		freezeCount++;

		if (freezeCount > 1) return;

		AdjustDirector();

		playerDirector.AddObserver(charaFreeze);
		foreach (EnemyDirector ene in enemyDirectors) {
			ene.AddObserver(charaFreeze);
		}
		if (bossDirector != null) {
			bossDirector.BossFreeze();
		}

		foreach (OtherDirector other in otherDirectors) {
			other.AddObserver(otherFreeze);
		}

		CameraFreeze();

		isWeaponFreezed = true;
	}

	//全てのオブジェクトの停止解除関数
	public void ReleaseAllFreeze(GameObject callObj) {
		if (!freezeList.Contains(callObj)) return;
		freezeCount--;
		freezeList.Remove(callObj);

		if (freezeCount > 0) return;

		AdjustDirector();

		playerDirector.RemoveObserver(charaFreeze);
		foreach (EnemyDirector ene in enemyDirectors) {
			ene.RemoveObserver(charaFreeze);
		}
		if (bossDirector != null)
			bossDirector.ReleaseBossFreeze();

		foreach (OtherDirector other in otherDirectors) {
			other.RemoveObserver(otherFreeze);
		}

		ReleaseCameraFreeze();

		isWeaponFreezed = false;
	}

	//プレイヤーの停止関数
	public void PlayerFreeze() {
		playerDirector.AddObserver(charaFreeze);
	}

	//プレイヤーの停止解除関数
	public void ReleasePlayerFreeze() {
		playerDirector.RemoveObserver(charaFreeze);
	}

	//カメラの停止関数
	public void CameraFreeze() {
		Camera.main.GetComponent<CameraDirector>().AddObserver(cameraFreeze);
	}

	//カメラの停止解除関数
	public void ReleaseCameraFreeze() {
		cameraDirector.RemoveObserver(cameraFreeze);
	}

	//エネミー管理リストとその他管理リストの更新関数
	public void AdjustDirector() {
		for (int i = enemyDirectors.Count - 1; i >= 0; i--) {
			if (enemyDirectors[i] == null)
				enemyDirectors.RemoveAt(i);
		}

		for (int i = otherDirectors.Count - 1; i >= 0; i--) {
			if (otherDirectors[i] == null)
				otherDirectors.RemoveAt(i);
		}
	}

	//エネミー管理リストの追加関数
    public void AddEnemyDirector(EnemyDirector ene) {
        if (enemyDirectors.Contains(ene)) return;
        enemyDirectors.Add(ene);
    }

	//エネミー管理リストの削除関数
    public void RemoveEnemyDirector(EnemyDirector ene) {
        if (!enemyDirectors.Contains(ene)) return;
        enemyDirectors.Remove(ene);
    }

	//その他管理リストの追加関数
    public void AddOtherDirector(OtherDirector other) {
        if (otherDirectors.Contains(other)) return;
        otherDirectors.Add(other);
    }

	//その他管理リストの削除関数
    public void RemoveOtherDirector(OtherDirector other) {
        if (!otherDirectors.Contains(other)) return;
        otherDirectors.Remove(other);
    }
				
	//GameDirectorのシングルトンオブジェクト関数
    public static GameDirector Instance() {
        return gameDirector;
    }
}
