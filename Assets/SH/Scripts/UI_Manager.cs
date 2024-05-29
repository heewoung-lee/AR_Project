using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// ����� �ӽ÷� ���� 
/// ���� ��Ű�� �����ӿ�ũ ������ ���� ���� 
/// </summary>
public class UI_Manager : MonoBehaviour
{
    static UI_Manager _instance;
    public static UI_Manager Instance => _instance;
    public  GameObject ShopUI => _shopUI; // ����
    public  GameObject InventoryUI => _inventoryUI; // �κ�    
    public  GameObject FishingRodUI => _fishigRodUI; // ���ô�
    public  GameObject BaitUI => _baitUI; // �̳�

    
    GameObject _shopUI; // ���� UI
    GameObject _inventoryUI; // �κ��丮 
    GameObject _fishigRodUI; // ���ô�
    GameObject _baitUI; // �̳�

    private void Awake()
    {
        _shopUI = GameObject.Find("UI_Shop");
        _inventoryUI = GameObject.Find("UI_Inventory");
        _fishigRodUI = GameObject.Find("UI_FishingRod");
        _baitUI = GameObject.Find("UI_Bait");
    }

}
