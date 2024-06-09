using UnityEngine;
using static UnityEditor.Progress;

public class GiveInvenDatatoAquarium : MonoBehaviour
{
    [SerializeField] GameObject PlayerInventory;
    [SerializeField] MainGotoAquarium MainGotoAquarium;
    public InvenFishData InvenFishData;

    private void Awake()
    {
        //InvenFishData = (InvenFishData)Resources.Load("SH/GiveInvenFishData");
        MainGotoAquarium.OnAquariumClick += GiveData;
    }

    public void GiveData()
    {
        for (int i = 0; i < PlayerInventory.GetComponent<PlayerInvenList>().InventoryLists.Count; i++)
        {
            if (!PlayerInventory.GetComponent<PlayerInvenList>().InventoryLists[i].GetComponent<InvenItemSlot>().IsEmpty)
            {
                Debug.Log($"{i}¹øÂ° {PlayerInventory.GetComponent<PlayerInvenList>().InventoryLists[i].GetComponent<InvenItemSlot>().ItemsID}");
                InvenFishData.fishItems[i] = PlayerInventory.GetComponent<PlayerInvenList>().InventoryLists[i].GetComponent<InvenItemSlot>().ItemsID;
            }
            else
            {
                InvenFishData.fishItems[i] = 0;
            }
            
        }
    }
}

