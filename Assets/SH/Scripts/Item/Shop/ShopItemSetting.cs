using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ShopItemList의 아이템ID를 받아와서 상점에 들어간 아이템에 맞게 세팅해줌 
/// </summary>
public class ShopItemSetting : MonoBehaviour
{
    public int ShopItemID
    {  get => _shopItemID;
       set => _shopItemID = value;
    }

    int _shopItemID; //상점에 아이템을 구분시켜줌

    Image _shopItemSprite; // 상점아이템의 이미지
    TMP_Text _shopItemInfo; // 상점아이템의 정보 텍스트
    TMP_Text _shopItemPrice; // 상점 아이템의 가격 텍스트
    ShopItemBuyButton _shopBuyButton; // 상점 사는 아이템의 가격 

    private void Awake()
    {
        _shopItemSprite = transform.Find("ItemImage/ItemIcon").GetComponent<Image>();
        _shopItemInfo = transform.Find("ItemInfoBG/ItemInfoText").GetComponent<TMP_Text>();
        _shopItemPrice = transform.Find("ItemInfoBG/ItemPriceText").GetComponent <TMP_Text>();
        _shopBuyButton = transform.Find("ItemBuyButton").GetComponent<ShopItemBuyButton>();
    }

    // 이미지 , 가격 , 아이템 설명 
    public void Start()
    {
        _shopItemSprite.sprite = ItemDataBase.ItemData[_shopItemID].itemImage;
        //_shopBuyButton.ShopItemPrice = ItemDataBase.ItemData[_shopItemID].itemPrice;
        _shopBuyButton.ShopItemID = ItemDataBase.ItemData[_shopItemID].itemID;
        _shopItemInfo.text = ItemDataBase.ItemData[_shopItemID].itemInfo;
        _shopItemPrice.text = ItemDataBase.ItemData[_shopItemID].itemPrice.ToString();
    }
}
