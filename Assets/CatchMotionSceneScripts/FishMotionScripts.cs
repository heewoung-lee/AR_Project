using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FishMotionScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveY(-2f, 2f).SetLoops(-1,LoopType.Yoyo);
    }

}
