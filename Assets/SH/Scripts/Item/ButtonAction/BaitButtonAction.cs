using System;
using UnityEngine;

public class BaitButtonAction : MonoBehaviour, IButtonAction
{
    InvenItemSlot invenitemslot;
    ButtonEventComponent buttonEvent;
    BaitObject baitobject;
    int BaitID;
    int count;
    public event Action<int,int> BaitChangeAction;
    private void Awake()
    {
        invenitemslot = GetComponent<InvenItemSlot>();
        buttonEvent = GetComponent<ButtonEventComponent>();
        buttonEvent.ButtonAction(() =>
        {
            baitobject = FindAnyObjectByType<BaitObject>();
            baitobject.baitButton = this;
            BaitChangeAction?.Invoke(BaitID, count);
        });
        invenitemslot.BaitIDChanged += (() => BaitID = GetComponent<InvenItemSlot>().ItemsID);
    }
}
