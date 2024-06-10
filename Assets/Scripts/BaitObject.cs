using FishingGameTool.Fishing;
using FishingGameTool.Fishing.BaitData;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BaitObject : MonoBehaviour
{
    private const int BAITNUMBER = 100;
    private const int BAIT_NUll_NUMBER = 3;
    GameObject _bait;
    GameObject _fishFloat;
    FishingBaitData _fishingBaitData;
    FishingSystem _fishingSystem;
    FishingLine _fishingLine;
    BaitCount _baitCount;
    public Material[] _materials;
    private Dictionary<string, int> _materialDictionary;
    public Dictionary<string, int> MaterialDictionary => _materialDictionary;
    void Start()
    {
        _fishingSystem = transform.GetComponentInParent<FishingSystem>();
        _fishingLine = GetComponent<FishingLine>();
        _materials = Resources.LoadAll<Material>("HW/BaitMetarials");
        _materialDictionary = new Dictionary<string, int>();
        for (int i = 0; i < _materials.Length; i++)
        {
            string materialName = _materials[i].name.Replace("Bait-", "");
            _materials[i].name = materialName;
            _materialDictionary[materialName] = i + BAITNUMBER;
        }

        foreach (var MaterialDictionary in MaterialDictionary)
        {
            Debug.Log($"{MaterialDictionary.Key}  {MaterialDictionary.Value}");
        }
        Debug.Log($"{MaterialDictionary.Keys}   {MaterialDictionary.Values}");
        _bait = Instantiate(Resources.Load("HW/Prefabs/BaitPrefab") as GameObject);
        _fishFloat = Instantiate(Resources.Load("HW/Prefabs/FishingFloat") as GameObject);
        _bait.GetComponent<MeshRenderer>().material = GetMaterialByName("Null");
        SubscribeToBaitButtonAction();
        _fishingSystem.afterCatchingAFishEvent += () => _bait.GetComponent<MeshRenderer>().material = GetMaterialByName("Null"); // 낚시하고나서 미끼 초기화

    }
    private void Update()
    {
        _fishFloat.GetComponentInChildren<StartPoint>().transform.position = _fishingLine.Segments[_fishingLine.Segments.Count - 1].position;
        _bait.transform.position = _fishFloat.GetComponentInChildren<EndPoint>().transform.position;
    }
    private void SubscribeToBaitButtonAction()
    {
        BaitButtonAction[] baitButtonActions = FindObjectsOfType<BaitButtonAction>();
        foreach (var baitButtonAction in baitButtonActions)
        {
            baitButtonAction.BaitChangeAction += ChangedBaitObject;
        }
    }
    public void DestroyObject()
    {
        _bait.SetActive(false);
        _fishFloat.SetActive(false);
    }

    public void On_Object()
    {
        _bait.SetActive(true);
        _fishFloat.SetActive(true);
    }

    public Material GetMaterialByName(string name)
    {
        if (_materialDictionary.TryGetValue(name, out int index))
        {
            return _materials[index - BAITNUMBER];
        }
        return null;
    }

    public void ChangedBaitObject(int baitType, BaitCount baitCount)
    {
        if (baitCount.BaitCounts == 0)
        {
            _fishingBaitData = Resources.Load<FishingBaitData>("Hw/BaitScriptableObject/Bait-Null");
            _bait.GetComponent<MeshRenderer>().material = _materials[BAIT_NUll_NUMBER];
        }
        else
        {
            switch (baitType)
            {
                case 0 + BAITNUMBER:
                    _fishingBaitData = Resources.Load<FishingBaitData>("Hw/BaitScriptableObject/Bait-Krill");
                    break;
                case 1 + BAITNUMBER:
                    _fishingBaitData = Resources.Load<FishingBaitData>("Hw/BaitScriptableObject/Bait-Crap");
                    break;
                case 2 + BAITNUMBER:
                    _fishingBaitData = Resources.Load<FishingBaitData>("Hw/BaitScriptableObject/Bait-Lumbricus");
                    break;
                default:
                    _fishingBaitData = Resources.Load<FishingBaitData>("Hw/BaitScriptableObject/Bait-Null");
                    break;
            }

            _bait.GetComponent<MeshRenderer>().material = _materials[baitType - BAITNUMBER];
        }


        _fishingSystem._bait = _fishingBaitData;
        _baitCount = baitCount;
        _fishingSystem.BaitCountDecreaseEvent = DecreaseBait;
        //미끼를 끼면 캐릭터에 있는 미끼 필드에 미끼가 끼워지게 수정
        //던지면 카운트가 --되게끔 수정.

    }

    public void DecreaseBait()
    {
        _baitCount.BaitCounts--;
        if (_baitCount.BaitCounts == 0)
        {
            _baitCount.GetComponentInParent<Image>().sprite = null;
        }
    }

}
