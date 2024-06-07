using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatFish : MonoBehaviour
{
    Camera _catchingCamera;
    GameObject _lootObject;
    // Update is called once per frame

    private void Awake()
    {
        _catchingCamera = GetComponent<Camera>();
    }
    private void OnEnable()
    {
        _lootObject = FindObjectOfType<FishScripts>().gameObject.GetComponentInChildren<FishTransform>().gameObject;
        _catchingCamera.transform.position = new Vector3(_lootObject.transform.position.x, _lootObject.transform.position.y, _lootObject.transform.position.z+5);
        _catchingCamera.transform.position += Vector3.up *1.5f;
    }
    private void OnDisable()
    {
        _lootObject = null;
    }

    void Update()
    {
        if (_lootObject == null)
            return;


        _catchingCamera.transform.LookAt(new Vector3(_lootObject.transform.position.x, _lootObject.transform.position.y, _lootObject.transform.position.z));//카메라 위치선정)
    }
}
