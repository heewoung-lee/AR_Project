using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AquariumFishInvenSetting : MonoBehaviour
{
    //public int AquariumItemID
    //{
    //    get => _AquariumItemID;
    //    set => _AquariumItemID = value;
    //}

    //int _AquariumItemID; //상점에 아이템을 구분시켜줌

    AquariumInvenItem aquariumInvenItem;

    Image _AquariumItemSprite; // 상점아이템의 이미지
    TMP_Text _AquariumItemInfo; // 상점아이템의 정보 텍스트

    private void Awake()
    {
        aquariumInvenItem = GetComponent<AquariumInvenItem>();
        _AquariumItemSprite = transform.Find("FishSlotBG/FishImageIcon").GetComponent<Image>();
        _AquariumItemInfo = transform.Find("FishInfoBG/InfoText").GetComponent<TMP_Text>();
        //_AquariumSettingButton = transform.Find("FishSettingButton").GetComponent<ShopItemBuyButton>();
    }

    // 이미지 , 가격 , 아이템 설명 
    public void Start()
    {
       // aquariumInvenItem.SettingFish += Setting;
    }


    private void Update()
    {
        Setting();
    }

    public void Setting()
    {
        if (aquariumInvenItem.ItemsID == 0)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        _AquariumItemSprite.sprite = ItemDataBase.ItemData[aquariumInvenItem.ItemsID].itemImage;
        _AquariumItemInfo.text = ItemDataBase.ItemData[aquariumInvenItem.ItemsID].itemInfo;
    }
   
}
