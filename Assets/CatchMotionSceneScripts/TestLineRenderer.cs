using UnityEngine;

public class TestLineRenderer : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        // LineRenderer 컴포넌트를 가져옵니다.
        lineRenderer = GetComponent<LineRenderer>();

        // LineRenderer의 포인트 수를 설정합니다.
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        // LineRenderer의 첫 번째 포인트를 시작점으로 설정합니다.
        lineRenderer.SetPosition(0, startPoint.position);

        // LineRenderer의 두 번째 포인트를 끝점으로 설정합니다.
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
