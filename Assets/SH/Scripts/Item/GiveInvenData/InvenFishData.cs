using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GiveFishData")]
public class InvenFishData : ScriptableObject
{
    // ���� ���⿡ ����? -> AquAriumButton �������� ����
    public List<int> fishItems = new List<int>();
}
