using UnityEngine;
using UnityEngine.UI;

public class ShopFishButton : MonoBehaviour
{
    Button _shopFishButton; // ����� ��ư
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
