using UnityEngine;

public class ShopCancelButton : MonoBehaviour
{
    ButtonEventComponent _buttonEvent;

    private void Awake()
    {
        _buttonEvent = gameObject.AddComponent<ButtonEventComponent>();
        _buttonEvent.ButtonAction(() => UI_Manager.Instance.ToggleUI("ShopUI"));
    }

    
}
