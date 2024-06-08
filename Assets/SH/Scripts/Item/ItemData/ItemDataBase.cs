using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static Dictionary<int , Item> ItemData => _itemData;
    static Dictionary<int , Item> _itemData = new Dictionary<int , Item>();


    private void Awake()
    {
        AddItemToDataBase(1000, "SH/ItemData/TestBait");
        AddItemToDataBase(500, "SH/ItemData/TestFish");
        AddItemToDataBase(0, "SH/ItemData/DefaultItem");
        AddItemToDataBase(10, "SH/ItemData/TestRod");
    }

    private void AddItemToDataBase(int key, string resourcePath)
    {
        if (!_itemData.ContainsKey(key))
        {
            _itemData.Add(key, (Item)Resources.Load(resourcePath));
        }
        else
        {
            return;
        }
    }

}
