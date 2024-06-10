using FishingGameTool.Example;
using FishingGameTool.Fishing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishLoadEndPosition : MonoBehaviour
{
    FishingSystem _fishingSystem;
    SimpleUIManager _simpleUIManeger;
    BaitObject _bateObject;
    GameObject lopeObject;

    private void Start()
    {
        _simpleUIManeger = FindObjectOfType<SimpleUIManager>();
        _fishingSystem = FindObjectOfType<FishingSystem>();
        lopeObject = Instantiate(Resources.Load<GameObject>("HW/Prefabs/LoadRope"), this.transform);
        _bateObject = lopeObject.transform.GetComponent<BaitObject>();
        _fishingSystem.castingMontion += DestroyFishingRope;
        _simpleUIManeger.FishLoadLineEnable += CreateFishingRope;
    }



    public void DestroyFishingRope()
    {
        lopeObject.GetComponent<LineRenderer>().enabled = false;
        _bateObject.DestroyObject();
    }

    public void CreateFishingRope()
    {
        lopeObject.GetComponent<LineRenderer>().enabled = true;
        _bateObject.On_Object();
    }
}
