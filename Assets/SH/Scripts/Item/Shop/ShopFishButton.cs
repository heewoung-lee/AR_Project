using UnityEngine;
using UnityEngine.UI;

public class ShopFishButton : MonoBehaviour
{
    Button _shopFishButton; // 물고기 버튼
    [SerializeField] GameObject _fishInventory;
    [SerializeField] GameObject _baitInventory;
    [SerializeField] GameObject _rodInventory;

    bool _shopFish = true;
    bool _shopOthers = false;
    private void Start()
    {
        _shopFishButton = GetComponent<Button>();
        _shopFishButton.onClick.AddListener(() => { _fishInventory.SetActive(_shopFish); _baitInventory.SetActive(_shopOthers); _rodInventory.SetActive(_shopOthers); });
    }
}
