using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




/// <summary>
/// TODO: 추후 피드백 적용 정리
/// </summary>
public class ShopItemBuyButton : MonoBehaviour
{
    InventoryList _playerInventoryList; // 상점 인벤 리스트
    Button _shopItemBuyButton;
    //public int ShopItemPrice
    //{
    //    get => _shopItemPrice;
    //    set => _shopItemPrice = value;
    //}

    public int ShopItemID
    {
        get => _shopItemID;
        set => _shopItemID = value;
    }

   // int _shopItemPrice; // 넘겨줄 가격 
    int _shopItemID; // 아이디 넘겨줄때 사용할 거

    public event Action OnBuyItem;

    private void Awake()
    {
        _shopItemBuyButton = GetComponent<Button>(); 
    }

    private void Start()
    {
        _playerInventoryList = this.transform.root.Find("MyInven /InvenGrid").GetComponent<InventoryList>();
        _shopItemBuyButton.onClick.AddListener(() => BuyItem());
    }

    /// <summary>
    /// Item 구매시 Inventory 적용 
    /// </summary>
    void BuyItem()
    {
        // 해당 버튼을 눌렀을때 들고있는 소지금 감소 시키고 
        if (PlayerGold.PlayerGolds <= 0) { PlayerGold._playerGold = 0; return; }
       
        // 해당 버튼 눌렀을때 인벤이 비어있으면 (이미지? bool?)들어가고 
        //Debug.Log(_playerInventoryList.InventoryLists[0].transform.GetChild(0).GetComponent<Image>());
        //int i = 0;
        foreach (var item in _playerInventoryList.InventoryLists)
        {
            if (item.GetComponent<InvenItemSlot>().IsEmpty)
            {
                item.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
                item.GetComponent<InvenItemSlot>().IsEmpty = false;
                PlayerGold.PlayerGolds -= ItemDataBase.ItemData[_shopItemID].itemPrice;
                return;
            }
        }
        // 중첩 가능? (48 칸인데  물고기만 중첩 가능?)
    }


}
