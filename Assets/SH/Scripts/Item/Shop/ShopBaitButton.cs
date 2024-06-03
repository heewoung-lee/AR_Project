using UnityEngine;

public class ShopBaitButton : MonoBehaviour
{
    [SerializeField] GameObject _fishInventory;
    [SerializeField] GameObject _baitInventory;
    [SerializeField] GameObject _rodInventory;

    ButtonEventComponent _buttonEvent;

    bool _shopBait = true;
    bool _shopOthers = false;

    private void Start()
    {
        _buttonEvent = gameObject.AddComponent<ButtonEventComponent>();
        _buttonEvent.ButtonAction(() => { _baitInventory.SetActive(_shopBait); _fishInventory.SetActive(_shopOthers); _rodInventory.SetActive(_shopOthers); });
    }
}
