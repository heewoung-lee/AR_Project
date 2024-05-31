using UnityEngine;

public class TestLineRenderer : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        // LineRenderer ������Ʈ�� �����ɴϴ�.
        lineRenderer = GetComponent<LineRenderer>();

        // LineRenderer�� ����Ʈ ���� �����մϴ�.
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        // LineRenderer�� ù ��° ����Ʈ�� ���������� �����մϴ�.
        lineRenderer.SetPosition(0, startPoint.position);

        // LineRenderer�� �� ��° ����Ʈ�� �������� �����մϴ�.
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
