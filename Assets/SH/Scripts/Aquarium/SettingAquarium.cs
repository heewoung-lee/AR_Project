using UnityEngine;

public class SettingAquarium : MonoBehaviour
{
    InvenFishData invenFishData;
    InventoryList aquariumInvenList;
    private void Awake()
    {
        aquariumInvenList = GetComponent<InventoryList>();
        invenFishData = (InvenFishData)Resources.Load("SH/GiveInvenFishData");
        SettingFishData();
    }

    public void SettingFishData()
    {
        for (int i = 0; i < invenFishData.fishItems.Count; i++) 
        {

            if (invenFishData.fishItems[i] != 0)
            {
                aquariumInvenList.InventoryLists[i].GetComponent<AquariumInvenItem>().ItemsID = invenFishData.fishItems[i];
            }
            else
            aquariumInvenList.InventoryLists[i].GetComponent<AquariumInvenItem>().ItemsID = 0;
        }
    }
}
