using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static Dictionary<int , Item> ItemData => _itemData;
    static Dictionary<int , Item> _itemData = new Dictionary<int , Item>();

    private void Awake()
    {
        _itemData.Add(1000, (Item)Resources.Load("SH/ItemData/TestBait"));
        _itemData.Add(500, (Item)Resources.Load("SH/ItemData/TestFish"));
        _itemData.Add(0, (Item)Resources.Load("SH/ItemData/DefaultItem"));
        _itemData.Add(10, (Item)Resources.Load("SH/ItemData/TestRod"));
    }

}
