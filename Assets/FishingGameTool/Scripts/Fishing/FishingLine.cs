using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public LineRenderer lineRenderer; // 라인 렌더러 컴포넌트
    public int segmentCount = 15; // 세그먼트 개수
    public int constrainLoop = 15; // 제약 적용 반복 횟수
    public float segmentLength; // 각 세그먼트의 길이
    public float ropeWidth = 0.02f; // 줄의 너비 (더 얇게 설정)
    [Space(10f)]
    public Transform startTransform; // 줄의 시작 위치를 나타내는 트랜스폼
    public Transform endTransform; // 줄의 끝 위치를 나타내는 트랜스폼
    public Vector3 gravity = new Vector3(0, -9.8f, 0); // 중력 벡터

    private List<Segment> segments = new List<Segment>(); // 세그먼트 리스트

    // 초기 설정을 재설정하는 메서드
    private void Reset()
    {
        TryGetComponent(out lineRenderer); // LineRenderer 컴포넌트를 가져옴
    }

    // 스크립트가 깨어날 때 호출되는 메서드
    private void Awake()
    {
        Vector3 segmentPos = startTransform.position; // 시작 위치 설정
        for (int i = 0; i < segmentCount; i++)
        {
            segments.Add(new Segment(segmentPos)); // 세그먼트 생성 후 리스트에 추가
            segmentPos.y -= segmentLength; // 다음 세그먼트의 위치 설정
        }
    }

    // 고정된 업데이트 타이밍에 호출되는 메서드
    private void FixedUpdate()
    {
        UpdateSegments(); // 세그먼트 업데이트
        for (int i = 0; i < constrainLoop; i++)
        {
            ApplyConstraint(); // 제약 적용
        }
        DrawRopes(); // 줄을 그림
    }

    // 줄을 그리는 메서드
    private void DrawRopes()
    {
        lineRenderer.startWidth = ropeWidth; // 시작 부분 줄의 너비 설정
        lineRenderer.endWidth = ropeWidth; // 끝 부분 줄의 너비 설정
        Vector3[] segmentPositions = new Vector3[segments.Count]; // 세그먼트 위치 배열 생성
        for (int i = 0; i < segments.Count; i++)
        {
            segmentPositions[i] = segments[i].position; // 세그먼트 위치를 배열에 저장
        }
        lineRenderer.positionCount = segmentPositions.Length; // 라인 렌더러의 포지션 개수 설정
        lineRenderer.SetPositions(segmentPositions); // 라인 렌더러에 포지션 설정
    }

    // 제약을 적용하는 메서드
    private void ApplyConstraint()
    {
        segments[0].position = startTransform.position; // 첫 번째 세그먼트 위치를 시작 위치로 고정

        for (int i = 0; i < segments.Count - 1; i++)
        {
            float distance = (segments[i].position - segments[i + 1].position).magnitude; // 세그먼트 간 거리 계산
            float difference = segmentLength - distance; // 거리 차이 계산
            Vector3 dir = (segments[i + 1].position - segments[i].position).normalized; // 방향 벡터 계산

            Vector3 movement = dir * difference; // 이동 벡터 계산

            if (i == 0)
            {
                segments[i + 1].position += movement; // 첫 번째 세그먼트의 다음 세그먼트 이동
            }
            else
            {
                segments[i].position -= movement * 0.5f; // 현재 세그먼트 이동
                segments[i + 1].position += movement * 0.5f; // 다음 세그먼트 이동
            }
        }

        // 마지막 세그먼트를 endTransform 위치로 고정
        if (endTransform != null)
        {
            segments[segments.Count - 1].position = endTransform.position;
        }
    }

    // 세그먼트를 업데이트하는 메서드
    private void UpdateSegments()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].velocity = segments[i].position - segments[i].previousPos; // 속도 계산
            segments[i].previousPos = segments[i].position; // 이전 위치 저장
            segments[i].position += gravity * Time.fixedDeltaTime * Time.fixedDeltaTime; // 중력에 의한 위치 변화 계산
            segments[i].position += segments[i].velocity; // 속도에 의한 위치 변화 계산
        }
    }

    // 세그먼트 클래스
    public class Segment
    {
        public Vector3 previousPos; // 이전 위치
        public Vector3 position; // 현재 위치
        public Vector3 velocity; // 속도

        public Segment(Vector3 _position)
        {
            previousPos = _position; // 이전 위치 초기화
            position = _position; // 현재 위치 초기화
            velocity = Vector3.zero; // 속도 초기화
        }
    }
}
