using FishingGameTool.Fishing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySceneManager : MonoBehaviour
{
    [SerializeField] private FishingSystem _fishingSystem;


    // Start is called before the first frame update
    void Start()
    {
        _fishingSystem = GameObject.Find("AR Session Origin/AR Camera/Character").GetComponent<FishingSystem>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
