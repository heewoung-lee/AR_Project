using UnityEngine;

public class ShopOpenCloseButton : MonoBehaviour
{
    Canvas _shopUI;
    ButtonEventComponent _buttonEvent;

    private void Awake()
    {
        _buttonEvent = gameObject.AddComponent<ButtonEventComponent>();
        _shopUI = GameObject.Find("UI_Shop").GetComponent<Canvas>();
        UI_Manager.Instance.RegisterUI("ShopUI", _shopUI);
        _buttonEvent.ButtonAction(() => UI_Manager.Instance.ToggleUI("ShopUI"));
    }
}
