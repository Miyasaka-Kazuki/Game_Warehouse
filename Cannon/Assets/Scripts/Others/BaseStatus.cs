using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトステータス管理クラスのベースクラス
public class BaseStatus : MonoBehaviour {

    public float moveSpeed = 5; //移動量
    protected const float jumpPower = 600; //ジャンプ量
    public int maxHealth = 10; //最大HP
	protected float horizontalInput; //プレイヤー操作受付変数(水平)
	protected float verticalInput; //プレイヤー操作受付変数(垂直)
    protected const float mass = 1; //体重
    protected int health; //現在のHP
    protected const float gravity = 30f; //体感重力
    protected bool muteki; //無敵か

	//それぞれの変数の取得関数
    public float GetHorizontalInput() { return horizontalInput; }
    public float GetVerticalInput() { return verticalInput; }
    public float GetMoveSpeed() { return moveSpeed; }
    public float GetJumpPower() { return jumpPower; }
    public float GetMass() { return mass; }
    public int GetHealth() { return health; }
    public int GetMaxHealth() { return maxHealth; }
    public float GetGravity() { return gravity; }
    public void SetMuteki(bool m) { muteki = m; }
}
