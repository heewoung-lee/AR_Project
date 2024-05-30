using UnityEngine;
using UnityEngine.UI;

public class ShopRodButton : MonoBehaviour
{
    Button _shopRodButton; // 물고기 버튼
    [SerializeField] GameObject _fishInventory;
    [SerializeField] GameObject _baitInventory;
    [SerializeField] GameObject _rodInventory;

    bool _shopRod = true;
    bool _shopOthers = false;
    private void Start()
    {
        _shopRodButton = GetComponent<Button>();
        _shopRodButton.onClick.AddListener(() => { _rodInventory.SetActive(_shopRod); _baitInventory.SetActive(_shopOthers); _fishInventory.SetActive(_shopOthers); });
    }
}
