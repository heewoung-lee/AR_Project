using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// 현재는 임시로 구성 
/// 추후 루키스 프레임워크 구성을 통한 관리 
/// </summary>
public class UI_Manager : MonoBehaviour
{
    static UI_Manager _instance;
    public static UI_Manager Instance => _instance;
    public  GameObject ShopUI => _shopUI; // 상점
    public  GameObject InventoryUI => _inventoryUI; // 인벤    
    public  GameObject FishingRodUI => _fishigRodUI; // 낚시대
    public  GameObject BaitUI => _baitUI; // 미끼

    
    GameObject _shopUI; // 상점 UI
    GameObject _inventoryUI; // 인벤토리 
    GameObject _fishigRodUI; // 낚시대
    GameObject _baitUI; // 미끼

    private void Awake()
    {
        _shopUI = GameObject.Find("UI_Shop");
        _inventoryUI = GameObject.Find("UI_Inventory");
        _fishigRodUI = GameObject.Find("UI_FishingRod");
        _baitUI = GameObject.Find("UI_Bait");
    }

}
