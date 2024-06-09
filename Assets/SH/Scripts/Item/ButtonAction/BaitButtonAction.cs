using System;
using UnityEngine;

public class BaitButtonAction : MonoBehaviour, IButtonAction
{
    InvenItemSlot invenitemslot;
    ButtonEventComponent buttonEvent;
    int BaitID;
    int count;
    public event Action<int,int, BaitButtonAction> BaitChangeAction;
    private void Awake()
    {
        invenitemslot = GetComponent<InvenItemSlot>();
        buttonEvent = GetComponent<ButtonEventComponent>();
        buttonEvent.ButtonAction(() => BaitChangeAction?.Invoke(BaitID,count,this));
        invenitemslot.BaitIDChanged += (() => BaitID = GetComponent<InvenItemSlot>().ItemsID);
    }
}
