using UnityEngine;

[CreateAssetMenu (menuName = "Data / Item / Other")]
public class OtherItem : Item
{

    private void Reset()
    {
       itemType = ItemType.Other;
    }
}
