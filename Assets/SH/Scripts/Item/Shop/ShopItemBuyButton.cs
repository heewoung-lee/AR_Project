using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




/// <summary>
/// TODO: 추후 피드백 적용 정리
/// </summary>
public class ShopItemBuyButton : MonoBehaviour
{
    PlayerInvenList _playerShopFishInventoryList; // 상점 Fish 인벤 리스트
    InventoryList _playerShopBaitInventoryList; // 상점 미끼 인벤 리스트
    InventoryList _playerShopRodInventoryList; // 상점 낚시대 인벤 리스트

    [SerializeField] PlayerInvenList _playerFishInventoryList; // Fish 인벤
    [SerializeField] InventoryList _playerBaitInventoryList; // 미끼 인벤 
    [SerializeField] InventoryList _playerRodInventoryList; // 낚시대 인벤

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


    private void Awake()
    {
        _shopItemBuyButton = GetComponent<Button>(); 
    }

    private void Start()
    {
        _playerShopFishInventoryList = this.transform.root.Find("MyInven /FishInvenGrid").GetComponent<PlayerInvenList>();
        _playerShopBaitInventoryList = this.transform.root.Find("MyInven /BaitInvenGrid ").GetComponent<InventoryList>();
        _playerShopRodInventoryList = this.transform.root.Find("MyInven /RodInvenGrid").GetComponent<InventoryList>();
        _shopItemBuyButton.onClick.AddListener(() => BuyItem());
    }

    /// <summary>
    /// Item 구매시 Inventory 적용 
    /// </summary>
    void BuyItem()
    {
        // 해당 버튼을 눌렀을때 들고있는 소지금 감소 시키고 
        if (PlayerGold.PlayerGolds <= 0 ) { return; }

        // 해당 버튼 눌렀을때 인벤이 비어있으면 (이미지? bool?)들어가고 
        //Debug.Log(_playerInventoryList.InventoryLists[0].transform.GetChild(0).GetComponent<Image>());
        //int i = 0;

        switch (ItemDataBase.ItemData[_shopItemID].itemType)
        {
            case ItemType.Bait: // 미끼 경우
                foreach (var item in _playerShopBaitInventoryList.InventoryLists)
                {
                    if (item.GetComponent<InvenItemSlot>().IsEmpty)
                    {
                        item.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
                        item.GetComponent<InvenItemSlot>().IsEmpty = false;
                       
                        foreach (var inven in _playerBaitInventoryList.InventoryLists)
                        {
                            if (inven.GetComponent<InvenItemSlot>().IsEmpty)
                            {
                                inven.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
                                inven.GetComponent<InvenItemSlot>().IsEmpty = false;
                                if (PlayerGold.PlayerGolds >= ItemDataBase.ItemData[_shopItemID].itemPrice)
                                {
                                    PlayerGold.PlayerGolds -= ItemDataBase.ItemData[_shopItemID].itemPrice;
                                }
                                return;
                            }
                        }
                        return;
                    }
                }
                
                break;
            case ItemType.Rod: //낚시대 경우
                foreach (var item in _playerShopRodInventoryList.InventoryLists)
                {
                    if (item.GetComponent<InvenItemSlot>().IsEmpty)
                    {
                        item.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
                        item.GetComponent<InvenItemSlot>().IsEmpty = false;

                        foreach (var inven in _playerRodInventoryList.InventoryLists)
                        {
                            if (inven.GetComponent<InvenItemSlot>().IsEmpty)
                            {
                                inven.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
                                inven.GetComponent<InvenItemSlot>().IsEmpty = false;
                                if (PlayerGold.PlayerGolds >= ItemDataBase.ItemData[_shopItemID].itemPrice)
                                {
                                    PlayerGold.PlayerGolds -= ItemDataBase.ItemData[_shopItemID].itemPrice;
                                }
                                return;
                            }
                        }
                    }
                }
                
                break;
            case ItemType.Fish: // 물고기인 경우
                // 위에서 리턴 되버리네
                foreach (var item in _playerShopFishInventoryList.InventoryLists)
                {
                    if (item.GetComponent<InvenItemSlot>().IsEmpty)
                    {
                        item.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
                        item.GetComponent<InvenItemSlot>().IsEmpty = false;

                        foreach (var inven in _playerFishInventoryList.InventoryLists)
                        {
                            if (inven.GetComponent<InvenItemSlot>().IsEmpty)
                            {
                                inven.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
                                inven.GetComponent<InvenItemSlot>().IsEmpty = false;
                                if (PlayerGold.PlayerGolds >= ItemDataBase.ItemData[_shopItemID].itemPrice)
                                {
                                    PlayerGold.PlayerGolds -= ItemDataBase.ItemData[_shopItemID].itemPrice;
                                }
                                return;
                            }
                        }
                    }
                }
                
                break;
            case ItemType.Other:
                break;
            default:
                break;
        }
        
        // 중첩 가능? (48 칸인데  물고기만 중첩 가능?)
    }


}
