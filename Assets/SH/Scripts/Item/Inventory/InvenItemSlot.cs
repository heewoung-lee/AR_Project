using System;
using UnityEngine;

public class InvenItemSlot : MonoBehaviour
{
    public event Action BaitIDChanged;

    public int ItemsID
    {
        get => _itemID;
        set { _itemID = value; BaitIDChanged?.Invoke(); } 
    }
    
    public bool IsEmpty
    {
        get => _isEmpty;
        set => _isEmpty = value;
    }

    int _itemID;
    bool _isEmpty = true; // 참 거짓 판단 말고 이미지 판단이라면?
}
