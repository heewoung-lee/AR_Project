using FishingGameTool.Fishing.BaitData;
using FishingGameTool.Fishing.CatchProbability;
using FishingGameTool.Fishing.Float;
using FishingGameTool.Fishing.Line;
using FishingGameTool.Fishing.LootData;
using FishingGameTool.Fishing.Rod;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace FishingGameTool.Fishing
{
    // 물고기를 잡는 방법을 지정하는 열거형: SpawnItem 또는 InvokeEvent
    public enum LootCatchType
    {
        SpawnItem,
        InvokeEvent
    };

    // FishingSystem 스크립트에 컴포넌트 메뉴 추가
    [AddComponentMenu("Fishing Game Tool/Fishing System")]
    public class FishingSystem : MonoBehaviour, InputSystem.IUIActions
    {
        // 물고기 잡기 이벤트를 처리하기 위한 직렬화된 클래스
        [System.Serializable]
        public class LootCatchEvent
        {
            public int _invokeCalls = 1; // 이벤트 호출 횟수
            public UnityEvent _event; // Unity 이벤트 객체
        }

        // 고급 설정을 위한 직렬화된 클래스
        [System.Serializable]
        public class AdvancedSettings
        {
            public FishingLootData _caughtLootData; // 잡은 물고기 데이터 저장
            public CatchProbabilityData _catchProbabilityData; // 잡기 확률 데이터
            public bool _caughtLoot = false; // 물고기를 잡았는지 여부
            public float _returnSpeedWithoutLoot = 3f; // 물고기 없이 돌아오는 속도
            public float _catchCheckInterval = 1f; // 잡기 확인 간격
            public float _lootWeight; // 물고기의 무게
        }

        private const int THRESHOLD = 5; //드래그 민감도
        public FishingRod _fishingRod; // 낚싯대 객체
        public LayerMask _fishingLayer; // 낚시 레이어 마스크
        public FishingBaitData _bait; // 미끼 데이터
        public LootCatchType _lootCatchType; // 물고기 잡기 타입
        public Ease FishingCompleteActionType;
        [HideInInspector]
        public bool _showCatchEvent = false; // 물고기 잡기 이벤트 표시 여부

        public LootCatchEvent _lootCatchEvent; // 물고기 잡기 이벤트
        public GameObject _fishingFloatPrefab; // 낚싯대 프리팹
        public float _maxCastForce = 20f; // 최대 던지기 힘
        public float _forceChargeRate = 6f; // 힘 충전 속도
        public float _currentCastForce; // 현재 던지기 힘
        public float _spawnFloatDelay = 0.3f; // 낚싯대 생성 지연 시간
        public float _catchDistance = 3.5f; // 잡기 최소 거리
        public float powerChaging = 2f;

        public bool _showAdvancedSettings = false; // 고급 설정 표시 여부

        public AdvancedSettings _advanced; // 고급 설정 객체

        [HideInInspector]
        private bool _attractInput; // 미끼 당기기 입력 여부
        public bool attractInput { get => _attractInput; } // 미끼 당기기 입력 접근자
        [HideInInspector]
        private bool _castInput = false; // 던지기 입력 여부
        public bool castInput { get => _castInput; } // 던지기 입력 접근자
        [HideInInspector]
        private bool _castFloat = false; // 던지기 동작 여부
        public bool castFloat { get => _castFloat; } // 던지기 동작 접근자

        public event Func<float> showPowerbarEvent; // 파워바 이벤트
        //public event Action<Transform> setLootCamera;
        public event Func<GameObject> setLineEndPoint;
        public event System.Action<GameObject, GameObject, Camera, Image> viewFishCaughtButtonEvent;

        private Vector2 _startPos; // 시작 위치
        private Vector2 _endPos;

        private InputSystem _inputSystem; // 입력 시스템
        private float _dragDistance = 0; // 드래그 거리
        public float dragDistance { get => _dragDistance; } // 드래그 거리 접근자
        private bool isCheckedMouseDraged = false;
        private float _lastPositionY = 0f; //드래그에서만 작동되게끔 마우스 포지션의 위치를 알기위한 필드
        private GameObject _fishingrope;
        private Transform _bigCatchWord;
        private FishLoadEndPosition _fishLoadEndPosition;

        public AnimationCurve _fishingCatchActionEase;
        public event Action castingMontion;
        public event Action afterCatchingAFishEvent;
        public Action BaitCountDecreaseEvent; //낚시대 던질때 껴진 미끼의 갯수 감소 이벤트
        public Action BaitInventoryDeleteEvent;
        #region PRIVATE VARIABLES

        private float _catchCheckIntervalTimer; // 잡기 확인 간격 타이머
        private float _randomSpeedChangerTimer = 2f; // 랜덤 속도 변경 타이머
        private float _randomSpeedChanger = 1f; // 랜덤 속도 변경 값
        private float _finalSpeed; // 최종 속도
        [SerializeField] private Camera _arMainCamera;
        [SerializeField] private Camera _catchLootCamera;
        [SerializeField] private Canvas _CharactorUI;



        private bool _isCheckedLoadScene = false;


        public bool isCheckedLoadScene { get => _isCheckedLoadScene; }


        [SerializeField] private Toggle _testToggle; //낚시대를 던지자마자 물고기들이 물어버리는 테스트용 토글
        [SerializeField] private Toggle _testToggleCatch; //켜져 있으면 물었던 물고기가 바로잡히는 테스트용 토글

        FishingFloatPathfinder _fishingFloatPathfinder = new FishingFloatPathfinder(); // 낚시 찌 경로 찾기 객체

        #endregion
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (_lootCatchType == LootCatchType.InvokeEvent)
                _showCatchEvent = true;
            else
                _showCatchEvent = false;
        }

#endif

        private void Awake()
        {
            _inputSystem = new InputSystem();
            _inputSystem.UI.AddCallbacks(this);
            _inputSystem.Enable();
            _arMainCamera = GetComponentInParent<Camera>();
            _fishingrope = Resources.Load<GameObject>("HW/Prefabs/Rope");
            _catchLootCamera = GameObject.Find("CatchLootCamera").GetComponent<Camera>();
            _CharactorUI = GameObject.Find("CharacterUI").GetComponent<Canvas>();
            _bigCatchWord = _CharactorUI.transform.Find("Bigcatchword").GetComponent<Transform>();
            _fishLoadEndPosition = GetComponentInChildren<FishLoadEndPosition>();
            _catchCheckIntervalTimer = _advanced._catchCheckInterval;
        }

        // Update 메서드: 매 프레임마다 호출되며 찌 당기기 및 던지기 동작 수행
        private void Update()
        {
            if (_fishingRod != null)
            {
                AttractFloat();
                CastFloat();
            }
        }

        #region AttractFloat

        // 찌 당기기 메서드
        private void AttractFloat()
        {
            // 낚시대의 낚시 찌가 null인지 확인하고 null이면 함수 종료
            if (_fishingRod._fishingFloat == null)
                return;

            // 낚시 찌의 표면 상태를 체크하여 substrateType에 저장
            SubstrateType substrateType = _fishingRod._fishingFloat.GetComponent<FishingFloat>().CheckSurface(_fishingLayer);
            // 만약 표면 상태가 물이면 낚시 찌의 위치와 변환 위치를 이용하여 loot을 체크
            if (substrateType == SubstrateType.Water)
                _advanced._caughtLoot = CheckingLoot(_advanced._caughtLoot, _bait, _advanced._catchProbabilityData, transform.position, _fishingRod._fishingFloat.position);

            // 낚시 줄의 길이를 제한하는 함수 호출
            LineLengthLimitation(_fishingRod._fishingFloat.gameObject, transform.position, _fishingRod._lineStatus, substrateType);

            // 사용자가 당기기 입력을 하였고, 표면 상태가 공중이며 loot을 잡지 못한 경우
            if (_attractInput && substrateType == SubstrateType.InAir && !_advanced._caughtLoot)
            {
                // 낚시 찌를 파괴하고 당기기 입력을 false로 설정, 낚시 찌를 null로 설정
                Destroy(_fishingRod._fishingFloat.gameObject);
                _attractInput = false;
                _fishingRod._fishingFloat = null;
                return;
            }
            //// 사용자가 당기기 입력을 하였고, 표면 상태가 땅이며 loot을 잡지 못한 경우
            //else if (_attractInput && substrateType == SubstrateType.Land && !_advanced._caughtLoot)
            //{
            //    // 변환 위치와 낚시 찌의 위치 사이의 거리를 계산
            //    float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);
            //    // 속도를 계산 (loot이 없을 때의 반환 속도에 120을 곱하고 deltaTime을 곱함)
            //    float speed = _advanced._returnSpeedWithoutLoot * 120f * Time.deltaTime;

            //    // 방향을 계산
            //    Vector3 direction = (transform.position - _fishingRod._fishingFloat.position).normalized;

            //    // 낚시 찌의 Rigidbody 컴포넌트를 가져와서 속도 설정
            //    Rigidbody fishingFloatRB = _fishingRod._fishingFloat.GetComponent<Rigidbody>();
            //    fishingFloatRB.velocity = direction * speed;

            //    // 거리가 설정된 거리 이하이면 낚시 찌를 파괴하고 null로 설정
            //    if (distance <= _catchDistance)
            //    {
            //        Destroy(_fishingRod._fishingFloat.gameObject);
            //        _fishingRod._fishingFloat = null;
            //        return;
            //    }
            //}
            // 사용자가 당기기 입력을 하였고, 표면 상태가 물이며 loot을 잡지 못한 경우
            else if (_attractInput && substrateType == SubstrateType.Water && !_advanced._caughtLoot)
            {
                // 변환 위치와 낚시 찌의 위치 사이의 거리를 계산
                float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);

                // 낚시 찌의 당기기 동작을 설정
                _fishingFloatPathfinder.FloatBehavior(null, _fishingRod._fishingFloat, transform.position, _fishingRod._lineStatus._maxLineLength,
                   _advanced._returnSpeedWithoutLoot, _attractInput, _fishingLayer);

                // 거리가 설정된 거리 이하이면 낚시 찌를 파괴하고 null로 설정
                if (distance <= _catchDistance)
                {
                    Destroy(_fishingRod._fishingFloat.gameObject);
                    _fishingRod._fishingFloat = null;
                    _fishLoadEndPosition.CreateFishingRope();
                    return;
                }
            }
            // loot을 잡았고, 낚시 찌가 null이 아닌 경우
            else if (_advanced._caughtLoot && _fishingRod._fishingFloat != null)
            {
                // 낚시대의 LootCaught 메서드 호출
                _fishingRod.LootCaught(_advanced._caughtLoot);

                // loot 데이터가 null인 동안 루프 실행
                while (_advanced._caughtLootData == null)
                {
                    // 물체로부터 loot 데이터를 가져와서 리스트에 저장
                    List<FishingLootData> lootDataList = _fishingRod._fishingFloat.GetComponent<FishingFloat>().GetLootDataFormWaterObject();
                    // bait과 loot 데이터 리스트를 이용해 loot 선택
                    _advanced._caughtLootData = ChooseFishingLoot(_bait, lootDataList);

                    if (_advanced._caughtLootData != null)
                    {
                        // loot의 무게를 랜덤 범위 내에서 설정
                        float lootWeight = UnityEngine.Random.Range(_advanced._caughtLootData._weightRange._minWeight, _advanced._caughtLootData._weightRange._maxWeight);
                        _advanced._lootWeight = lootWeight;

                        // bait을 null로 설정
                        _bait = null;

                        BaitCountDecreaseEvent?.Invoke();
                        BaitInventoryDeleteEvent?.Invoke();
                        //TODO : BaitObject에 넣었던 미끼의 카운터를 --할것; --해서 0이 되면 그자리를 Null로 설정
                    }
                }

                // loot이 있는 상태로 당기기 동작 설정
                AttractWithLoot(_advanced._caughtLootData, _fishingRod._fishingFloat, transform.position, _fishingLayer, _attractInput, _advanced._lootWeight, _fishingRod);

                if (_attractInput && _fishingRod._fishingFloat != null)
                {
                    float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);
                    if (_testToggleCatch.isOn)
                    {
                        distance = _catchDistance;
                    }
                    // 변환 위치와 낚시 찌의 위치 사이의 거리를 계산


                    // 거리가 설정된 거리 이하이면 loot을 잡고 낚시 찌를 파괴
                    if (distance <= _catchDistance)
                    {
                        GrabLoot(_advanced._caughtLootData, _fishingRod._fishingFloat.position, transform.position, out GameObject lootprefab);
                        CatchFishingScene(lootprefab, _catchLootCamera);//Todo: 카메라 연출 실행
                        Destroy(_fishingRod._fishingFloat.gameObject);
                        _fishingRod._fishingFloat = null;
                        _advanced._caughtLoot = false;
                        _advanced._caughtLootData = null;
                        _fishingRod.FinishFishing();
                        // 경로 데이터 초기화
                        _fishingFloatPathfinder.ClearPathData();
                        return;
                    }
                }
            }
        }


        #endregion

        #region LineLengthLimitation
        // 낚싯줄 길이 제한 메서드
        private void LineLengthLimitation(GameObject fishingFloat, Vector3 transformPosition, FishingLineStatus fishingLineStatus, SubstrateType substrateType)
        {
            if (fishingLineStatus._currentLineLength > fishingLineStatus._maxLineLength && substrateType != SubstrateType.Water)
            {
                Vector3 fishingFloatPosition = fishingFloat.transform.position;
                Vector3 direction = (transformPosition - fishingFloatPosition).normalized;

                Rigidbody fishingFloatRB = fishingFloat.GetComponent<Rigidbody>();

                float speed = (fishingLineStatus._currentLineLength - fishingLineStatus._maxLineLength) / Time.deltaTime;
                float maxSpeed = 5f;
                float clampedSpeed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

                fishingFloatRB.velocity = direction * clampedSpeed;
            }
        }

        #endregion

        #region AttractWithLoot

        // 물고기를 잡고 있을 때의 찌 당기기 메서드
        private void AttractWithLoot(FishingLootData lootData, Transform fishingFloatTransform, Vector3 transformPosition, LayerMask fishingLayer, bool attractInput,
            float lootWeight, FishingRod fishingRod)
        {
            float lootSpeed = CalculateLootSpeed(lootData, lootWeight);
            float attractSpeed = CalculateAttractSpeed(fishingRod, attractInput, lootWeight, (int)lootData._lootTier) * powerChaging;
            _finalSpeed = Mathf.Lerp(_finalSpeed, attractInput == true ? CalculateFinalAttractSpeed(lootSpeed, attractSpeed, lootData) : lootSpeed, 3f * Time.deltaTime);

            _fishingFloatPathfinder.FloatBehavior(lootData, fishingFloatTransform, transformPosition, fishingRod._lineStatus._maxLineLength, _finalSpeed, attractInput, fishingLayer);
        }

        // 물고기 속도 계산
        private float CalculateLootSpeed(FishingLootData lootData, float lootWeight)
        {
            float[] speedMultipliersByTier = { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f };
            float baseSpeed = 1.4f;

            int tier = (int)lootData._lootTier;

            _randomSpeedChangerTimer -= Time.deltaTime;
            if (_randomSpeedChangerTimer < 0)
            {
                _randomSpeedChanger = UnityEngine.Random.Range(1f, 3f);
                _randomSpeedChangerTimer = UnityEngine.Random.Range(2f, 4f);
            }

            float lootSpeed = (baseSpeed + lootWeight * 0.1f * speedMultipliersByTier[tier]) * _randomSpeedChanger;
            return lootSpeed;
        }

        // 당기기 속도 계산
        private float CalculateAttractSpeed(FishingRod fishingRod, bool attractInput, float lootWeight, int lootTier)
        {
            FishingLineStatus fishingLineStatus = fishingRod.CalculateLineLoad(_attractInput, lootWeight, lootTier);
            float attractionSpeed = fishingLineStatus._attractFloatSpeed;

            return attractionSpeed;
        }

        // 최종 당기기 속도 계산
        private float CalculateFinalAttractSpeed(float lootSpeed, float attractSpeed, FishingLootData lootData)
        {
            int tier = (int)lootData._lootTier;
            float[] speedFactorByTier = { 1.2f, 1.0f, 0.8f, 0.6f, 0.5f };

            float finalAttractSpeed = (attractSpeed - lootSpeed) * speedFactorByTier[tier];
            finalAttractSpeed = finalAttractSpeed < 2f ? 2f : finalAttractSpeed;

            return finalAttractSpeed;
        }

        #endregion

        #region GrabLoot
        // 물고기를 잡는 메서드
        private void GrabLoot(FishingLootData lootData, Vector3 fishingFloatPosition, Vector3 transformPosition, out GameObject lootprefab)
        {
            switch (_lootCatchType)
            {
                case LootCatchType.SpawnItem:

                    if (lootData._lootPrefab == null)
                    {
                        Debug.LogError("No loot prefab added!");
                        lootprefab = null;
                        return;
                    }
                    SpawnLootItem(lootData._lootPrefab, fishingFloatPosition, transformPosition, out GameObject lootObject);
                    lootObject.GetComponent<FishScripts>().fishNumber = lootData.FishNumber;
                    lootprefab = lootObject;
                    return;

                case LootCatchType.InvokeEvent:

                    Debug.LogWarning("이 기능은 개발 중입니다. 버전 1.0에서는 작동하지 않습니다.");
                    lootprefab = null;
                    return;

                default:
                    lootprefab = null;
                    break;
            }
        }

        // 물고기 아이템 생성 메서드
        private void SpawnLootItem(GameObject lootPrefab, Vector3 fishingFloatPosition, Vector3 transformPosition, out GameObject lootObject)
        {
            Vector3 direction = ((transformPosition - fishingFloatPosition) + (Vector3.up * 3f)).normalized;
            Vector3 spawnPosition = fishingFloatPosition + new Vector3(0f, 1f, 0f);

            GameObject spawnedLootPrefab = Instantiate(lootPrefab, spawnPosition, lootPrefab.transform.localRotation);
            lootObject = spawnedLootPrefab;
            float distance = Vector3.Distance(fishingFloatPosition, transformPosition);
            float desiredTime = 0.8f;

            float force = distance / desiredTime;

            spawnedLootPrefab.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
        }

        #endregion

        #region CheckingLoot

        // 물고기를 잡았는지 확인하는 메서드
        private bool CheckingLoot(bool caughtLoot, FishingBaitData baitData, CatchProbabilityData catchProbabilityData,
            Vector3 transformPosition, Vector3 fishingFloatPosition)
        {

            if (_testToggle.isOn)
                return true;

            if (caughtLoot)
                return true;

            bool caught = false;

            _catchCheckIntervalTimer -= Time.deltaTime;

            if (_catchCheckIntervalTimer <= 0)
            {
                caught = CheckLootIsCatch(baitData, catchProbabilityData, transformPosition, fishingFloatPosition);
                _catchCheckIntervalTimer = _advanced._catchCheckInterval;
            }

            return caught;
        }

        // 물고기를 잡았는지 확인하는 세부 메서드
        private bool CheckLootIsCatch(FishingBaitData baitData, CatchProbabilityData catchProbabilityData,
            Vector3 transformPosition, Vector3 fishingFloatPosition)
        {
            float distance = Vector3.Distance(transformPosition, fishingFloatPosition);
            float minSafeFishingDistanceFactor = 10f;

            int chanceVal = UnityEngine.Random.Range(1, 100);

            int commonProbability = 1;
            int uncommonProbability = 5;
            int rareProbability = 10;
            int epicProbability = 20;
            int legendaryProbability = 45;

            if (catchProbabilityData != null)
            {
                commonProbability = catchProbabilityData._commonProbability;
                uncommonProbability = catchProbabilityData._uncommonProbability;
                rareProbability = catchProbabilityData._rareProbability;
                epicProbability = catchProbabilityData._epicProbability;
                legendaryProbability = catchProbabilityData._legendaryProbability;
                minSafeFishingDistanceFactor = catchProbabilityData._minSafeFishingDistanceFactor;
            }

            commonProbability = CalculateProbabilityValueByCastDistance(commonProbability, distance, minSafeFishingDistanceFactor);
            uncommonProbability = CalculateProbabilityValueByCastDistance(uncommonProbability, distance, minSafeFishingDistanceFactor);
            rareProbability = CalculateProbabilityValueByCastDistance(rareProbability, distance, minSafeFishingDistanceFactor);
            epicProbability = CalculateProbabilityValueByCastDistance(epicProbability, distance, minSafeFishingDistanceFactor);
            legendaryProbability = CalculateProbabilityValueByCastDistance(legendaryProbability, distance, minSafeFishingDistanceFactor);

            if (baitData == null)
            {
                if (chanceVal <= commonProbability)
                    return true;
                else
                    return false;
            }
            else
            {
                switch (baitData._baitTier)
                {
                    case BaitTier.Uncommon:

                        if (chanceVal <= uncommonProbability)
                            return true;
                        else
                            return false;

                    case BaitTier.Rare:

                        if (chanceVal <= rareProbability)
                            return true;
                        else
                            return false;

                    case BaitTier.Epic:

                        if (chanceVal <= epicProbability)
                            return true;
                        else
                            return false;

                    case BaitTier.Legendary:

                        if (chanceVal <= legendaryProbability)
                            return true;
                        else
                            return false;
                }
            }

            return false;
        }

        // 던지기 거리로 확률 값을 계산하는 메서드
        private static int CalculateProbabilityValueByCastDistance(float probability, float distance, float minSafeFishingDistanceFactor)
        {
            float minValue = 0.3f;
            float maxValue = 1f;

            float x = Mathf.InverseLerp(0, minSafeFishingDistanceFactor, distance);
            float value = Mathf.Lerp(minValue, maxValue, x);

            probability = probability * value;

            return (int)probability;
        }

        #endregion

        #region ChooseFishingLoot

        // 낚시 물고기를 선택하는 메서드
        private FishingLootData ChooseFishingLoot(FishingBaitData baitData, List<FishingLootData> lootDataList)
        {
            for (int i = 0; i < lootDataList.Count; i++)
            {
                FishingLootData temp = lootDataList[i];
                int randomIndex = UnityEngine.Random.Range(i, lootDataList.Count);
                lootDataList[i] = lootDataList[randomIndex];
                lootDataList[randomIndex] = temp;
            }

            float totalRarity = CalculateTotalRarity(lootDataList);
            List<float> lootRarityList = CalculatePercentageRarity(lootDataList, totalRarity);

            int baitTier = 0;

            if (baitData != null)
                baitTier = (int)baitData._baitTier + 1;

            float chanceVal = UnityEngine.Random.Range(1f, 100f);
            float addedRarity = 0f;

            for (int i = 0; i < lootRarityList.Count; i++)
            {
                addedRarity += lootRarityList[i];

                if (chanceVal <= addedRarity)
                {
                    if (baitTier >= (int)lootDataList[i]._lootTier)
                        return lootDataList[i];
                    else
                        return null;
                }
            }

            return null;
        }

        // 총 희귀도를 계산하는 메서드
        private float CalculateTotalRarity(List<FishingLootData> lootDataList)
        {
            float totalRarity = 0;

            foreach (var lootData in lootDataList)
            {
                totalRarity += lootData._lootRarity;
            }

            return totalRarity;
        }

        // 희귀도의 백분율을 계산하는 메서드
        private List<float> CalculatePercentageRarity(List<FishingLootData> lootDataList, float totalRarity)
        {
            List<float> lootRarityList = new List<float>();

            foreach (var lootData in lootDataList)
            {
                float percentageRarity = (lootData._lootRarity / totalRarity) * 100f;
                lootRarityList.Add(percentageRarity);
            }

            return lootRarityList;
        }

        #endregion

        #region CastFloat

        // 찌 던지기 메서드
        private void CastFloat()
        {
            // 낚싯대가 이미 던져졌거나, 낚싯대 던지기 동작이 이미 실행 중이거나, 낚싯줄이 끊어진 상태라면 동작하지 않음
            if (_fishingRod._fishingFloat != null || _castFloat)
                return;

            // 던지기 입력이 활성화된 상태라면 던지기 힘을 계산
            if (_castInput)
            {
                _currentCastForce = CalculateCastForce(_currentCastForce, _maxCastForce, _forceChargeRate);
                //Debug.Log("파워: " + _castingPower);
            }
            else if (!_castInput && _currentCastForce != 0f) // 던지기 입력이 비활성화되고 던지기 힘이 0이 아니라면 던지기 동작을 시작
            {
                //Debug.Log("파워가 남아있다면 여기로 들어옴");
                Vector3 spawnPoint = _fishingRod._line._lineAttachment.position; // 낚싯대의 시작 위치
                Vector3 castDirection = transform.forward + Vector3.up; // 던지는 방향
                // 던지기 지연을 시작
                StartCoroutine(CastingDelay(_spawnFloatDelay, castDirection, spawnPoint, _currentCastForce, _fishingFloatPrefab));
                StartCoroutine(waitDestroyRope());
                _currentCastForce = 0f; // 던지기 힘 초기화
            }
        }


        IEnumerator waitDestroyRope()
        {
            yield return new WaitForSeconds(0.5f);
            castingMontion?.Invoke();
        }
        // 던지기 지연 메서드
        private IEnumerator CastingDelay(float delay, Vector3 castDirection, Vector3 spawnPoint, float castForce, GameObject fishingFloatPrefab)
        {
            _castFloat = true; // 던지기 동작 중임을 표시

            yield return new WaitForSeconds(delay); // 지정된 지연 시간 동안 대기
            _fishingRod._fishingFloat = Cast(castDirection, spawnPoint, castForce, fishingFloatPrefab); // 던지기 실행
            _castFloat = false; // 던지기 동작 완료
        }

        // 찌 던지기 메서드
        private Transform Cast(Vector3 castDirection, Vector3 spawnPoint, float castForce, GameObject fishingFloatPrefab)
        {
            // 낚싯대 프리팹을 지정된 위치에 생성
            GameObject spawnedFishingFloat = Instantiate(fishingFloatPrefab, spawnPoint, Quaternion.identity);
            // 생성된 낚싯대에 힘을 가하여 던지기 동작 수행
            spawnedFishingFloat.GetComponent<Rigidbody>().AddForce(castDirection * castForce, ForceMode.Impulse);

            // 생성된 낚싯대의 Transform을 반환
            return spawnedFishingFloat.transform;
        }

        // 던지기 힘 계산 메서드
        private float CalculateCastForce(float currentCastForce, float maxCastForce, float forceChargeRate)
        {
            // 현재 던지기 힘을 힘 충전 속도에 따라 증가
            currentCastForce += forceChargeRate * Time.deltaTime;
            // 던지기 힘이 최대 힘을 초과하지 않도록 제한
            currentCastForce = currentCastForce > maxCastForce ? maxCastForce : currentCastForce;

            return currentCastForce; // 계산된 던지기 힘 반환
        }

        #endregion

        // 낚시 강제 종료 메서드
        public void ForceStopFishing()
        {
            Destroy(_fishingRod._fishingFloat.gameObject);
            _fishingRod._fishingFloat = null;
            _advanced._caughtLoot = false;
            _advanced._caughtLootData = null;
            _fishingRod.FinishFishing();
            _fishingFloatPathfinder.ClearPathData();
        }

        // 사용자 정의 잡기 확률 데이터 추가 메서드
        public void AddCustomCatchProbabilityData(CatchProbabilityData catchProbabilityData)
        {
            _advanced._catchProbabilityData = catchProbabilityData;
        }

        // 낚싯줄 고정 메서드
        public void FixFishingLine()
        {
            if (_fishingRod._lineStatus._isLineBroken)
                _fishingRod._lineStatus._isLineBroken = false;
        }

        public void CatchFishingScene(GameObject lootObject, Camera catchLootCamera)
        {
            //카메라 컬링 레이어를 통해 ARLayer를 가지고 있는 애들이면 렌더링 안하도록 설정
            //2.CatchlootCamera가 활성화 되면서 lootObject를 따라다니도록 설정
            catchLootCamera.enabled = true;
            catchLootCamera.GetComponent<LookatFish>().enabled = true;
            CatchingAnimation(lootObject, catchLootCamera.transform);

            //Transform LinePoint = GameObject.Find("LinePoint").GetComponent<Transform>();
            //LinePoint.transform.position = new Vector3(lootObject.transform.position.x, lootObject.transform.position.y+4, lootObject.transform.position.z);
            //lootObject.transform.SetParent(LinePoint); //루트 카메라 밑에 LinePoint 자식으로 설정
            //lootObject.name = "CaughtFish";
            //LinePoint.AddComponent<FishingAnimation>(); //낚는 애니메이션 시작
            //viewFishCaughtButtonEvent?.Invoke(); //Ok버튼 나오도록


            //LinePoint.transform.localPosition = Vector3.zero;
            //Transform FishMousePosition = lootObject.transform.Find("MousePosition").GetComponent<Transform>();
            ////FishMousePosition.SetParent(lootObject.transform);
            ////FishMousePosition.transform.localPosition = Vector3.zero;
            ////FishMousePosition.transform.localRotation = Quaternion.identity;
            ////lootObject.transform.Rotate(new Vector3(-90f, 0, -90f)); //물고기가 정면을 보게끔 수정
        }

        public void CatchingAnimation(GameObject lootobject, Transform catchLootCamera)
        {
            //로프의 끝을 물고기와 연결해야 함.
            GameObject FishingLope = Instantiate(_fishingrope);
            FishingLine fishingLine = FishingLope.GetComponent<FishingLine>();


            fishingLine.startTransform = FishingLope.transform.Find("StartPoint");
            fishingLine.startTransform.transform.position = catchLootCamera.transform.position + Vector3.up * 2;
            fishingLine.startTransform.transform.position += Vector3.right;//낚시줄 위치 조정
            //fishingLine.endTransform = lootobject.transform.Find("MousePosition");//입에 포지션을 설정
            fishingLine.endTransform = lootobject.GetComponentInChildren<FindMousePosition>().transform;


            //ToDO: 물속에 박혀 있다가 물밖으로 끄집어내도록 물속-> 물밖 닷트윈 실행
            lootobject.transform.position -= lootobject.transform.forward * 9f;
            // DOTween 시퀀스를 사용하여 "월척" 애니메이션 실행
            Image yatchaWord = _bigCatchWord.Find("Yatchaword").GetComponent<Image>();
            // 시퀀스 생성
            DG.Tweening.Sequence fishCompingUpSequence = DOTween.Sequence();
            DG.Tweening.Sequence typingSequnence = DOTween.Sequence();


            fishCompingUpSequence
                .Append(lootobject.transform.DOMove(lootobject.transform.position + lootobject.transform.forward * 9f, 0.5f))
                .SetEase(_fishingCatchActionEase)
                .Join(lootobject.transform.DORotate(new Vector3(0, 0, -90), 2f))
                .Append(fishingLine.startTransform.transform.DOMove(fishingLine.startTransform.transform.position -= Vector3.right, 1));


            typingSequnence
                .AppendCallback(() => yatchaWord.enabled = true)
                .Append(yatchaWord.transform.DOScale(12, 0))
                .Append(yatchaWord.transform.DOScale(1.7f, 1))
                .AppendInterval(1f)
                .AppendCallback(() => viewFishCaughtButtonEvent?.Invoke(lootobject, fishingLine.gameObject, catchLootCamera.GetComponent<Camera>(), yatchaWord))
                .AppendCallback(() => afterCatchingAFishEvent?.Invoke()); // 낚시하고나서 미끼 초기화


            //.AppendCallback(() => bigCatchWord_1.enabled = true)
            //.Append(bigCatchWord_1.transform.DOScale(12, 0))
            //.Append(bigCatchWord_1.transform.DOScale(4, 1))
            //.AppendCallback(() => bigCatchWord_2.enabled = true)
            //.Append(bigCatchWord_2.transform.DOScale(12, 0))
            //.Append(bigCatchWord_2.transform.DOScale(4, 1))
            //.AppendInterval(1f)
            //.AppendCallback(() => viewFishCaughtButtonEvent?.Invoke(lootobject, fishingLine.gameObject, catchLootCamera.GetComponent<Camera>(), bigCatchWord_1, bigCatchWord_2))
            //.AppendCallback(()=> afterCatchingAFishEvent?.Invoke());


            fishCompingUpSequence.Play().Append(typingSequnence);




        }
        // InputSystem 인터페이스 구현 메서드
        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {

            if (_catchLootCamera.enabled)
                return;

            if (EventSystem.current.IsPointerOverGameObject())//UI에서는 반응안하게 설정
                return;

            Vector2 currentPos = context.ReadValue<Vector2>();
            if (_fishingRod._fishingFloat != null && isCheckedMouseDraged && currentPos.y < _startPos.y) // 찌가 있고 마우스를 드래그 하는 중이라면
            {
                _attractInput = _lastPositionY - currentPos.y > THRESHOLD;
                _lastPositionY = currentPos.y;
            }
        }

        // 클릭 이벤트 처리 메서드
        public void OnClick(InputAction.CallbackContext context)
        {

            if (_catchLootCamera.enabled)
                return;

            if (EventSystem.current.IsPointerOverGameObject())//UI에서는 반응안하게 설정
                return;

            if (context.ReadValueAsButton())
            {
                _castInput = true; // 마우스 버튼이 눌렸을 때
                isCheckedMouseDraged = true; // 드래그 기능을 사용 중일 때
                _startPos = Pointer.current.position.ReadValue();
                Debug.Log("처음지점 좌표" + _startPos.ToString());
            }
            else
            {
                _endPos = Pointer.current.position.ReadValue();
                _dragDistance = _endPos.y - _startPos.y;
                _attractInput = false;
                isCheckedMouseDraged = false; // 마우스 버튼을 뗐을 때
                Debug.Log(_dragDistance);
                if (_dragDistance >= 0 && _fishingRod._fishingFloat == null && _advanced._caughtLoot == false) // 찌가 이미 있는 상태이고 미끼가 안문 상태이면 정방향으로 슬라이더를 하면 찌를 힘만큼 던진다.
                {
                    _castInput = false; // 마우스 버튼이 떼어졌을 때
                    _currentCastForce = showPowerbarEvent.Invoke();
                }
            }
        }
        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }
    }
}
