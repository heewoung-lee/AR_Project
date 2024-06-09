using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaitObject : MonoBehaviour
{

    GameObject _bait;
    GameObject _fishFloat;


    FishingLine _fishingLine;
    // Start is called before the first frame update
    void Start()
    {
        _fishingLine = GetComponent<FishingLine>();
        _bait = Instantiate(Resources.Load("HW/Prefabs/BaitPrefab") as GameObject);
        _fishFloat = Instantiate(Resources.Load("HW/Prefabs/FishingFloat") as GameObject);
    }

    private void Update()
    {
        _fishFloat.GetComponentInChildren<StartPoint>().transform.position = _fishingLine.Segments[_fishingLine.Segments.Count - 1].position;
        _bait.transform.position = _fishFloat.GetComponentInChildren<EndPoint>().transform.position;
    }

    public void DestroyObject()
    {
        _bait.SetActive(false);
        _fishFloat.SetActive(false);
    }

    public void On_Object()
    {
        _bait.SetActive(true);
        _fishFloat.SetActive(true);
    }
}
