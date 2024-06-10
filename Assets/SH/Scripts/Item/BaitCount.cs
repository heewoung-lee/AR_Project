using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaitCount : MonoBehaviour
{
    TMP_Text text;

    public event Action baitCountEvent;
    public int BaitCounts 
    {
        get => baitCount;

        set { 
            baitCount = value; 
            text.text = baitCount.ToString();
            baitCountEvent?.Invoke();
        }
    }

    int baitCount = 0;
    private void Start()
    {
        text = GetComponent<TMP_Text>();    
        text.text = baitCount.ToString();
    }
}
