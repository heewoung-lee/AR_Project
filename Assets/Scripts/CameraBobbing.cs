using UnityEngine;
using DG.Tweening;

public class CameraBobbing : MonoBehaviour
{
    private const float DURATION = 1.0f; // 애니메이션의 반복 주기
    private const float ANGLE = 1f; // 카메라가 움직일 최대 각도

    void Start()
    {
        // 초기 로테이션을 저장
        Vector3 initialRotation = transform.localEulerAngles;

        // 고개를 위아래로 반복하는 애니메이션 설정
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(new Vector3(initialRotation.x + ANGLE, initialRotation.y, initialRotation.z), DURATION).SetEase(Ease.InOutSine))
                .Append(transform.DOLocalRotate(new Vector3(initialRotation.x - ANGLE, initialRotation.y, initialRotation.z), DURATION).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo); // 무한 반복
    }
}
