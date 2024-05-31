using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScripts : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(gameObject, 5f);
    }
}
