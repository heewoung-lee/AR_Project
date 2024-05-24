using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLoad : MonoBehaviour
{
    [SerializeField]private GameObject _fishingLoad;
    void Start()
    {

        GameObject fishingLoad = Instantiate(_fishingLoad);


        fishingLoad.transform.SetParent(this.transform);

        RectTransform fishingLoad_RectTransform = fishingLoad.GetComponent<RectTransform>();
        fishingLoad_RectTransform.anchorMin = new Vector2(1, 0.5f); // Right center
        fishingLoad_RectTransform.anchorMax = new Vector2(1, 0.5f);
        fishingLoad_RectTransform.pivot = new Vector2(1, 0.5f);
        fishingLoad_RectTransform.anchoredPosition = new Vector2(-50, 0); // Slight offset from the right edge

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
