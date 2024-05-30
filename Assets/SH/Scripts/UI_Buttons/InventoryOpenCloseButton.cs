using UnityEngine;
using UnityEngine.UI;

public class InventoryOpenCloseButton : MonoBehaviour
{
    Canvas _inventoryUI;
    Button cancelButton;


    public static bool _shopButton = false;
    private void Awake()
    {
        cancelButton = GetComponent<Button>();
        _inventoryUI = GameObject.Find("UI_Inventory").GetComponent<Canvas>();
    }

    private void Start()
    {
        cancelButton.onClick.AddListener(() => { _shopButton = !_shopButton; _inventoryUI.enabled = _shopButton; });
    }
}
