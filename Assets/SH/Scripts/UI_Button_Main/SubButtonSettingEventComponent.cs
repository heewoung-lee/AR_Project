using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubButtonSettingEventComponent : MonoBehaviour
{
    TMP_Text _buttonText;
    Image _buttonImage;

    // �ش� ��ư �ϴܿ� �̹����� ���� ���� �������� 
    // �ش� ���p �ϴܿ� �ؽ�Ÿ�� ������ �� �������� �ִµ�..

    private void Start()
    {
        _buttonImage = GetComponentInChildren<Image>();
        _buttonText = GetComponentInChildren<TMP_Text>();
    }

    private void SetColorChange()
    {

    }
}
