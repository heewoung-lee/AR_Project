using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCancelButton : MonoBehaviour
{
    ButtonEventComponent _buttonEvent;

    private void Awake()
    {
        _buttonEvent = gameObject.AddComponent<ButtonEventComponent>();
        _buttonEvent.ButtonAction(() => UI_Manager.Instance.ToggleUI("InventoryUI"));
    }

}
