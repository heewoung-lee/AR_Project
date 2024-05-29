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
using FishingGameTool.CustomAttribute;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using System;

namespace FishingGameTool.Fishing
{
    public enum LootCatchType
    {
        SpawnItem,
        InvokeEvent
    };

    [AddComponentMenu("Fishing Game Tool/Fishing System")]
    public class FishingSystem : MonoBehaviour, InputSystem.IUIActions
    {
        [System.Serializable]
        public class LootCatchEvent
        {
            [InfoBox("This UnityEvent passes a prefab from LootData as an argument of type GameObject.")]
            [InfoBox("This feature is under development. In version 1.0, it does not work.")]
            public int _invokeCalls = 1;
            public UnityEvent _event;
        }

        [System.Serializable]
        public class AdvancedSettings
        {
            [ReadOnlyField]
            public FishingLootData _caughtLootData; 

            [InfoBox("Custom Catch Probability Data.")]
            public CatchProbabilityData _catchProbabilityData;

            public bool _caughtLoot = false;
            public float _returnSpeedWithoutLoot = 3f;
            [InfoBox("The time interval in seconds between consecutive checks to determine if the catch has been successfully caught.")]
            public float _catchCheckInterval = 1f;
            [ReadOnlyField]
            public float _lootWeight;
        }

        [BetterHeader("Fishing Settings", 20), Space]
        public FishingRod _fishingRod;

        [InfoBox("LayerMask used to determine on which layer fishing is allowed.")]
        public LayerMask _fishingLayer;
        public FishingBaitData _bait;

        [Space, BetterHeader("Loot Settings", 20), InfoBox("Select the method of handling loot catching:\n\n- \"SpawnItem\": Creates a game object " +
            "upon successfully catching the loot.\n- \"InvokeEvent\": Triggers a designated event upon successfully catching the loot.")]
        public LootCatchType _lootCatchType;

        [HideInInspector]
        public bool _showCatchEvent = false;

        [ShowVariable("_showCatchEvent")]
        public LootCatchEvent _lootCatchEvent;

        [Space, BetterHeader("Cast And Attract Settings", 20)]
        public GameObject _fishingFloatPrefab;
        public float _maxCastForce = 20f;
        public float _forceChargeRate = 4f;
        [ReadOnlyField]
        public float _currentCastForce;
        public float _spawnFloatDelay = 0.3f;

        [InfoBox("Minimum distance required for successfully picking up or catching a target (e.g., fish or object).")]
        public float _catchDistance = 3.5f;

        [Space, AddButton("Advanced Settings", "_showAdvancedSettings")]
        public bool _showAdvancedSettings = false;

        [ShowVariable("_showAdvancedSettings")]
        public AdvancedSettings _advanced;

        [HideInInspector]
        private bool _attractInput;
        public bool attractInput { get => _attractInput; }
        [HideInInspector]
        private bool _castInput = false;
        public bool castInput { get => _castInput; } 
        [HideInInspector]
        private bool _castFloat = false;
        public bool castFloat { get => _castFloat; }

        private bool ischeckedCast = false; 
        public event Action<bool> showPowerbarEvent;


        private Button _attractButton;
        private Vector2 _startPos;

        private InputSystem _inputSystem;
        private float _dragDistance = 0;
        public float dragDistance
        {
            get => _dragDistance;
        }
        #region PRIVATE VARIABLES

        private float _catchCheckIntervalTimer;
        private float _randomSpeedChangerTimer = 2f;
        private float _randomSpeedChanger = 1f;
        private float _finalSpeed;

        FishingFloatPathfinder _fishingFloatPathfinder = new FishingFloatPathfinder();

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


            _catchCheckIntervalTimer = _advanced._catchCheckInterval;
            //_castingButton = GameObject.Find("CharacterUI/Casting").GetComponent<Button>();
            _attractButton = GameObject.Find("CharacterUI/Attract").GetComponent<Button>();

            //AddEventTrigger(_castingButton.gameObject, EventTriggerType.PointerDown, (e) => { _castInput = true; });
            //AddEventTrigger(_castingButton.gameObject, EventTriggerType.PointerUp, (e) => { _castInput = false; });

            AddEventTrigger(_attractButton.gameObject, EventTriggerType.PointerDown, (e) => { _attractInput = true; });
            AddEventTrigger(_attractButton.gameObject, EventTriggerType.PointerUp, (e) => { _attractInput = false; });
        }
        private void AddEventTrigger(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        private void Update()
        {
            if (_fishingRod != null)
            {
                AttractFloat();
                CastFloat();
            }

        }

        #region AttractFloat

        private void AttractFloat()
        {
            if (_fishingRod._fishingFloat == null)
                return;

            SubstrateType substrateType = _fishingRod._fishingFloat.GetComponent<FishingFloat>().CheckSurface(_fishingLayer);

            if (substrateType == SubstrateType.Water)
                _advanced._caughtLoot = CheckingLoot(_advanced._caughtLoot, _bait, _advanced._catchProbabilityData, transform.position, _fishingRod._fishingFloat.position);

            LineLengthLimitation(_fishingRod._fishingFloat.gameObject, transform.position, _fishingRod._lineStatus, substrateType);

            if (_attractInput && substrateType == SubstrateType.InAir && !_advanced._caughtLoot)
            {
                Destroy(_fishingRod._fishingFloat.gameObject);
                _fishingRod._fishingFloat = null;

                return;
            }
            else if (_attractInput && substrateType == SubstrateType.Land && !_advanced._caughtLoot)
            {
                float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);
                float speed = _advanced._returnSpeedWithoutLoot * 120f * Time.deltaTime;

                Vector3 direction = (transform.position - _fishingRod._fishingFloat.position).normalized;

                Rigidbody fishingFloatRB = _fishingRod._fishingFloat.GetComponent<Rigidbody>();
                fishingFloatRB.velocity = direction * speed;

                if (distance <= _catchDistance)
                {
                    Destroy(_fishingRod._fishingFloat.gameObject);
                    _fishingRod._fishingFloat = null;

                    return;
                }
            }
            else if (_attractInput && substrateType == SubstrateType.Water && !_advanced._caughtLoot)
            {
                float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);

                _fishingFloatPathfinder.FloatBehavior(null, _fishingRod._fishingFloat, transform.position, _fishingRod._lineStatus._maxLineLength,
                   _advanced._returnSpeedWithoutLoot, _attractInput, _fishingLayer);

                if (distance <= _catchDistance)
                {
                    Destroy(_fishingRod._fishingFloat.gameObject);
                    _fishingRod._fishingFloat = null;

                    return;
                }
            }
            else if (_advanced._caughtLoot && _fishingRod._fishingFloat != null)
            {
                _fishingRod.LootCaught(_advanced._caughtLoot);

                while (_advanced._caughtLootData == null)
                {
                    List<FishingLootData> lootDataList = _fishingRod._fishingFloat.GetComponent<FishingFloat>().GetLootDataFormWaterObject();
                    _advanced._caughtLootData = ChooseFishingLoot(_bait, lootDataList);

                    if (_advanced._caughtLootData != null)
                    {
                        float lootWeight = UnityEngine.Random.Range(_advanced._caughtLootData._weightRange._minWeight, _advanced._caughtLootData._weightRange._maxWeight);
                        _advanced._lootWeight = lootWeight;

                        _bait = null;
                    }
                }

                AttractWithLoot(_advanced._caughtLootData, _fishingRod._fishingFloat, transform.position, _fishingLayer, _attractInput, _advanced._lootWeight, _fishingRod);

                if (_attractInput && _fishingRod._fishingFloat != null)
                {
                    float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);

                    if (distance <= _catchDistance)
                    {
                        GrabLoot(_advanced._caughtLootData, _fishingRod._fishingFloat.position, transform.position);
                        Destroy(_fishingRod._fishingFloat.gameObject);
                        _fishingRod._fishingFloat = null;
                        _advanced._caughtLoot = false;
                        _advanced._caughtLootData = null;
                        _fishingRod.FinishFishing();

                        _fishingFloatPathfinder.ClearPathData();

                        return;
                    }
                }
            }
        }

        #endregion

        #region LineLengthLimitation
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

        private void AttractWithLoot(FishingLootData lootData, Transform fishingFloatTransform, Vector3 transformPosition, LayerMask fishingLayer, bool attractInput,
            float lootWeight, FishingRod fishingRod)
        {
            float lootSpeed = CalculateLootSpeed(lootData, lootWeight);
            float attractSpeed = CalculateAttractSpeed(fishingRod, attractInput, lootWeight, (int)lootData._lootTier);
            _finalSpeed = Mathf.Lerp(_finalSpeed, attractInput == true ? CalculateFinalAttractSpeed(lootSpeed, attractSpeed, lootData) : lootSpeed, 3f * Time.deltaTime);

            _fishingFloatPathfinder.FloatBehavior(lootData, fishingFloatTransform, transformPosition, fishingRod._lineStatus._maxLineLength, _finalSpeed, attractInput, fishingLayer);

            //Debug.Log("Loot Speed: " + lootSpeed + " | Attract Speed: " + attractSpeed + " | Final Speed: " + _finalSpeed);
        }

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

        private float CalculateAttractSpeed(FishingRod fishingRod, bool attractInput, float lootWeight, int lootTier)
        {
            FishingLineStatus fishingLineStatus = fishingRod.CalculateLineLoad(_attractInput, lootWeight, lootTier);
            float attractionSpeed = fishingLineStatus._attractFloatSpeed;

            return attractionSpeed;
        }

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
        private void GrabLoot(FishingLootData lootData, Vector3 fishingFloatPosition, Vector3 transformPosition)
        {
            switch (_lootCatchType)
            {
                case LootCatchType.SpawnItem:

                    if (lootData._lootPrefab == null)
                    {
                        Debug.LogError("No loot prefab added!");
                        return;
                    }

                    SpawnLootItem(lootData._lootPrefab, fishingFloatPosition, transformPosition);

                    return;

                case LootCatchType.InvokeEvent:

                    Debug.LogWarning("This feature is under development. In version 1.0, it does not work.");

                    return;
            }
        }

        private void SpawnLootItem(GameObject lootPrefab, Vector3 fishingFloatPosition, Vector3 transformPosition)
        {
            Vector3 direction = ((transformPosition - fishingFloatPosition) + (Vector3.up * 3f)).normalized;
            Vector3 spawnPosition = fishingFloatPosition + new Vector3(0f, 1f, 0f);

            GameObject spawnedLootPrefab = Instantiate(lootPrefab, spawnPosition, Quaternion.identity);

            float distance = Vector3.Distance(fishingFloatPosition, transformPosition);
            float desiredTime = 0.8f;

            float force = distance / desiredTime;

            spawnedLootPrefab.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
        }

        #endregion

        #region CheckingLoot

        private bool CheckingLoot(bool caughtLoot, FishingBaitData baitData, CatchProbabilityData catchProbabilityData,
            Vector3 transformPosition, Vector3 fishingFloatPosition)
        {
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

        private bool CheckLootIsCatch(FishingBaitData baitData, CatchProbabilityData catchProbabilityData,
            Vector3 transformPosition, Vector3 fishingFloatPosition)
        {
            float distance = Vector3.Distance(transformPosition, fishingFloatPosition);
            float minSafeFishingDistanceFactor = 10f;

            int chanceVal = UnityEngine.Random.Range(1, 100);

            int commonProbability = 5;
            int uncommonProbability = 12;
            int rareProbability = 22;
            int epicProbability = 35;
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

        private float CalculateTotalRarity(List<FishingLootData> lootDataList)
        {
            float totalRarity = 0;

            foreach (var lootData in lootDataList)
            {
                totalRarity += lootData._lootRarity;
            }

            return totalRarity;
        }

        private List<float> CalculatePercentageRarity(List<FishingLootData> lootDataList, float totalRarity)
        {
            List<float> lootRarityList = new List<float>();

            foreach (var lootData in lootDataList)
            {
                float percentageRarity = (lootData._lootRarity / totalRarity) * 100f;
                lootRarityList.Add(percentageRarity);

                //Debug.Log(lootData + " - Rarity Percentage: " + percentageRarity + "%");
            }

            return lootRarityList;
        }

        #endregion

        #region CastFloat

        private void CastFloat()
        {
            // 낚싯대가 이미 던져졌거나, 낚싯대 던지기 동작이 이미 실행 중이거나, 낚싯줄이 끊어진 상태라면 동작하지 않음
            if (_fishingRod._fishingFloat != null || _castFloat || _fishingRod._lineStatus._isLineBroken)
                return;

            // 던지기 입력이 활성화된 상태라면 던지기 힘을 계산
            if (_castInput)
            {
                _currentCastForce = CalculateCastForce(_currentCastForce, _maxCastForce, _forceChargeRate);
            }
            else if (!_castInput && _currentCastForce != 0f) // 던지기 입력이 비활성화되고 던지기 힘이 0이 아니라면 던지기 동작을 시작
            {
                Vector3 spawnPoint = _fishingRod._line._lineAttachment.position; // 낚싯대의 시작 위치
                Vector3 castDirection = transform.forward + Vector3.up; // 던지는 방향

                // 던지기 지연을 시작
                StartCoroutine(CastingDelay(_spawnFloatDelay, castDirection, spawnPoint, _currentCastForce, _fishingFloatPrefab));

                _currentCastForce = 0f; // 던지기 힘 초기화
            }
        }


        private IEnumerator CastingDelay(float delay, Vector3 castDirection, Vector3 spawnPoint, float castForce, GameObject fishingFloatPrefab)
        {
            _castFloat = true; // 던지기 동작 중임을 표시

            yield return new WaitForSeconds(delay); // 지정된 지연 시간 동안 대기
            _fishingRod._fishingFloat = Cast(castDirection, spawnPoint, castForce, fishingFloatPrefab); // 던지기 실행
            _castFloat = false; // 던지기 동작 완료
        }

        private Transform Cast(Vector3 castDirection, Vector3 spawnPoint, float castForce, GameObject fishingFloatPrefab)
        {
            // 낚싯대 프리팹을 지정된 위치에 생성
            GameObject spawnedFishingFloat = Instantiate(fishingFloatPrefab, spawnPoint, Quaternion.identity);
            // 생성된 낚싯대에 힘을 가하여 던지기 동작 수행
            spawnedFishingFloat.GetComponent<Rigidbody>().AddForce(castDirection * castForce, ForceMode.Impulse);

            // 생성된 낚싯대의 Transform을 반환
            return spawnedFishingFloat.transform;
        }

        private float CalculateCastForce(float currentCastForce, float maxCastForce, float forceChargeRate)
        {
            // 현재 던지기 힘을 힘 충전 속도에 따라 증가
            currentCastForce += forceChargeRate * Time.deltaTime;
            // 던지기 힘이 최대 힘을 초과하지 않도록 제한
            currentCastForce = currentCastForce > maxCastForce ? maxCastForce : currentCastForce;

            return currentCastForce; // 계산된 던지기 힘 반환
        }


        #endregion

        public void ForceStopFishing()
        {
            Destroy(_fishingRod._fishingFloat.gameObject);
            _fishingRod._fishingFloat = null;
            _advanced._caughtLoot = false;
            _advanced._caughtLootData = null;
            _fishingRod.FinishFishing();
            _fishingFloatPathfinder.ClearPathData();
        }

        public void AddBait(FishingBaitData baitData)
        {
            if (_bait == null)
                _bait = baitData;
            else
            {
                Vector3 spawnPos = transform.position + transform.forward + Vector3.up;
                Instantiate(_bait._baitPrefab, spawnPos, Quaternion.identity);

                _bait = baitData;
            }
        }

        public void AddCustomCatchProbabilityData(CatchProbabilityData catchProbabilityData)
        {
            _advanced._catchProbabilityData = catchProbabilityData;
        }

        public void FixFishingLine()
        {
            if (_fishingRod._lineStatus._isLineBroken)
                _fishingRod._lineStatus._isLineBroken = false;
        }

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

        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (IsPointerOverButton(_attractButton))
                return; // _attractButton 위에 있을 때는 OnClick 동작을 무시


            if (context.ReadValueAsButton())
            {
                _castInput = true; // 마우스 버튼이 눌렸을 때
                _startPos = Pointer.current.position.ReadValue();
                Debug.Log("처음지점 좌표" + _startPos.ToString());
            }
            else
            {
                _castInput = false; // 마우스 버튼이 떼어졌을 때
                ischeckedCast = true; // 힘의 바 체크를 위한 변수
                showPowerbarEvent?.Invoke(ischeckedCast);
                Vector2 endPos = Pointer.current.position.ReadValue();
                _dragDistance = Mathf.Abs(endPos.y - _startPos.y);
                Debug.Log("끝지점 좌표" + endPos.ToString());
                Debug.Log("거리차이" + dragDistance);
            }
        }

        private bool IsPointerOverButton(Button button)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Pointer.current.position.ReadValue(), Camera.main);
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