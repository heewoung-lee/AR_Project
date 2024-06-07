using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingCatchActionEase : MonoBehaviour
{

    public AnimationCurve FishingCatchAction;
    // Start is called before the first frame update
    void Start()
    {
        FishingCatchAction = new AnimationCurve();
    }
}
