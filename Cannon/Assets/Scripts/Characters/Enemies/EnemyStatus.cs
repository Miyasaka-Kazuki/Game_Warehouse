using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
    Pursuiter,
    Shooter,
}

//エネミーの状態クラス
public class EnemyStatus : BaseStatus {

    [SerializeField] private EnemyType type; //エネミータイプ
    public Transform player; //プレイヤー位置
    public bool useMuteki = false; //無敵か

    private const int attack = 5;
    private const float wanderSpeed = 1;
    private const float visualForPlayerDistance = 130; //プレイヤーを発見する距離
    private const float closeToPlayerDistance = 1.5f; //プレイヤーに十分近づいている距離
    private const float awayFromPlayerDistance = 10f; //プレイヤーから十分離れている距離

	private bool dead;
    private bool damaged;
    private bool flownDamaged;
    private float superArmorValue;
    public float superArmorThresholdValue = 3;
    private Animator enemyAnim;
    private Collider touchedWeaponCol;
    private BoxCollider attackCol;
    private SkinnedMeshRenderer[] eneMesh;
    private int damageCount;

    private int playerMask;
    private const float wallJumpedDistance = 1.5f;
    private float curJumpDistance;
    private float preJumpDistance;
    private CharacterController charCon;
    private bool notUseWeapon;
    private Vector3 awayDestination;

    //アフォーダンストリガーはステータスが持っておいて、
    //適宜コマンドがゴニョニョする
    private List<AffordanceTrigger> afforTriObj;
    private AffordanceTrigger regAfforTriObj;
    public struct AffordanceVolume { //0~1
        public float safetyVolume;
    }
    private AffordanceVolume afforVol;
    public Property[] canUseProperty;

    private bool preDamaged;
    private float mutekiTimer;
    private float mutekiThresholdTime = 2;

	//初期化処理
    public void Start() {
        player = GameObject.FindWithTag("Player").transform;
        health = maxHealth;
        horizontalInput = 0;
        verticalInput = 0;
        damaged = false;
        flownDamaged = false;
        superArmorValue = 0;
        dead = false;
        touchedWeaponCol = null;
        attackCol = GetComponentInChildren<BoxCollider>();
        playerMask = LayerMask.NameToLayer("Player");
        playerMask = ~(playerMask);
        curJumpDistance = 0;
        preJumpDistance = 0;
        enemyAnim = GetComponent<Animator>();
        charCon = GetComponent<CharacterController>();
        eneMesh = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

		damageCount = 0;
        muteki = false;
        notUseWeapon = false;

        afforTriObj = new List<AffordanceTrigger>();
        afforVol.safetyVolume = 0;

        preDamaged = false;
        mutekiTimer = 0;
    }

	//更新関数
    public void Update() {
        //スーパーアーマー値は時間経過で減っていく
        superArmorValue -= Time.deltaTime;
        if (superArmorValue <= 0)
            superArmorValue = 0;

        if (dead) {
            flownDamaged = true;
        }

        RegisterAfforTri();
        ComputeAwayDestination();

        //無敵時間
        if (useMuteki) {
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
        }
    }

    //体力はBTで減らす
    //damagedとflownDamagedはBTでfalseにする
    void OnTriggerEnter(Collider hit) {
        //終わってから３秒くらい無敵になる
        if (flownDamaged || dead || muteki ||
            //downToUpが1の時にまたdamagedがtrueになって瞬間的にノックバックしてバグるから
            enemyAnim.GetCurrentAnimatorStateInfo(0).IsName("DownToUp"))
            return;

        if (hit.gameObject.tag == "Weapon") {
            touchedWeaponCol = hit;
            damaged = true;

            BulletBase bulletBase = hit.gameObject.GetComponent<BulletBase>();
            superArmorValue += bulletBase.GetWeaponValue();
            health -= bulletBase.GetWeaponValue();
        } else if (hit.gameObject.tag == "PlayerAttack1" || hit.gameObject.tag == "PlayerAttack2" ||
            hit.gameObject.tag == "PlayerAttack3") {
            PlayerStatus ps = hit.gameObject.GetComponentInParent<PlayerStatus>();
            if (ps.GetAttackCol1().enabled) {
                superArmorValue += ps.GetAttackFirstValue();
                health -= ps.GetAttackFirstValue();
            } else if (ps.GetAttackCol2().enabled){
                superArmorValue += ps.GetAttackSecondValue();
                health -= ps.GetAttackFirstValue();
            } else if (ps.GetAttackCol3().enabled) {
                superArmorValue += ps.GetAttackThirdValue();
                health -= ps.GetAttackFirstValue();
            }
            damaged = true;
        }
        //スーパーアーマー値を超えたらtrueにする
        if (superArmorValue >= superArmorThresholdValue)
            flownDamaged = true;
        if (health <= 0) {
            dead = true;
            flownDamaged = true;
        }
    }


    //ダメージ表現する関数(ここでdamagedをfalseにしている)
    public void DamageExpression() {
        damageCount++;
        for (int i = 0; i < eneMesh.Length; i++)
            eneMesh[i].enabled = false;

        if (damageCount >= 6) {
            damaged = false;
            damageCount = 0;
            for (int i = 0; i < eneMesh.Length; i++)
                eneMesh[i].enabled = true;
        }
    }


    public void AddAfforTriObserver(AffordanceTrigger at) {
        if (afforTriObj.Contains(at)) return;
        afforTriObj.Add(at);
    }
		
    public void RemoveAfforTriObserver(AffordanceTrigger at) {
        if (!afforTriObj.Contains(at)) return;
        afforTriObj.Remove(at);
    }

    public void AddAffordanceVolume(string type, float volume) {
        switch (type) {
            case "Safety":
                afforVol.safetyVolume += volume;
                if (afforVol.safetyVolume > 1) afforVol.safetyVolume = 1;
                break;
        }
    }

    public void SubAffordanceVolume(string type, float volume) {
        switch (type) {
            case "Safety":
                afforVol.safetyVolume -= volume;
                if (afforVol.safetyVolume < 0) afforVol.safetyVolume = 0;
                break;
        }
    }

    public float GetAffordanceVolume(string type) {
        switch (type) {
            case "Safety":
                return afforVol.safetyVolume;
        }

        return -1;
    }

    //リストにして優先度の比較関数でソートして、
    //優先度が一番高いものだけでリストを作って、
    //距離でソートして一番近いものを選ぶ？
    public void RegisterAfforTri(){
        if (regAfforTriObj != null) return;

        SortedList<float, AffordanceTrigger> dis_affor_map = new SortedList<float, AffordanceTrigger>();
        foreach (AffordanceTrigger obj in afforTriObj) {
            bool exist = false;
            for (int i = 0; i < canUseProperty.Length; i++) {
                exist = obj.IsToMatchProperty(canUseProperty[i]);
                if (exist) break;
            }
            if (!exist) continue; //属性が違うなら
            bool isUsed = obj.IsUsed();
            if (isUsed) continue; //使われているなら

            float distance = (obj.GetDestination() - transform.position).magnitude;
            dis_affor_map[distance] = obj;
        }

        //1個もないなら終了
        if (dis_affor_map.Count == 0)
            return;

        regAfforTriObj = dis_affor_map.Values[0];
        regAfforTriObj.SetIsUsed(true);
        regAfforTriObj.SetEnemy(gameObject);
    }

    public void SetAfforTriUsedForFalse() {

        if (regAfforTriObj == null) return;
        regAfforTriObj.SetIsUsed(false);
        regAfforTriObj = null;
    }

    public void ComputeAwayDestination() {
        if (regAfforTriObj != null) {
            awayDestination = regAfforTriObj.GetDestination();
            return;
        }

        Vector3 vecFromPlayer = transform.position - player.position;
        float awayDis = 0;
        if (vecFromPlayer.magnitude < GetAwayFromPlayerDistance())
            awayDis = GetAwayFromPlayerDistance() - vecFromPlayer.magnitude;
        awayDestination = transform.position + vecFromPlayer.normalized * awayDis;
    }

    public Vector3 GetAwayDestination() {
        return awayDestination;
    }
		
    public float GetWanderSpeed() { return wanderSpeed; }
    public float GetVisualForPlayerDistance() { return visualForPlayerDistance; }
    public float GetCloseToPlayerDistance() { return closeToPlayerDistance; }
    public float GetAwayFromPlayerDistance() { return awayFromPlayerDistance; }
    public bool GetDamaged() { return damaged; }
    public bool GetFlownDamaged() { return flownDamaged; }
    public bool GetIsDying() { return dead; }
    public Animator GetAnimator() { return enemyAnim; }
    public Collider GetTouchedWeaponCol() { return touchedWeaponCol; }
    public BoxCollider GetAttackCol() { return attackCol; }
    public void SetFlownDamagedWithFalse() { flownDamaged = false; }
    public void ResetSuperArmorValue() { superArmorValue = 0; }
    public bool GetNotUseWeapon() { return notUseWeapon; }
    public void SetNotUseWeapon(bool nUW) { notUseWeapon = nUW; }
    public void SetEnemyType(EnemyType t) { type = t; }
    public EnemyType GetEnemyType() { return type; }
}
