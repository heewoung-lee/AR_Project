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

    private void Start()
    {
        _simpleUIManeger = FindObjectOfType<SimpleUIManager>();
        _fishingSystem = FindObjectOfType<FishingSystem>();
        GameObject lopeObject = Instantiate(Resources.Load<GameObject>("HW/Prefabs/LoadRope"), this.transform);
        _bateObject = lopeObject.transform.GetComponent<BaitObject>();
        _fishingSystem.castingMontion += (() =>
        {
                lopeObject.GetComponent<LineRenderer>().enabled = false;
                _bateObject.DestroyObject();
        });

        _simpleUIManeger.FishLoadLineEnable += (() => 
        {
            lopeObject.GetComponent<LineRenderer>().enabled = true;
            _bateObject.On_Object();
         });
    }
}
