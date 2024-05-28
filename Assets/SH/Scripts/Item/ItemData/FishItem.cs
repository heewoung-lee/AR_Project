using UnityEngine;

[CreateAssetMenu (menuName = "Data / Item / Fish")]
public class FishItem : Item
{
    public FishType fishType; // 강인진 민물인지 바다인지 판단
    private void Reset()
    {
        itemType = ItemType.Fish;
    }
}
