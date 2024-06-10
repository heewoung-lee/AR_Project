using System;
using UnityEngine;

public class BaitButtonAction : MonoBehaviour
{
    InvenItemSlot invenitemslot;
    ButtonEventComponent buttonEvent;
    BaitCount baitcount;
    int BaitID;
    int count;
    public event Action<int,BaitCount> BaitChangeAction;
    private void Awake()
    {
        invenitemslot = GetComponent<InvenItemSlot>();
        baitcount = GetComponentInChildren<BaitCount>();
        buttonEvent = GetComponent<ButtonEventComponent>();
        buttonEvent.ButtonAction(() =>
        {
            BaitChangeAction?.Invoke(BaitID,baitcount);
        });
        invenitemslot.BaitIDChanged += (() => BaitID = invenitemslot.ItemsID);
        baitcount.baitCountEvent += (() => count = baitcount.BaitCounts);
    }
}
