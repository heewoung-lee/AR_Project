using UnityEngine;

[CreateAssetMenu (menuName = "Data / Item / Fish")]
public class FishItem : Item
{
    public FishType fishType; // ������ �ι����� �ٴ����� �Ǵ�
    public int fishMinLength; // ����� ����
    public int fishMaxLength;
    public int fishMinKg; // ����� �ּ� ����
    public int fishMaxKg; // ����� �ִ� ����

    private void Reset()
    {
        itemType = ItemType.Fish;
    }
}
