using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 대상 오브젝트
    public Vector3 offset; // 카메라와 대상 간의 거리
    public float smoothSpeed = 0.125f; // 카메라 이동의 부드러움 정도

    private void OnEnable()
    {
        target = transform.GetChild(0).GetComponent<Transform>();
        offset = new Vector3(5, 5, 5);
    }
    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset; // 대상 위치에 offset을 더한 위치
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // 부드럽게 이동
        transform.position = smoothedPosition; // 카메라 위치 설정

        transform.LookAt(target); // 대상 오브젝트를 바라보도록 설정 (옵션)
    }
}
