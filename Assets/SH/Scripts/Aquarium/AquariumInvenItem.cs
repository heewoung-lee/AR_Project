using System;
using UnityEngine;

public class AquariumInvenItem : MonoBehaviour
{
    public int ItemsID
    {
        get => _itemID;
        set { _itemID = value; SettingFish?.Invoke(); }
    }

    public bool IsEmpty
    {
        get => _isEmpty;
        set => _isEmpty = value;
    }

    int _itemID;
    bool _isEmpty = true; // �� ���� �Ǵ� ���� �̹��� �Ǵ��̶��?

    public event Action SettingFish;
}
