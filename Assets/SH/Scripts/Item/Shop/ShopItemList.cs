using System.Collections.Generic;
using UnityEngine;

public class ShopItemList : MonoBehaviour
{
    [SerializeField] List<GameObject> ShopItemSlots = new List<GameObject>(); // 상점 슬롯 
    [SerializeField] List<Item> ShopItemLists = new List<Item>(); // 슬롯에 들어갈 아이템들

    private void Awake()
    {
        int i = 0;
        foreach (var item in ShopItemSlots)
        {
            item.GetComponent<ShopItemSetting>().ShopItemID = ShopItemLists[i].itemID;
            i++;
            Debug.Log(item.GetComponent<ShopItemSetting>().ShopItemID);
        }
    }

}
