using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アイテムクラス
public class item : MonoBehaviour {
    public InventoryItem type;

	//プレイヤー接触開始関数
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            PlayerInventory inv = other.GetComponent<PlayerInventory>();
            inv.GetItem(type);
            if (type == InventoryItem.HEALTH_UP_ITEM)
                GameDirector.Instance().ui_Director.InstanceUI("HP", other.gameObject);
            GameDirector.Instance().ui_Director.InstanceUI("GetBulletAmmoUI", gameObject);
            GameDirector.Instance().audioDirector.PlaySE("itemGet", 3);
            Destroy(gameObject);
        }
    }
}
