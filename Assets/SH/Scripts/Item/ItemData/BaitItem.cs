using UnityEngine;
[CreateAssetMenu(menuName = "Data / Item / Bait")]
public class BaitItem : Item
{
    public BaitType baitType; 
    private void Reset()
    {
        itemType = ItemType.Bait;
    }
}
