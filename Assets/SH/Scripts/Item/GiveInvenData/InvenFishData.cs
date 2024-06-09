using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GiveFishData")]
public class InvenFishData : ScriptableObject
{
    // 언제 여기에 저장? -> AquAriumButton 눌렀을때 저장
    public List<int> fishItems = new List<int>();
}
