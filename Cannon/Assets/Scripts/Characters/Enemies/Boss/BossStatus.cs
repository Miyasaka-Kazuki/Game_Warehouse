using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスの状態管理クラス
public class BossStatus : ParentTrigger {
	//ボスの手足の位置
    [SerializeField] private Transform leftFoot1; //左足
    [SerializeField] private Transform leftFoot2; //左足
    [SerializeField] private Transform rightFoot1; //左足
    [SerializeField] private Transform rightFoot2; //左足
    [SerializeField] private Transform centerPos; //中央座標
    [SerializeField] private Transform body; //体
    [SerializeField] private Transform player; //プレイヤー座標

	[SerializeField] private GameObject inkFloor; //滑らせる床
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attack = 5;
    [SerializeField] private int distanceFromCenter = 3; 
    [SerializeField] private float superArmorThresholdValue = (1 << 30);
    [SerializeField] private Material inkMaterial;

	private int health; //HP
    private bool dead; //死亡か
    private bool damaged; //胴体がダメージを受けたら全部が赤くなる
    private bool muteki; //無敵か
    private bool canBePersuit; //プレイヤーを追いかけるか
    private bool IsInMotion; //モーション中か
    private int damageCount; //ダメージカウント
    private Renderer[] eneMesh;
    private Color[] baseColor;
    private Transform[] foots; //left1,right1,left2,right2
    private float deathTimer; //死亡時間

    //初期化処理
    void Start() {
        health = maxHealth;
        dead = false;
        damaged = false;
        muteki = false;
        IsInMotion = false;
        canBePersuit = true;
        damageCount = 0;

        //eneMeshには本体を入れる
        eneMesh = new Renderer[5];
        eneMesh[0] = leftFoot1.GetComponentInChildren<Renderer>();
        eneMesh[1] = rightFoot1.GetComponentInChildren<Renderer>();
        eneMesh[2] = leftFoot2.GetComponentInChildren<Renderer>();
        eneMesh[3] = rightFoot2.GetComponentInChildren<Renderer>();
        eneMesh[4] = body.GetComponentInChildren<Renderer>();
        baseColor = new Color[5];
        for (int i = 0; i < eneMesh.Length; i++)
            baseColor[i] = eneMesh[i].material.color;
        foots = new Transform[4];
        foots[0] = leftFoot1;
        foots[1] = rightFoot1;
        foots[2] = leftFoot2;
        foots[3] = rightFoot2;
        body.position = centerPos.position;

        inkFloor.SetActive(false);
        deathTimer = 0;
    }

	//更新処理
    void Update () {
        //Start関数でやるとUIDirectorのListが初期化される前に呼んじゃうから
        GameDirector.Instance().ui_Director.InstanceUI("BossHPBer", gameObject);

        //中心は常にプレイヤーのほうを向く
        Vector3 dir = player.position - centerPos.position;
        dir.y = 0;
        centerPos.LookAt(centerPos.position + dir);

        if (dead) {
            GameDirector.Instance().ui_Director.DestroyUI("BossHPBer");
            deathTimer += Time.deltaTime;
            if (deathTimer > 3) {
                Application.LoadLevel("Clear");
            }
        }
    }

    //flownDamagedはBTでfalseにする
    void OnTriggerEnter(Collider hit) {
        TriggerOnParent(hit);
    }


    public override void TriggerOnParent(Collider hit) {
        //終わってから３秒くらい無敵になる
        if (/*flownDamaged ||*/ dead || muteki) return;

        if (hit.gameObject.tag == "Weapon") {
            damaged = true;

            BulletBase bulletBase = hit.gameObject.GetComponent<BulletBase>();
            health -= bulletBase.GetWeaponValue();
        } else if (hit.gameObject.tag == "PlayerAttack1" || hit.gameObject.tag == "PlayerAttack2" ||
            hit.gameObject.tag == "PlayerAttack3") {
            PlayerStatus ps = hit.gameObject.GetComponentInParent<PlayerStatus>();
            if (ps.GetAttackCol1().enabled) {
                health -= ps.GetAttackFirstValue();
            } else if (ps.GetAttackCol2().enabled) {
                health -= ps.GetAttackFirstValue();
            } else if (ps.GetAttackCol3().enabled) {
                health -= ps.GetAttackFirstValue();
            }
            damaged = true;
        }
        if (health <= 0) {
            dead = true;
            muteki = true;
        }
    }

    //ダメージ表現関数(ここでdamagedをfalseにしている)
    public void DamageExpression() {
        float increaseRedValue = 0.2f;
        for (int i = 0; i < eneMesh.Length; i++) {
            Color curColorValue = eneMesh[i].material.color;
            curColorValue.g -= increaseRedValue;
            curColorValue.b -= increaseRedValue;
            if (curColorValue.g <= 0.3f) {
                increaseRedValue = -increaseRedValue;
            } else if (curColorValue.g >= 1) {
                increaseRedValue = -increaseRedValue;

                //元に戻す
                for (int x = 0; x < eneMesh.Length; x++) {
                    eneMesh[x].material.color = baseColor[x];
                }
                damaged = false;
                break;
            } else {
                eneMesh[i].material.color = curColorValue;
            }
        }
    }

	//取得関数
    public Transform GetPlayer() { return player; }
    public Transform GetBody() { return body; }
    public int GetHealth() { return health; }
    public int GetMaxHealth() { return maxHealth; }
    public int GetDistanceFromCenter() { return distanceFromCenter; }
    public bool GetDamaged() { return damaged; }
    public bool GetIsDying() { return dead; }
    public Transform[] GetFoots() { return foots; }
    public Transform GetFootCenter() { return centerPos; }
    public void SetCanBePersuit(bool cbp) { canBePersuit = cbp; }
    public bool GetCanbePersuit() { return canBePersuit; }
    public void SetMuteki(bool m) { muteki = m; }
    public void SetIsInMotion(bool m) { IsInMotion = m; }
    public bool GetIsInMotion() { return IsInMotion; }
    public GameObject GetInkFloor() { return inkFloor; }
    public Material GetInkMaterial() { return inkMaterial; }
}
