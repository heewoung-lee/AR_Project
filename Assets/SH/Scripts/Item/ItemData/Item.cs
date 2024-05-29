using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Data / Item")]
public class Item : ScriptableObject
{
    public ItemType itemType; // 그 아이템의 타입 , 기본 세팅 Other
    public int itemID; // 해당 아이템의 아이템ID
    public Sprite itemImage; // 해당 아이템의 이미지
    public string itemInfo; // 해당 아이템의 정보
    public int itemPrice; // 해당 아이템의 가격
    public int itmeEffect; // 미끼의 힘? 현재는 이거 뿐임
    
}
