using UnityEngine;

public class InventoryOpenCloseButton : MonoBehaviour
{
    Canvas _inventoryUI;
    ButtonEventComponent buttonEventComponent;

    private void Awake()
    {
        buttonEventComponent = gameObject.AddComponent<ButtonEventComponent>();
        _inventoryUI = GameObject.Find("UI_Inventory").GetComponent<Canvas>();
        UI_Manager.Instance.RegisterUI("InventoryUI", _inventoryUI);
        buttonEventComponent.ButtonAction(() => UI_Manager.Instance.ToggleUI("InventoryUI"));
    }

}
