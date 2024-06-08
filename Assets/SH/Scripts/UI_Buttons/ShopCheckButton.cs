using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCheckButton : MonoBehaviour
{
    ButtonEventComponent buttonEvent;
    InvenItemSlot slot;
    Image itemIcon;
    PlayerInvenList playerInvenList;
    PlayerInvenList shopInvenList;

    private void Start()
    {
        buttonEvent = GetComponent<ButtonEventComponent>();
        playerInvenList = GameObject.Find("UI_Inventory").GetComponent<PlayerInvenList>();
        shopInvenList = GetComponentInParent<PlayerInvenList>();

        Debug.Log(playerInvenList);
        slot = GetComponent<InvenItemSlot>();
        itemIcon = transform.GetChild(0).GetComponent<Image>();

        buttonEvent.ButtonAction(() =>
        {
            slot.IsEmpty = true;
            itemIcon.sprite = null;
            int i = 0;
            foreach (var item in shopInvenList.InventoryLists)
            {
                playerInvenList.InventoryLists[i].GetComponent<InvenItemSlot>().IsEmpty = shopInvenList.InventoryLists[i].GetComponent<InvenItemSlot>().IsEmpty;
                playerInvenList.InventoryLists[i].transform.GetChild(0).GetComponent<Image>().sprite = shopInvenList.InventoryLists[i].transform.GetChild(0).GetComponent<Image>().sprite;
                i++;
            }

        }
        );
    }
}
