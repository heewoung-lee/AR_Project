using UnityEngine;

[CreateAssetMenu (menuName = "Data / Item / Fish")]
public class FishItem : Item
{
    public FishType fishType; // 강인진 민물인지 바다인지 판단
    public int fishMinLength; // 물고기 길이
    public int fishMaxLength;
    public int fishMinKg; // 물고기 최소 무게
    public int fishMaxKg; // 물고기 최대 무게

    private void Reset()
    {
        itemType = ItemType.Fish;
    }
}
