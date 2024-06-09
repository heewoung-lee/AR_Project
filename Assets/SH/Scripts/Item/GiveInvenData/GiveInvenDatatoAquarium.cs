using UnityEngine;

public class GiveInvenDatatoAquarium : MonoBehaviour
{
    [SerializeField] GameObject PlayerInventory;
    public InvenFishData InvenFishData;
    GiveInvenDatatoAquarium Instance;

    private void Awake()
    {
        if (Instance != null) 
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }


    public void GiveData()
    {
        int i = 0;
        foreach (var item in PlayerInventory.GetComponent<InventoryList>().InventoryLists) 
        {
            InvenFishData.fishItems[i] = item.GetComponent<InvenItemSlot>().ItemsID;
            i++;
        }
    }
}
