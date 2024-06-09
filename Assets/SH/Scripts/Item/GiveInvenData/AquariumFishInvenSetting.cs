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

    //int _AquariumItemID; //������ �������� ���н�����

    AquariumInvenItem aquariumInvenItem;

    Image _AquariumItemSprite; // ������������ �̹���
    TMP_Text _AquariumItemInfo; // ������������ ���� �ؽ�Ʈ

    private void Awake()
    {
        aquariumInvenItem = GetComponent<AquariumInvenItem>();
        _AquariumItemSprite = transform.Find("FishSlotBG/FishImageIcon").GetComponent<Image>();
        _AquariumItemInfo = transform.Find("FishInfoBG/InfoText").GetComponent<TMP_Text>();
        //_AquariumSettingButton = transform.Find("FishSettingButton").GetComponent<ShopItemBuyButton>();
    }

    // �̹��� , ���� , ������ ���� 
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
