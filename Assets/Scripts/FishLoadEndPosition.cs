using FishingGameTool.Fishing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishLoadEndPosition : MonoBehaviour
{
    FishingSystem _fishingSystem;
    private void Start()
    {
        _fishingSystem = FindObjectOfType<FishingSystem>();
        GameObject Lope = Resources.Load<GameObject>("HW/Prefabs/LoadRope");
        Instantiate(Lope,this.transform);
    }
}
