using FishingGameTool.Fishing.BaitData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaitObject : MonoBehaviour
{
    private const int BAITNUMBER = 100;
    GameObject _bait;
    GameObject _fishFloat;
    FishingBaitData _fishingBaitData;
    FishingLine _fishingLine;
    public Material[] _materials;
    private Dictionary<string, int> _materialDictionary;
    public Dictionary<string, int> MaterialDictionary => _materialDictionary;
    IButtonAction baitButton = new BaitButtonAction();
    void Start()
    {
        _fishingLine = GetComponent<FishingLine>();
        _materials = Resources.LoadAll<Material>("HW/BaitMetarials");

        _materialDictionary = new Dictionary<string, int>();
        for (int i = 0; i < _materials.Length; i++)
        {
            string materialName = _materials[i].name.Replace("Bait-", "");
            _materials[i].name = materialName;
            _materialDictionary[materialName] =i+BAITNUMBER;

        }

        foreach(var MaterialDictionary in MaterialDictionary)
        {
            Debug.Log($"{MaterialDictionary.Key}  {MaterialDictionary.Value}");
        }
        Debug.Log($"{MaterialDictionary.Keys}   {MaterialDictionary.Values}");
        _bait = Instantiate(Resources.Load("HW/Prefabs/BaitPrefab") as GameObject);
        _fishFloat = Instantiate(Resources.Load("HW/Prefabs/FishingFloat") as GameObject);
        _bait.GetComponent<MeshRenderer>().material = GetMaterialByName("Null");
        baitButton.BaitChangeAction += ChangedBaitObject;
    }

    private void Update()
    {
        _fishFloat.GetComponentInChildren<StartPoint>().transform.position = _fishingLine.Segments[_fishingLine.Segments.Count - 1].position;
        _bait.transform.position = _fishFloat.GetComponentInChildren<EndPoint>().transform.position;
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
            return _materials[index- BAITNUMBER];
        }
        return null;
    }

    public void ChangedBaitObject(int biteNumber,int count,BaitButtonAction buttonAction)
    {
        baitButton = buttonAction;

        Debug.Log("버튼눌림");
        //미끼 버튼을 누르면 여기서 미끼 머테리얼 바뀌도록 수정하는 함수 작성
        //미끼 버튼을 누르면 _fishingBaitData도 바뀌어야함
        //미끼를 던지는 순간, 마지막에 낀 미끼가 -되도록 수정 Null은 제외

    }

}
