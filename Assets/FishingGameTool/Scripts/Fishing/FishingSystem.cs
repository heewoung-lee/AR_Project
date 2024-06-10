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
    // ����⸦ ��� ����� �����ϴ� ������: SpawnItem �Ǵ� InvokeEvent
    public enum LootCatchType
    {
        SpawnItem,
        InvokeEvent
    };

    // FishingSystem ��ũ��Ʈ�� ������Ʈ �޴� �߰�
    [AddComponentMenu("Fishing Game Tool/Fishing System")]
    public class FishingSystem : MonoBehaviour, InputSystem.IUIActions
    {
        // ����� ��� �̺�Ʈ�� ó���ϱ� ���� ����ȭ�� Ŭ����
        [System.Serializable]
        public class LootCatchEvent
        {
            public int _invokeCalls = 1; // �̺�Ʈ ȣ�� Ƚ��
            public UnityEvent _event; // Unity �̺�Ʈ ��ü
        }

        // ��� ������ ���� ����ȭ�� Ŭ����
        [System.Serializable]
        public class AdvancedSettings
        {
            public FishingLootData _caughtLootData; // ���� ����� ������ ����
            public CatchProbabilityData _catchProbabilityData; // ��� Ȯ�� ������
            public bool _caughtLoot = false; // ����⸦ ��Ҵ��� ����
            public float _returnSpeedWithoutLoot = 3f; // ����� ���� ���ƿ��� �ӵ�
            public float _catchCheckInterval = 1f; // ��� Ȯ�� ����
            public float _lootWeight; // ������� ����
        }

        private const int THRESHOLD = 5; //�巡�� �ΰ���
        public FishingRod _fishingRod; // ���˴� ��ü
        public LayerMask _fishingLayer; // ���� ���̾� ����ũ
        public FishingBaitData _bait; // �̳� ������
        public LootCatchType _lootCatchType; // ����� ��� Ÿ��
        public Ease FishingCompleteActionType;
        [HideInInspector]
        public bool _showCatchEvent = false; // ����� ��� �̺�Ʈ ǥ�� ����

        public LootCatchEvent _lootCatchEvent; // ����� ��� �̺�Ʈ
        public GameObject _fishingFloatPrefab; // ���˴� ������
        public float _maxCastForce = 20f; // �ִ� ������ ��
        public float _forceChargeRate = 6f; // �� ���� �ӵ�
        public float _currentCastForce; // ���� ������ ��
        public float _spawnFloatDelay = 0.3f; // ���˴� ���� ���� �ð�
        public float _catchDistance = 3.5f; // ��� �ּ� �Ÿ�
        public float powerChaging = 2f;

        public bool _showAdvancedSettings = false; // ��� ���� ǥ�� ����

        public AdvancedSettings _advanced; // ��� ���� ��ü

        [HideInInspector]
        private bool _attractInput; // �̳� ���� �Է� ����
        public bool attractInput { get => _attractInput; } // �̳� ���� �Է� ������
        [HideInInspector]
        private bool _castInput = false; // ������ �Է� ����
        public bool castInput { get => _castInput; } // ������ �Է� ������
        [HideInInspector]
        private bool _castFloat = false; // ������ ���� ����
        public bool castFloat { get => _castFloat; } // ������ ���� ������

        public event Func<float> showPowerbarEvent; // �Ŀ��� �̺�Ʈ
        //public event Action<Transform> setLootCamera;
        public event Func<GameObject> setLineEndPoint;
        public event System.Action<GameObject, GameObject, Camera, Image> viewFishCaughtButtonEvent;

        private Vector2 _startPos; // ���� ��ġ
        private Vector2 _endPos;

        private InputSystem _inputSystem; // �Է� �ý���
        private float _dragDistance = 0; // �巡�� �Ÿ�
        public float dragDistance { get => _dragDistance; } // �巡�� �Ÿ� ������
        private bool isCheckedMouseDraged = false;
        private float _lastPositionY = 0f; //�巡�׿����� �۵��ǰԲ� ���콺 �������� ��ġ�� �˱����� �ʵ�
        private GameObject _fishingrope;
        private Transform _bigCatchWord;
        private FishLoadEndPosition _fishLoadEndPosition;

        public AnimationCurve _fishingCatchActionEase;
        public event Action castingMontion;
        public event Action afterCatchingAFishEvent;
        public Action BaitCountDecreaseEvent; //���ô� ������ ���� �̳��� ���� ���� �̺�Ʈ
        public Action BaitInventoryDeleteEvent;
        #region PRIVATE VARIABLES

        private float _catchCheckIntervalTimer; // ��� Ȯ�� ���� Ÿ�̸�
        private float _randomSpeedChangerTimer = 2f; // ���� �ӵ� ���� Ÿ�̸�
        private float _randomSpeedChanger = 1f; // ���� �ӵ� ���� ��
        private float _finalSpeed; // ���� �ӵ�
        [SerializeField] private Camera _arMainCamera;
        [SerializeField] private Camera _catchLootCamera;
        [SerializeField] private Canvas _CharactorUI;



        private bool _isCheckedLoadScene = false;


        public bool isCheckedLoadScene { get => _isCheckedLoadScene; }


        [SerializeField] private Toggle _testToggle; //���ô븦 �����ڸ��� �������� ��������� �׽�Ʈ�� ���
        [SerializeField] private Toggle _testToggleCatch; //���� ������ ������ ����Ⱑ �ٷ������� �׽�Ʈ�� ���

        FishingFloatPathfinder _fishingFloatPathfinder = new FishingFloatPathfinder(); // ���� �� ��� ã�� ��ü

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

        // Update �޼���: �� �����Ӹ��� ȣ��Ǹ� �� ���� �� ������ ���� ����
        private void Update()
        {
            if (_fishingRod != null)
            {
                AttractFloat();
                CastFloat();
            }
        }

        #region AttractFloat

        // �� ���� �޼���
        private void AttractFloat()
        {
            // ���ô��� ���� � null���� Ȯ���ϰ� null�̸� �Լ� ����
            if (_fishingRod._fishingFloat == null)
                return;

            // ���� ���� ǥ�� ���¸� üũ�Ͽ� substrateType�� ����
            SubstrateType substrateType = _fishingRod._fishingFloat.GetComponent<FishingFloat>().CheckSurface(_fishingLayer);
            // ���� ǥ�� ���°� ���̸� ���� ���� ��ġ�� ��ȯ ��ġ�� �̿��Ͽ� loot�� üũ
            if (substrateType == SubstrateType.Water)
                _advanced._caughtLoot = CheckingLoot(_advanced._caughtLoot, _bait, _advanced._catchProbabilityData, transform.position, _fishingRod._fishingFloat.position);

            // ���� ���� ���̸� �����ϴ� �Լ� ȣ��
            LineLengthLimitation(_fishingRod._fishingFloat.gameObject, transform.position, _fishingRod._lineStatus, substrateType);

            // ����ڰ� ���� �Է��� �Ͽ���, ǥ�� ���°� �����̸� loot�� ���� ���� ���
            if (_attractInput && substrateType == SubstrateType.InAir && !_advanced._caughtLoot)
            {
                // ���� � �ı��ϰ� ���� �Է��� false�� ����, ���� � null�� ����
                Destroy(_fishingRod._fishingFloat.gameObject);
                _attractInput = false;
                _fishingRod._fishingFloat = null;
                return;
            }
            //// ����ڰ� ���� �Է��� �Ͽ���, ǥ�� ���°� ���̸� loot�� ���� ���� ���
            //else if (_attractInput && substrateType == SubstrateType.Land && !_advanced._caughtLoot)
            //{
            //    // ��ȯ ��ġ�� ���� ���� ��ġ ������ �Ÿ��� ���
            //    float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);
            //    // �ӵ��� ��� (loot�� ���� ���� ��ȯ �ӵ��� 120�� ���ϰ� deltaTime�� ����)
            //    float speed = _advanced._returnSpeedWithoutLoot * 120f * Time.deltaTime;

            //    // ������ ���
            //    Vector3 direction = (transform.position - _fishingRod._fishingFloat.position).normalized;

            //    // ���� ���� Rigidbody ������Ʈ�� �����ͼ� �ӵ� ����
            //    Rigidbody fishingFloatRB = _fishingRod._fishingFloat.GetComponent<Rigidbody>();
            //    fishingFloatRB.velocity = direction * speed;

            //    // �Ÿ��� ������ �Ÿ� �����̸� ���� � �ı��ϰ� null�� ����
            //    if (distance <= _catchDistance)
            //    {
            //        Destroy(_fishingRod._fishingFloat.gameObject);
            //        _fishingRod._fishingFloat = null;
            //        return;
            //    }
            //}
            // ����ڰ� ���� �Է��� �Ͽ���, ǥ�� ���°� ���̸� loot�� ���� ���� ���
            else if (_attractInput && substrateType == SubstrateType.Water && !_advanced._caughtLoot)
            {
                // ��ȯ ��ġ�� ���� ���� ��ġ ������ �Ÿ��� ���
                float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);

                // ���� ���� ���� ������ ����
                _fishingFloatPathfinder.FloatBehavior(null, _fishingRod._fishingFloat, transform.position, _fishingRod._lineStatus._maxLineLength,
                   _advanced._returnSpeedWithoutLoot, _attractInput, _fishingLayer);

                // �Ÿ��� ������ �Ÿ� �����̸� ���� � �ı��ϰ� null�� ����
                if (distance <= _catchDistance)
                {
                    Destroy(_fishingRod._fishingFloat.gameObject);
                    _fishingRod._fishingFloat = null;
                    _fishLoadEndPosition.CreateFishingRope();
                    return;
                }
            }
            // loot�� ��Ұ�, ���� � null�� �ƴ� ���
            else if (_advanced._caughtLoot && _fishingRod._fishingFloat != null)
            {
                // ���ô��� LootCaught �޼��� ȣ��
                _fishingRod.LootCaught(_advanced._caughtLoot);

                // loot �����Ͱ� null�� ���� ���� ����
                while (_advanced._caughtLootData == null)
                {
                    // ��ü�κ��� loot �����͸� �����ͼ� ����Ʈ�� ����
                    List<FishingLootData> lootDataList = _fishingRod._fishingFloat.GetComponent<FishingFloat>().GetLootDataFormWaterObject();
                    // bait�� loot ������ ����Ʈ�� �̿��� loot ����
                    _advanced._caughtLootData = ChooseFishingLoot(_bait, lootDataList);

                    if (_advanced._caughtLootData != null)
                    {
                        // loot�� ���Ը� ���� ���� ������ ����
                        float lootWeight = UnityEngine.Random.Range(_advanced._caughtLootData._weightRange._minWeight, _advanced._caughtLootData._weightRange._maxWeight);
                        _advanced._lootWeight = lootWeight;

                        // bait�� null�� ����
                        _bait = null;

                        BaitCountDecreaseEvent?.Invoke();
                        BaitInventoryDeleteEvent?.Invoke();
                        //TODO : BaitObject�� �־��� �̳��� ī���͸� --�Ұ�; --�ؼ� 0�� �Ǹ� ���ڸ��� Null�� ����
                    }
                }

                // loot�� �ִ� ���·� ���� ���� ����
                AttractWithLoot(_advanced._caughtLootData, _fishingRod._fishingFloat, transform.position, _fishingLayer, _attractInput, _advanced._lootWeight, _fishingRod);

                if (_attractInput && _fishingRod._fishingFloat != null)
                {
                    float distance = Vector3.Distance(transform.position, _fishingRod._fishingFloat.position);
                    if (_testToggleCatch.isOn)
                    {
                        distance = _catchDistance;
                    }
                    // ��ȯ ��ġ�� ���� ���� ��ġ ������ �Ÿ��� ���


                    // �Ÿ��� ������ �Ÿ� �����̸� loot�� ��� ���� � �ı�
                    if (distance <= _catchDistance)
                    {
                        GrabLoot(_advanced._caughtLootData, _fishingRod._fishingFloat.position, transform.position, out GameObject lootprefab);
                        CatchFishingScene(lootprefab, _catchLootCamera);//Todo: ī�޶� ���� ����
                        Destroy(_fishingRod._fishingFloat.gameObject);
                        _fishingRod._fishingFloat = null;
                        _advanced._caughtLoot = false;
                        _advanced._caughtLootData = null;
                        _fishingRod.FinishFishing();
                        // ��� ������ �ʱ�ȭ
                        _fishingFloatPathfinder.ClearPathData();
                        return;
                    }
                }
            }
        }


        #endregion

        #region LineLengthLimitation
        // ������ ���� ���� �޼���
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

        // ����⸦ ��� ���� ���� �� ���� �޼���
        private void AttractWithLoot(FishingLootData lootData, Transform fishingFloatTransform, Vector3 transformPosition, LayerMask fishingLayer, bool attractInput,
            float lootWeight, FishingRod fishingRod)
        {
            float lootSpeed = CalculateLootSpeed(lootData, lootWeight);
            float attractSpeed = CalculateAttractSpeed(fishingRod, attractInput, lootWeight, (int)lootData._lootTier) * powerChaging;
            _finalSpeed = Mathf.Lerp(_finalSpeed, attractInput == true ? CalculateFinalAttractSpeed(lootSpeed, attractSpeed, lootData) : lootSpeed, 3f * Time.deltaTime);

            _fishingFloatPathfinder.FloatBehavior(lootData, fishingFloatTransform, transformPosition, fishingRod._lineStatus._maxLineLength, _finalSpeed, attractInput, fishingLayer);
        }

        // ����� �ӵ� ���
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

        // ���� �ӵ� ���
        private float CalculateAttractSpeed(FishingRod fishingRod, bool attractInput, float lootWeight, int lootTier)
        {
            FishingLineStatus fishingLineStatus = fishingRod.CalculateLineLoad(_attractInput, lootWeight, lootTier);
            float attractionSpeed = fishingLineStatus._attractFloatSpeed;

            return attractionSpeed;
        }

        // ���� ���� �ӵ� ���
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
        // ����⸦ ��� �޼���
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

                    Debug.LogWarning("�� ����� ���� ���Դϴ�. ���� 1.0������ �۵����� �ʽ��ϴ�.");
                    lootprefab = null;
                    return;

                default:
                    lootprefab = null;
                    break;
            }
        }

        // ����� ������ ���� �޼���
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

        // ����⸦ ��Ҵ��� Ȯ���ϴ� �޼���
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

        // ����⸦ ��Ҵ��� Ȯ���ϴ� ���� �޼���
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

        // ������ �Ÿ��� Ȯ�� ���� ����ϴ� �޼���
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

        // ���� ����⸦ �����ϴ� �޼���
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

        // �� ��͵��� ����ϴ� �޼���
        private float CalculateTotalRarity(List<FishingLootData> lootDataList)
        {
            float totalRarity = 0;

            foreach (var lootData in lootDataList)
            {
                totalRarity += lootData._lootRarity;
            }

            return totalRarity;
        }

        // ��͵��� ������� ����ϴ� �޼���
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

        // �� ������ �޼���
        private void CastFloat()
        {
            // ���˴밡 �̹� �������ų�, ���˴� ������ ������ �̹� ���� ���̰ų�, �������� ������ ���¶�� �������� ����
            if (_fishingRod._fishingFloat != null || _castFloat)
                return;

            // ������ �Է��� Ȱ��ȭ�� ���¶�� ������ ���� ���
            if (_castInput)
            {
                _currentCastForce = CalculateCastForce(_currentCastForce, _maxCastForce, _forceChargeRate);
                //Debug.Log("�Ŀ�: " + _castingPower);
            }
            else if (!_castInput && _currentCastForce != 0f) // ������ �Է��� ��Ȱ��ȭ�ǰ� ������ ���� 0�� �ƴ϶�� ������ ������ ����
            {
                //Debug.Log("�Ŀ��� �����ִٸ� ����� ����");
                Vector3 spawnPoint = _fishingRod._line._lineAttachment.position; // ���˴��� ���� ��ġ
                Vector3 castDirection = transform.forward + Vector3.up; // ������ ����
                // ������ ������ ����
                StartCoroutine(CastingDelay(_spawnFloatDelay, castDirection, spawnPoint, _currentCastForce, _fishingFloatPrefab));
                StartCoroutine(waitDestroyRope());
                _currentCastForce = 0f; // ������ �� �ʱ�ȭ
            }
        }


        IEnumerator waitDestroyRope()
        {
            yield return new WaitForSeconds(0.5f);
            castingMontion?.Invoke();
        }
        // ������ ���� �޼���
        private IEnumerator CastingDelay(float delay, Vector3 castDirection, Vector3 spawnPoint, float castForce, GameObject fishingFloatPrefab)
        {
            _castFloat = true; // ������ ���� ������ ǥ��

            yield return new WaitForSeconds(delay); // ������ ���� �ð� ���� ���
            _fishingRod._fishingFloat = Cast(castDirection, spawnPoint, castForce, fishingFloatPrefab); // ������ ����
            _castFloat = false; // ������ ���� �Ϸ�
        }

        // �� ������ �޼���
        private Transform Cast(Vector3 castDirection, Vector3 spawnPoint, float castForce, GameObject fishingFloatPrefab)
        {
            // ���˴� �������� ������ ��ġ�� ����
            GameObject spawnedFishingFloat = Instantiate(fishingFloatPrefab, spawnPoint, Quaternion.identity);
            // ������ ���˴뿡 ���� ���Ͽ� ������ ���� ����
            spawnedFishingFloat.GetComponent<Rigidbody>().AddForce(castDirection * castForce, ForceMode.Impulse);

            // ������ ���˴��� Transform�� ��ȯ
            return spawnedFishingFloat.transform;
        }

        // ������ �� ��� �޼���
        private float CalculateCastForce(float currentCastForce, float maxCastForce, float forceChargeRate)
        {
            // ���� ������ ���� �� ���� �ӵ��� ���� ����
            currentCastForce += forceChargeRate * Time.deltaTime;
            // ������ ���� �ִ� ���� �ʰ����� �ʵ��� ����
            currentCastForce = currentCastForce > maxCastForce ? maxCastForce : currentCastForce;

            return currentCastForce; // ���� ������ �� ��ȯ
        }

        #endregion

        // ���� ���� ���� �޼���
        public void ForceStopFishing()
        {
            Destroy(_fishingRod._fishingFloat.gameObject);
            _fishingRod._fishingFloat = null;
            _advanced._caughtLoot = false;
            _advanced._caughtLootData = null;
            _fishingRod.FinishFishing();
            _fishingFloatPathfinder.ClearPathData();
        }

        // ����� ���� ��� Ȯ�� ������ �߰� �޼���
        public void AddCustomCatchProbabilityData(CatchProbabilityData catchProbabilityData)
        {
            _advanced._catchProbabilityData = catchProbabilityData;
        }

        // ������ ���� �޼���
        public void FixFishingLine()
        {
            if (_fishingRod._lineStatus._isLineBroken)
                _fishingRod._lineStatus._isLineBroken = false;
        }

        public void CatchFishingScene(GameObject lootObject, Camera catchLootCamera)
        {
            //ī�޶� �ø� ���̾ ���� ARLayer�� ������ �ִ� �ֵ��̸� ������ ���ϵ��� ����
            //2.CatchlootCamera�� Ȱ��ȭ �Ǹ鼭 lootObject�� ����ٴϵ��� ����
            catchLootCamera.enabled = true;
            catchLootCamera.GetComponent<LookatFish>().enabled = true;
            CatchingAnimation(lootObject, catchLootCamera.transform);

            //Transform LinePoint = GameObject.Find("LinePoint").GetComponent<Transform>();
            //LinePoint.transform.position = new Vector3(lootObject.transform.position.x, lootObject.transform.position.y+4, lootObject.transform.position.z);
            //lootObject.transform.SetParent(LinePoint); //��Ʈ ī�޶� �ؿ� LinePoint �ڽ����� ����
            //lootObject.name = "CaughtFish";
            //LinePoint.AddComponent<FishingAnimation>(); //���� �ִϸ��̼� ����
            //viewFishCaughtButtonEvent?.Invoke(); //Ok��ư ��������


            //LinePoint.transform.localPosition = Vector3.zero;
            //Transform FishMousePosition = lootObject.transform.Find("MousePosition").GetComponent<Transform>();
            ////FishMousePosition.SetParent(lootObject.transform);
            ////FishMousePosition.transform.localPosition = Vector3.zero;
            ////FishMousePosition.transform.localRotation = Quaternion.identity;
            ////lootObject.transform.Rotate(new Vector3(-90f, 0, -90f)); //����Ⱑ ������ ���Բ� ����
        }

        public void CatchingAnimation(GameObject lootobject, Transform catchLootCamera)
        {
            //������ ���� ������ �����ؾ� ��.
            GameObject FishingLope = Instantiate(_fishingrope);
            FishingLine fishingLine = FishingLope.GetComponent<FishingLine>();


            fishingLine.startTransform = FishingLope.transform.Find("StartPoint");
            fishingLine.startTransform.transform.position = catchLootCamera.transform.position + Vector3.up * 2;
            fishingLine.startTransform.transform.position += Vector3.right;//������ ��ġ ����
            //fishingLine.endTransform = lootobject.transform.Find("MousePosition");//�Կ� �������� ����
            fishingLine.endTransform = lootobject.GetComponentInChildren<FindMousePosition>().transform;


            //ToDO: ���ӿ� ���� �ִٰ� �������� ��������� ����-> ���� ��Ʈ�� ����
            lootobject.transform.position -= lootobject.transform.forward * 9f;
            // DOTween �������� ����Ͽ� "��ô" �ִϸ��̼� ����
            Image yatchaWord = _bigCatchWord.Find("Yatchaword").GetComponent<Image>();
            // ������ ����
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
                .AppendCallback(() => afterCatchingAFishEvent?.Invoke()); // �����ϰ��� �̳� �ʱ�ȭ


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
        // InputSystem �������̽� ���� �޼���
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

            if (EventSystem.current.IsPointerOverGameObject())//UI������ �������ϰ� ����
                return;

            Vector2 currentPos = context.ReadValue<Vector2>();
            if (_fishingRod._fishingFloat != null && isCheckedMouseDraged && currentPos.y < _startPos.y) // � �ְ� ���콺�� �巡�� �ϴ� ���̶��
            {
                _attractInput = _lastPositionY - currentPos.y > THRESHOLD;
                _lastPositionY = currentPos.y;
            }
        }

        // Ŭ�� �̺�Ʈ ó�� �޼���
        public void OnClick(InputAction.CallbackContext context)
        {

            if (_catchLootCamera.enabled)
                return;

            if (EventSystem.current.IsPointerOverGameObject())//UI������ �������ϰ� ����
                return;

            if (context.ReadValueAsButton())
            {
                _castInput = true; // ���콺 ��ư�� ������ ��
                isCheckedMouseDraged = true; // �巡�� ����� ��� ���� ��
                _startPos = Pointer.current.position.ReadValue();
                Debug.Log("ó������ ��ǥ" + _startPos.ToString());
            }
            else
            {
                _endPos = Pointer.current.position.ReadValue();
                _dragDistance = _endPos.y - _startPos.y;
                _attractInput = false;
                isCheckedMouseDraged = false; // ���콺 ��ư�� ���� ��
                Debug.Log(_dragDistance);
                if (_dragDistance >= 0 && _fishingRod._fishingFloat == null && _advanced._caughtLoot == false) // � �̹� �ִ� �����̰� �̳��� �ȹ� �����̸� ���������� �����̴��� �ϸ� � ����ŭ ������.
                {
                    _castInput = false; // ���콺 ��ư�� �������� ��
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
