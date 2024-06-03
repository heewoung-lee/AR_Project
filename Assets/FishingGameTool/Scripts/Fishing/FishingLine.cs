using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public LineRenderer lineRenderer; // ���� ������ ������Ʈ
    public int segmentCount = 15; // ���׸�Ʈ ����
    public int constrainLoop = 15; // ���� ���� �ݺ� Ƚ��
    public float segmentLength; // �� ���׸�Ʈ�� ����
    public float ropeWidth = 0.02f; // ���� �ʺ� (�� ��� ����)
    [Space(10f)]
    public Transform startTransform; // ���� ���� ��ġ�� ��Ÿ���� Ʈ������
    public Transform endTransform; // ���� �� ��ġ�� ��Ÿ���� Ʈ������
    public Vector3 gravity = new Vector3(0, -9.8f, 0); // �߷� ����

    private List<Segment> segments = new List<Segment>(); // ���׸�Ʈ ����Ʈ

    // �ʱ� ������ �缳���ϴ� �޼���
    private void Reset()
    {
        TryGetComponent(out lineRenderer); // LineRenderer ������Ʈ�� ������
    }

    // ��ũ��Ʈ�� ��� �� ȣ��Ǵ� �޼���
    private void Awake()
    {
        Vector3 segmentPos = startTransform.position; // ���� ��ġ ����
        for (int i = 0; i < segmentCount; i++)
        {
            segments.Add(new Segment(segmentPos)); // ���׸�Ʈ ���� �� ����Ʈ�� �߰�
            segmentPos.y -= segmentLength; // ���� ���׸�Ʈ�� ��ġ ����
        }
    }

    // ������ ������Ʈ Ÿ�ֿ̹� ȣ��Ǵ� �޼���
    private void FixedUpdate()
    {
        UpdateSegments(); // ���׸�Ʈ ������Ʈ
        for (int i = 0; i < constrainLoop; i++)
        {
            ApplyConstraint(); // ���� ����
        }
        DrawRopes(); // ���� �׸�
    }

    // ���� �׸��� �޼���
    private void DrawRopes()
    {
        lineRenderer.startWidth = ropeWidth; // ���� �κ� ���� �ʺ� ����
        lineRenderer.endWidth = ropeWidth; // �� �κ� ���� �ʺ� ����
        Vector3[] segmentPositions = new Vector3[segments.Count]; // ���׸�Ʈ ��ġ �迭 ����
        for (int i = 0; i < segments.Count; i++)
        {
            segmentPositions[i] = segments[i].position; // ���׸�Ʈ ��ġ�� �迭�� ����
        }
        lineRenderer.positionCount = segmentPositions.Length; // ���� �������� ������ ���� ����
        lineRenderer.SetPositions(segmentPositions); // ���� �������� ������ ����
    }

    // ������ �����ϴ� �޼���
    private void ApplyConstraint()
    {
        segments[0].position = startTransform.position; // ù ��° ���׸�Ʈ ��ġ�� ���� ��ġ�� ����

        for (int i = 0; i < segments.Count - 1; i++)
        {
            float distance = (segments[i].position - segments[i + 1].position).magnitude; // ���׸�Ʈ �� �Ÿ� ���
            float difference = segmentLength - distance; // �Ÿ� ���� ���
            Vector3 dir = (segments[i + 1].position - segments[i].position).normalized; // ���� ���� ���

            Vector3 movement = dir * difference; // �̵� ���� ���

            if (i == 0)
            {
                segments[i + 1].position += movement; // ù ��° ���׸�Ʈ�� ���� ���׸�Ʈ �̵�
            }
            else
            {
                segments[i].position -= movement * 0.5f; // ���� ���׸�Ʈ �̵�
                segments[i + 1].position += movement * 0.5f; // ���� ���׸�Ʈ �̵�
            }
        }

        // ������ ���׸�Ʈ�� endTransform ��ġ�� ����
        if (endTransform != null)
        {
            segments[segments.Count - 1].position = endTransform.position;
        }
    }

    // ���׸�Ʈ�� ������Ʈ�ϴ� �޼���
    private void UpdateSegments()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].velocity = segments[i].position - segments[i].previousPos; // �ӵ� ���
            segments[i].previousPos = segments[i].position; // ���� ��ġ ����
            segments[i].position += gravity * Time.fixedDeltaTime * Time.fixedDeltaTime; // �߷¿� ���� ��ġ ��ȭ ���
            segments[i].position += segments[i].velocity; // �ӵ��� ���� ��ġ ��ȭ ���
        }
    }

    // ���׸�Ʈ Ŭ����
    public class Segment
    {
        public Vector3 previousPos; // ���� ��ġ
        public Vector3 position; // ���� ��ġ
        public Vector3 velocity; // �ӵ�

        public Segment(Vector3 _position)
        {
            previousPos = _position; // ���� ��ġ �ʱ�ȭ
            position = _position; // ���� ��ġ �ʱ�ȭ
            velocity = Vector3.zero; // �ӵ� �ʱ�ȭ
        }
    }
}
