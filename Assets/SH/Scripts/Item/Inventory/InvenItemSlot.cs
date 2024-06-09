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
    bool _isEmpty = true; // �� ���� �Ǵ� ���� �̹��� �Ǵ��̶��?
}
