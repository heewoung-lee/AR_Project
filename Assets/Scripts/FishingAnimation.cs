using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FishingAnimation : MonoBehaviour
{
    private Transform _catchLootCamera;
    private LineRenderer _fishingline;
    private Transform _startPoint;
    private Transform _endPoint;
    private Transform _caughtFish;

    // Start is called before the first frame update
    void Start()
    {
        _startPoint = GetComponent<Transform>();
        _fishingline = GetComponent<LineRenderer>();

        _caughtFish = transform.Find("CaughtFish").GetComponent<Transform>();
        _endPoint = transform.Find("CaughtFish/MousePosition").GetComponent<Transform>();
        _fishingline.positionCount = 2;
        _catchLootCamera = GetComponentInParent<Transform>();
    }

    private void Update()
    {
        if (_endPoint == null)
            return;


        _fishingline.SetPosition(0, _startPoint.position);
        _fishingline.SetPosition(1, _endPoint.position);
    }

}
