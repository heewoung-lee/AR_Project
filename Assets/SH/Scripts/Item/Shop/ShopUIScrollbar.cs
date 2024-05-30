using UnityEngine;
using UnityEngine.UI;

public class ShopUIScrollbar : MonoBehaviour
{
    Scrollbar _scrollbar;

    private void Awake()
    {
        _scrollbar = GetComponent<Scrollbar>();
    }

    private void OnEnable()
    {
        _scrollbar.value = 1;
    }


}
