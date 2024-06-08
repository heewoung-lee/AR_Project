using System;
using UnityEngine;

public class BaitButtonAction : MonoBehaviour
{
    ButtonEventComponent buttonEvent;
    int BaitID;
    public event Action<int> BaitChangeAction;


    private void Start()
    {
        BaitID = GetComponent<InvenItemSlot>().ItemsID;
        buttonEvent = GetComponent<ButtonEventComponent>();
        buttonEvent.ButtonAction(() => BaitChangeAction?.Invoke(BaitID));
    }
}
