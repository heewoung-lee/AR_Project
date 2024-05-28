using UnityEngine;
[CreateAssetMenu(menuName = "Data / Item / Rod")]
public class RodItem : Item
{
    public RodType rodType; 
    private void Reset()
    {
        itemType = ItemType.Rod;
    }
}
