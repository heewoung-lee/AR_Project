using UnityEngine;

public class FishingRodOpenCloseButton : MonoBehaviour
{
    Canvas _fishingRod;
    ButtonEventComponent buttonEventComponent;

    private void Awake()
    {
        buttonEventComponent = gameObject.AddComponent<ButtonEventComponent>();
        _fishingRod = GameObject.Find("UI_FishingRod").GetComponent<Canvas>();
        UI_Manager.Instance.RegisterUI("FishingRodUI", _fishingRod);
        buttonEventComponent.ButtonAction(() => UI_Manager.Instance.ToggleUI("FishingRodUI"));
    }

}
