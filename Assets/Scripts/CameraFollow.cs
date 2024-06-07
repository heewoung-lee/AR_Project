using DG.Tweening;
using FishingGameTool.Fishing;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    
    public Transform target; // 카메라가 따라갈 대상 오브젝트
    private FishingSystem _fishingSystem;
    private Camera _camera;


    //1. 물고기가 낚이면 물고기 방향으로 카메라 고정

    public void Awake()
    {
        _camera = GetComponent<Camera>();
        _fishingSystem = GameObject.Find("AR Session Origin/AR Camera/Character").GetComponent<FishingSystem>();
    }

    public void SetLootCamera(Transform targetTransform)
    {

    }

}
