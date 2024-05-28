using UnityEngine;

[CreateAssetMenu (menuName = "Data / Item / Fish")]
public class FishItem : Item
{
    public FishType fishType; // ������ �ι����� �ٴ����� �Ǵ�
    private void Reset()
    {
        itemType = ItemType.Fish;
    }
}
