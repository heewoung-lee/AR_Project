using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation; //XR 시스템을 사용하기 위한 두가지 세팅
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

        // 밑부분은 고민?
        _textMesh = GetComponent<TextMesh>();   
        _textObj = GetComponent<GameObject>();
        _mainCam = GetComponent<GameObject>();
    }
}
