using System;
using UnityEngine;

public class BaitActionButton : MonoBehaviour
{
    ButtonEventComponent buttonEvent;
    int _baitID;
    public event Action<int> BaitButtonAction;

    private void Start()
    {
        _baitID = GetComponent<InvenItemSlot>().ItemsID;
        Debug.Log(_baitID);
        buttonEvent = GetComponent<ButtonEventComponent>();
        buttonEvent.ButtonAction(() => BaitButtonAction?.Invoke(_baitID));
    }
}
