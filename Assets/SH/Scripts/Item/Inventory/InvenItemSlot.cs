using UnityEngine;

public class InvenItemSlot : MonoBehaviour
{
    public int ItemsID
    {
        get => _itemID;
        set => _itemID = value; 
    }
    
    public bool IsEmpty
    {
        get => _isEmpty;
        set => _isEmpty = value;
    }

    int _itemID;
    bool _isEmpty = true; // �� ���� �Ǵ� ���� �̹��� �Ǵ��̶��?
}
