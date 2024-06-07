using UnityEngine;
using UnityEngine.UI;

public class ShopOpenCloseButton : MonoBehaviour
{
    Canvas ShopUI;
    Button cancelButton;


    public static bool _shopButton = false;
    private void Awake()
    {
        cancelButton = GetComponent<Button>();
        ShopUI = GameObject.Find("UI_Shop").GetComponent<Canvas>();
    }

    private void Start()
    {
        cancelButton.onClick.AddListener(() => { _shopButton = !_shopButton; ShopUI.enabled = _shopButton; });
    }
}
