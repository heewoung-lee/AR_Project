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
        GameObject lopeObject = Instantiate(Resources.Load<GameObject>("HW/Prefabs/LoadRope"), this.transform);
        _fishingSystem.castingMontion += () => { lopeObject.GetComponent<LineRenderer>().enabled = false; };
    }
}
