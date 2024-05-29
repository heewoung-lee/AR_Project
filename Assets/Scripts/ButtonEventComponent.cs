using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ���Ŀ� �̺�Ʈ ���� ���ϸ� ��
/// </summary>
public class ButtonEventComponent : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetEvent(UnityAction callBack)
    {
        button.onClick.AddListener(callBack);
    }
   
}
