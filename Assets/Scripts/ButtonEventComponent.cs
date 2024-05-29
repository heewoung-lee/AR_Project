using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 추후에 이벤트 연결 다하면 됨
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
