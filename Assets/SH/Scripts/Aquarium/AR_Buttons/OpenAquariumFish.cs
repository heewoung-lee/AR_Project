using UnityEngine;

public class OpenAquariumFish : MonoBehaviour
{
    Canvas _AquariumUI;
    ButtonEventComponent _buttonevent;

    private void Awake()
    {
        _buttonevent = gameObject.AddComponent<ButtonEventComponent>();
        _AquariumUI = GameObject.Find("UI_Aquarium").GetComponent<Canvas>();

        UI_Manager.Instance.RegisterUI("AquariumUI", _AquariumUI);
        _buttonevent.ButtonAction(() => UI_Manager.Instance.ToggleUI("AquariumUI"));
    }
}
