using UnityEngine;
using DG.Tweening;

public class CameraBobbing : MonoBehaviour
{
    private const float DURATION = 1.0f; // �ִϸ��̼��� �ݺ� �ֱ�
    private const float ANGLE = 1f; // ī�޶� ������ �ִ� ����

    void Start()
    {
        // �ʱ� �����̼��� ����
        Vector3 initialRotation = transform.localEulerAngles;

        // ���� ���Ʒ��� �ݺ��ϴ� �ִϸ��̼� ����
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(new Vector3(initialRotation.x + ANGLE, initialRotation.y, initialRotation.z), DURATION).SetEase(Ease.InOutSine))
                .Append(transform.DOLocalRotate(new Vector3(initialRotation.x - ANGLE, initialRotation.y, initialRotation.z), DURATION).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo); // ���� �ݺ�
    }
}
