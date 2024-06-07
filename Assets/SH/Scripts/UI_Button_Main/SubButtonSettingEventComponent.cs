using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubButtonSettingEventComponent : MonoBehaviour
{
    TMP_Text _buttonText;
    Image _buttonImage;

    // 해당 버튼 하단에 이미지가 있을 수도 없을수도 
    // 해당 버늩 하단에 텍스타가 있을수 도 없을수도 있는데..

    private void Start()
    {
        _buttonImage = GetComponentInChildren<Image>();
        _buttonText = GetComponentInChildren<TMP_Text>();
    }

    private void SetColorChange()
    {

    }
}
