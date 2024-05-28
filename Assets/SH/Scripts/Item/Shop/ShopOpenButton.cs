using UnityEngine;
using UnityEngine.UI;

public class ShopOpenButton : MonoBehaviour
{
    Canvas ShopUI;
    Button cancelButton;

    private void Awake()
    {
        cancelButton = GetComponent<Button>();
        ShopUI = GameObject.Find("UI_Shop").GetComponent<Canvas>();
    }

    private void Start()
    {
        cancelButton.onClick.AddListener(() => ShopUI.enabled = true);
    }
}
