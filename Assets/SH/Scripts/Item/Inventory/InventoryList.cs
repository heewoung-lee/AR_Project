using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryList : MonoBehaviour
{
    public List<GameObject> InventoryLists => inventoryList; 
    [SerializeField] List<GameObject> inventoryList = new List<GameObject>(); // 인벤토리 리스트
    // 해당 리스트를 이용해서  ID를 바꾸면 됨

}
