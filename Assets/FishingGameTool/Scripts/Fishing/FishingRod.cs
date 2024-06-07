using UnityEngine;
using System;
using FishingGameTool.Fishing.Line;
using FishingGameTool.CustomAttribute;

// FishingGameTool.Fishing.Rod 네임스페이스에 정의됨
namespace FishingGameTool.Fishing.Rod
{
    // 이 컴포넌트를 게임 오브젝트에 추가할 수 있도록 함
    [AddComponentMenu("Fishing Game Tool/Fishing Rod")]
    // 이 컴포넌트를 사용할 때 Animator와 LineRenderer 컴포넌트가 필요함을 명시
    [RequireComponent(typeof(Animator), typeof(LineRenderer))]
    public class FishingRod : MonoBehaviour
    {
        // 직렬화 가능한 클래스: 낚싯줄 설정을 저장
        [Serializable]
        public class FishingLineSettings
        {
            // 낚싯줄이 부착되는 위치를 나타내는 Transform
            public Transform _lineAttachment;
            // 낚싯줄의 포인트 수 범위
            [InfoBox("This is the range of the number of points for the Line Renderer. It is adjusted based on the distance between the line attachment and the float.")]
            public Vector2 _resolutionRange = new Vector2 { x = 40, y = 10 };
            // 중력 시뮬레이션 값을 나타내며, 음수 값임
            [Range(-2f, 0f)]
            public float _simulateGravity = -1f;
            [Space]
            // 낚싯줄 색상
            public Color _color = new Color32(0, 0, 0, 255);
            // 낚싯줄의 너비
            public float _width = 0.005f;
        }

        // 낚싯줄 설정에 대한 헤더 추가
        [BetterHeader("Fishing Line Settings", 20)]
        public FishingLineSettings _line;

        // 낚싯줄 상태에 대한 헤더 추가
        [Space, BetterHeader("Fishing Line Status", 20)]
        public FishingLineStatus _lineStatus;
        public bool _isLineBreakable = true; // 낚싯줄이 끊어질 수 있는지 여부

        // 낚싯대 설정에 대한 헤더 추가
        [Space, BetterHeader("Fishing Rod Settings", 20)]
        public float _baseAttractSpeed = 5f; // 기본 유도 속도
        [InfoBox("Determines the allowable range of bending angles for the fishing rod. It is used to adjust the bending of the rod based on the calculated angles. " +
            "The x-component represents the minimum angle, while the y-component represents the maximum angle.")]
        public Vector2 _angleRange = new Vector2 { x = -110f, y = 110 };

        // 디버그 옵션 표시 버튼 추가
        [Space, AddButton("Show Debug Options", "_showDebugOption")]
        public bool _showDebugOption = false;

        // 디버그 옵션 변수 표시
        [ShowVariable("_showDebugOption")]
        [Space, BetterHeader("For Debug", 20), InfoBox("The variables below allow you to test the fishing rod during configuration. These variables are modified by the main Fishing System script.")]
        public Transform _fishingFloat; // 낚싯대가 던질 대상 (부표)
        [ShowVariable("_showDebugOption")]
        public bool _lootCaught = false; // 낚싯대가 잡은 루트 여부

        #region PRIVATE VARIABLES

        // 비공개 변수들
        private Animator _animator; // Animator 컴포넌트
        private LineRenderer _fishingLineRenderer; // LineRenderer 컴포넌트

        private float _smoothedSimGravity; // 부드럽게 조정된 중력 시뮬레이션 값

        private Vector2 _smoothedBend; // 부드럽게 조정된 낚싯대 휘어짐 값

        #endregion

        // Awake 메서드: 초기화 작업 수행
        private void Awake()
        {
            // 낚싯줄 부착 지점이 설정되지 않은 경우 오류 메시지 출력
            if (_line._lineAttachment == null)
            {
                Debug.LogError("Please add a fishing line attachment!");
                this.enabled = false;
            }

            // Animator 컴포넌트 가져오기
            _animator = GetComponent<Animator>();
            // LineRenderer 컴포넌트 가져오기
            _fishingLineRenderer = GetComponent<LineRenderer>();

            // LineRenderer의 시작 색상 설정
            _fishingLineRenderer.startColor = _line._color;
            // LineRenderer의 끝 색상 설정
            _fishingLineRenderer.endColor = _line._color;
            // LineRenderer의 시작 너비 설정
            _fishingLineRenderer.startWidth = _line._width;
            // LineRenderer의 끝 너비 설정
            _fishingLineRenderer.endWidth = _line._width;
        }

        // Update 메서드: 매 프레임마다 호출
        private void Update()
        {
            // 낚싯대 휘어짐 계산
            CalculateBend();
            // 낚싯줄 렌더링 처리
            FishingLine();
        }

        #region CalculateBend

        // 낚싯대 휘어짐 계산 메서드
        private void CalculateBend()
        {
            Vector2 bend = Vector2.zero;

            // 낚싯줄이 끊어졌거나, 부표가 없거나, 루트를 잡지 못한 경우 휘어짐을 0으로 설정
            if (_lineStatus._isLineBroken || _fishingFloat == null || !_lootCaught)
                bend = Vector2.zero;
            else
                // 부표와 낚싯대 위치의 각도를 계산하고, 휘어짐 범위에 맞게 재매핑
                bend = RemapAngleToBend(CalculateAngles(_fishingFloat.position, transform.position), _angleRange);

            // 휘어짐 속도 설정
            float bendingSpeed = 14f;
            // 부드럽게 휘어짐 값을 조정
            _smoothedBend = Vector2.Lerp(_smoothedBend, bend, Time.deltaTime * bendingSpeed);

            // Animator에 휘어짐 값 설정
            _animator.SetFloat("HorizontalBend", _smoothedBend.x);
            _animator.SetFloat("VerticalBend", _smoothedBend.y);
        }

        // 각도를 휘어짐 값으로 재매핑하는 메서드
        private static Vector2 RemapAngleToBend(Vector2 angle, Vector2 angleRange)
        {
            // 각도를 0과 1 사이의 값으로 재매핑
            float x = Mathf.InverseLerp(angleRange.x, angleRange.y, angle.x);
            float y = Mathf.InverseLerp(angleRange.x, angleRange.y, angle.y);

            // 휘어짐 값으로 재매핑
            float valueX = Mathf.Lerp(-1f, 1f, x);
            float valueY = Mathf.Lerp(-1f, 1f, y);

            Vector2 bend = new Vector2(-valueX, -valueY);

            return bend;
        }

        // 두 위치 사이의 각도를 계산하는 메서드
        private Vector2 CalculateAngles(Vector3 floatPosition, Vector3 position)
        {
            Vector3 dir = floatPosition - position;

            float angleCorrection = 90;
            float angleX = Vector3.Angle(transform.right, dir) - angleCorrection;
            float angleY = Vector3.Angle(transform.up, dir) - angleCorrection;

            Vector2 angle = new Vector2(angleX, angleY);

            return angle;
        }

        #endregion

        #region FishingLine

        // 낚싯줄 렌더링 처리 메서드
        private void FishingLine()
        {
            // 낚싯줄이 끊어졌거나, 부표가 없는 경우 낚싯줄 포인트 수를 0으로 설정
            if (_lineStatus._isLineBroken || _fishingFloat == null)
            {
                _fishingLineRenderer.positionCount = 0;
                return;
            }

            // 부착 지점과 부표 사이의 거리 계산
            float distance = Vector3.Distance(_line._lineAttachment.position, _fishingFloat.position);
            // 거리에 따른 낚싯줄 해상도 계산
            int resolution = CalculateLineResolution(distance, _line._resolutionRange);

            _lineStatus._currentLineLength = distance;
            _fishingLineRenderer.positionCount = resolution;

            // 낚싯줄의 각 포인트 위치 계산 및 설정
            for (int i = 0; i < resolution; i++)
            {
                float t = i / (float)resolution;
                Vector3 position = CalculatePointOnCurve(t, _line._lineAttachment.position, _fishingFloat.position, _lootCaught, _line._simulateGravity);
                _fishingLineRenderer.SetPosition(i, position);
            }
        }

        // 거리에 따른 낚싯줄 해상도를 계산하는 메서드
        private static int CalculateLineResolution(float distance, Vector2 resolutionRange)
        {
            float minDis = 1f;
            float maxDis = 20f;

            float x = Mathf.InverseLerp(minDis, maxDis, distance);
            float value = Mathf.Lerp(resolutionRange.x, resolutionRange.y, x);

            return (int)value;
        }

        // 곡선 위의 포인트를 계산하는 메서드
        private Vector3 CalculatePointOnCurve(float t, Vector3 attachmentPosition, Vector3 floatPosition, bool lootCaught, float simulateGravity)
        {
            Vector3 pointA = attachmentPosition;
            Vector3 pointB = floatPosition;

            float lineTensionSpeed = 2f;
            // 부드럽게 중력 시뮬레이션 값을 조정
            _smoothedSimGravity = Mathf.Lerp(_smoothedSimGravity, lootCaught == true ? 0f : simulateGravity, Time.deltaTime * lineTensionSpeed);

            // 제어점을 계산하여 베지어 곡선 상의 포인트를 계산
            Vector3 controlPoint = Vector3.Lerp(pointA, pointB, 0.5f) + Vector3.up * _smoothedSimGravity;
            Vector3 pointOnCurve = CalculateBezier(pointA, controlPoint, pointB, t, floatPosition);

            return pointOnCurve;
        }

        // 베지어 곡선을 계산하는 메서드
        private Vector3 CalculateBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t, Vector3 floatPosition)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 point = uuu * p0; // uuu * p0
            point += 3 * uu * t * p1; // 3 * uu * t * p1
            point += 3 * u * tt * p2; // 3 * u * tt * p2
            point += ttt * floatPosition; // ttt * p3

            return point;
        }

        #endregion

        /// <summary>
        /// 낚싯줄의 현재 상태를 계산하는 메서드
        /// </summary>
        /// <param name="attractInput">낚싯줄이 유도 중인지 여부</param>
        /// <param name="lootWeight">루트의 무게</param>
        /// <returns>낚싯줄의 현재 상태를 나타내는 FishingLineStatus 객체</returns>
        public FishingLineStatus CalculateLineLoad(bool attractInput, float lootWeight, int lootTier)
        {
            Vector3 dir = _fishingFloat.position - transform.position;
            float angle = Vector3.Angle(transform.forward, dir);

            angle = angle > _angleRange.y ? _angleRange.y : angle;

            if (attractInput)
            {
                float loadDecreaseFactor = 4f;
                float calculatedLootWeight = (lootWeight - (lootWeight / lootTier)) <= 0f ? 1f : (lootWeight - (lootWeight / lootTier));

                _lineStatus._currentLineLoad += ((angle * calculatedLootWeight) * Time.deltaTime) / loadDecreaseFactor;
                _lineStatus._currentLineLoad = _lineStatus._currentLineLoad > _lineStatus._maxLineLoad ? _lineStatus._maxLineLoad : _lineStatus._currentLineLoad;
            }
            else
            {
                _lineStatus._currentOverLoad = 0f;
                float loadReduction = 5f;
                _lineStatus._currentLineLoad -= loadReduction * Time.deltaTime;
                _lineStatus._currentLineLoad = _lineStatus._currentLineLoad < 0f ? 0f : _lineStatus._currentLineLoad;
            }

            if (_lineStatus._currentLineLoad == _lineStatus._maxLineLoad)
            {
                _lineStatus._currentOverLoad += Time.deltaTime;

                if (_lineStatus._currentOverLoad >= _lineStatus._overLoadDuration)
                {
                    if (_isLineBreakable)
                        _lineStatus._isLineBroken = true;

                    FishingSystem[] fishingSystem = FindObjectsOfType<FishingSystem>();

                    if (fishingSystem.Length > 1)
                        Debug.LogWarning("There is more than one object on the scene containing the Fishing System component. " +
                            "Please remove the other components containing Fishing System!");
                    else
                        fishingSystem[0].ForceStopFishing();
                }
            }

            _lineStatus._attractFloatSpeed = CalculateAttractSpeed(angle, _angleRange, _lineStatus._currentLineLoad, _lineStatus._maxLineLoad, _baseAttractSpeed, lootTier);

            return _lineStatus;
        }

        // 유도 속도를 계산하는 메서드
        private float CalculateAttractSpeed(float angle, Vector2 angleRange, float currentLineLoad, float maxLineLoad, float baseAttractSpeed, int lootTier)
        {
            float normalizeAngle = angle / angleRange.y;
            float attractBonus = CalculateAttractBonus(currentLineLoad, maxLineLoad, lootTier);
            float speedBonus = normalizeAngle * attractBonus;
            float attractSpeed = baseAttractSpeed + speedBonus;

            return attractSpeed;
        }

        // 유도 보너스를 계산하는 메서드
        private static float CalculateAttractBonus(float currentLineLoad, float maxLineLoad, int lootTier)
        {
            float[] attractBonusMultiplier = { 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };

            float x = Mathf.InverseLerp(0f, maxLineLoad, currentLineLoad);
            float value = Mathf.Lerp(1f, currentLineLoad * attractBonusMultiplier[lootTier], x);

            return value;
        }

        // 루트를 잡았는지 여부를 설정하는 메서드
        public void LootCaught(bool value)
        {
            _lootCaught = value;
        }

        // 낚시를 종료하는 메서드
        public void FinishFishing()
        {
            _lineStatus._attractFloatSpeed = 0f;
            _lineStatus._currentLineLoad = 0f;
            _lineStatus._currentOverLoad = 0f;
            _lootCaught = false;
        }
    }
}
