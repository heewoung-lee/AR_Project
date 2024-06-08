using FishingGameTool.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryList : MonoBehaviour
{
    //SimpleUIManager _simpleUIManeger;
    public List<GameObject> InventoryLists => inventoryList;
    [SerializeField] List<GameObject> inventoryList = new List<GameObject>(); // 인벤토리 리스
                                                                              // 해당 리스트를 이용해서  ID를 바꾸면  
    private void Start()
    {
        //_simpleUIManeger = FindObjectOfType<SimpleUIManager>();
        //_simpleUIManeger.FishInventoryIn += GetFIsh;
    }
    //public void GetFIsh(int Fishnumber)
    //{
    //    foreach(var item in inventoryList)
    //    {
    //        if (item.GetComponent<InvenItemSlot>().IsEmpty)
    //        {
    //            item.GetComponent<InvenItemSlot>().IsEmpty = false;
    //            item.GetComponent<InvenItemSlot>().ItemsID = Fishnumber;
    //            item.transform.GetChild(0).GetComponent<Image>().sprite = ItemDataBase.ItemData[Fishnumber].itemImage;
    //            Debug.Log($"{item}호출");
    //            return;
    //        }
    //    }
    //}
}
