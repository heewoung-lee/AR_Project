using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ī�޶� ���� ��� ������Ʈ
    public Vector3 offset; // ī�޶�� ��� ���� �Ÿ�
    public float smoothSpeed = 0.125f; // ī�޶� �̵��� �ε巯�� ����

    private void OnEnable()
    {
        target = transform.GetChild(0).GetComponent<Transform>();
        offset = new Vector3(5, 5, 5);
    }
    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset; // ��� ��ġ�� offset�� ���� ��ġ
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // �ε巴�� �̵�
        transform.position = smoothedPosition; // ī�޶� ��ġ ����

        transform.LookAt(target); // ��� ������Ʈ�� �ٶ󺸵��� ���� (�ɼ�)
    }
}
