using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryList : MonoBehaviour
{
    public List<GameObject> InventoryLists => inventoryList; 
    [SerializeField] List<GameObject> inventoryList = new List<GameObject>(); // �κ��丮 ����Ʈ
    // �ش� ����Ʈ�� �̿��ؼ�  ID�� �ٲٸ� ��

}
