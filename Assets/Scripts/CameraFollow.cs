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
    
    public Transform target; // ī�޶� ���� ��� ������Ʈ
    private FishingSystem _fishingSystem;
    private Camera _camera;


    //1. ����Ⱑ ���̸� ����� �������� ī�޶� ����

    public void Awake()
    {
        _camera = GetComponent<Camera>();
        _fishingSystem = GameObject.Find("AR Session Origin/AR Camera/Character").GetComponent<FishingSystem>();
    }

    public void SetLootCamera(Transform targetTransform)
    {

    }

}
