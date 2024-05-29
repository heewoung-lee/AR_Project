using UnityEngine;
using UnityEngine.UI;

public class ShopCancelButton : MonoBehaviour
{
    Canvas ShopUI;
    Button cancelButton;

    private void Awake()
    {
        cancelButton = GetComponent<Button>();
        ShopUI = transform.root.GetComponent<Canvas>();
    }

    private void Start()
    {
        cancelButton.onClick.AddListener(() => ShopUI.enabled = false);
    }
}
