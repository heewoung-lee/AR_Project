using System;
using UnityEngine;

public class BaitButtonAction : MonoBehaviour
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
            BaitChangeAction?.Invoke(BaitID, count);
        });
        invenitemslot.BaitIDChanged += (() => BaitID = GetComponent<InvenItemSlot>().ItemsID);
    }
}
