using UnityEngine;

public class BaitOpenCloseButton : MonoBehaviour
{
    Canvas _baitUI;
    ButtonEventComponent _buttonevent;

    private void Awake()
    {
        _buttonevent = gameObject.AddComponent<ButtonEventComponent>();
        _baitUI = GameObject.Find("UI_Bait").GetComponent<Canvas>();

        UI_Manager.Instance.RegisterUI("BaitUI", _baitUI);
        _buttonevent.ButtonAction(() => UI_Manager.Instance.ToggleUI("BaitUI"));
    }
}
