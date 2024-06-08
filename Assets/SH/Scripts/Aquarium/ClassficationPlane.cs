using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation; //XR 시스템을 사용하기 위한 두가지 세팅
using UnityEngine.XR.ARSubsystems;


public class ClassficationPlane : MonoBehaviour
{
    ARPlane _arPlane;
    MeshRenderer _planeMeshRenderer;
    [SerializeField] TMP_Text _textMesh;
    [SerializeField] GameObject _textObj;
    GameObject _mainCam;


    private void Awake()
    {
        _arPlane = GetComponent<ARPlane>();
        _planeMeshRenderer = GetComponent<MeshRenderer>();
        _mainCam = FindObjectOfType<Camera>().gameObject;
    }

    private void Update()
    {
        UpdateLabel();
        UpdatePlaneColor();
    }

    private void UpdateLabel()
    {
        _textMesh.text = _arPlane.classification.ToString();
        _textObj.transform.position = _arPlane.center; // 정 중앙에 배치 
        _textObj.transform.LookAt(_mainCam.transform); // 캠이 바라 보는 위치로 
        _textObj.transform.Rotate(new Vector3(0, 180, 0));
    }

    void UpdatePlaneColor()
    {
        Color planeMatColor = Color.cyan;
        switch(_arPlane.classification)
        {
            case PlaneClassification.None:
                planeMatColor = Color.cyan;
                break;

            case PlaneClassification.Wall:
                planeMatColor = Color.white;
                break;

            case PlaneClassification.Floor:
                planeMatColor = Color.green;
                break;

            case PlaneClassification.Ceiling:
                planeMatColor = Color.blue;
                break;

            case PlaneClassification.Table:
                planeMatColor = Color.yellow;
                break;

            case PlaneClassification.Seat:
                planeMatColor = Color.magenta;
                break;

            case PlaneClassification.Door:
                planeMatColor = Color.red;
                break;

            case PlaneClassification.Window:
                planeMatColor = Color.clear;
                break;
        }

        planeMatColor.a = 0f;
        _planeMeshRenderer.material.color = planeMatColor;
    }

}
