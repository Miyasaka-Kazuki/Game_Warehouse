using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アイテムタイプ
public enum InventoryItem {
    DEBUG_ITEM,
    COIN,
    HUGE_COIN,
    WEAPON_GUN,
    WEAPON_HIGHHODAIN,
    WEAPON_HODAIN,
    HEALTH_UP_ITEM,
    MAX_HEALTH_UP_ITEM,
    FOUR_COMPLETE_ITEM,
    COUNT_NUM_ITEMS,
}

//プレイヤーのアイテム取得処理クラス
public class PlayerInventory : MonoBehaviour {
    private int[] items;
    private PlayerStatus playerStat;
    private WeaponStatus wStatus;

	void Awake () {
        items = new int[(int)InventoryItem.COUNT_NUM_ITEMS];
        for (int i = 0; i < (int)InventoryItem.COUNT_NUM_ITEMS; i++) {
            items[i] = 0;
        }
	}

    void Start() {
        playerStat = GetComponent<PlayerStatus>();
        wStatus = GetComponentInChildren<WeaponStatus>();
    }

    public void GetItem(InventoryItem item) {
        items[(int)item]++;
        switch (item) {
            case InventoryItem.HEALTH_UP_ITEM:
                playerStat.AddHealth(1);
                break;
            case InventoryItem.MAX_HEALTH_UP_ITEM:
                playerStat.AddHealth(playerStat.GetMaxHealth());
                break;

            case InventoryItem.WEAPON_GUN:
                wStatus.AddBulletNum(20, 0);
                break;

            case InventoryItem.WEAPON_HODAIN:
                wStatus.AddBulletNum(3, 0);
                break;

            case InventoryItem.WEAPON_HIGHHODAIN:
                wStatus.AddBulletNum(3, 1);
                break;

            case InventoryItem.FOUR_COMPLETE_ITEM:
                if (items[(int)item] >= 4) {
                    //鍵を開ける
                }
                break;
        }
   }

	public int GetItemCount(InventoryItem item) {
	    return items[(int)item];
	} 
}
