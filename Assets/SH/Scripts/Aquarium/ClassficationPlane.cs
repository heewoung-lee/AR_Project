using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation; //XR �ý����� ����ϱ� ���� �ΰ��� ����
using UnityEngine.XR.ARSubsystems;

public class ClassficationPlane : MonoBehaviour
{
    ARPlane _arPlane;
    MeshRenderer _planeMeshRenderer;
    TextMesh _textMesh;
    GameObject _textObj;
    GameObject _mainCam;

    private void Awake()
    {
        _arPlane = GetComponent<ARPlane>();
        _planeMeshRenderer = GetComponent<MeshRenderer>();

        // �غκ��� ���?
        _textMesh = GetComponent<TextMesh>();   
        _textObj = GetComponent<GameObject>();
        _mainCam = GetComponent<GameObject>();
    }
}
