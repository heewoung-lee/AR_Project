using UnityEngine;
using UnityEngine.UI;

public class ShopBaitButton : MonoBehaviour
{
    Button _shopBaitButton; // ¹Ì³¢ ¹öÆ°
    [SerializeField] GameObject _fishInventory;
    [SerializeField] GameObject _baitInventory;
    [SerializeField] GameObject _rodInventory;

    bool _shopBait = true;
    bool _shopOthers = false;
    private void Start()
    {
        _shopBaitButton = GetComponent<Button>();
        _shopBaitButton.onClick.AddListener(() => { _baitInventory.SetActive(_shopBait); _fishInventory.SetActive(_shopOthers); _rodInventory.SetActive(_shopOthers); });
    }
}
