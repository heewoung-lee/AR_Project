using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginGotoMain : MonoBehaviour
{
    ButtonEventComponent _buttonEvent;

    private void Start()
    {
        _buttonEvent = gameObject.AddComponent<ButtonEventComponent>();
        _buttonEvent.ButtonAction(() => SceneManager.LoadScene("SeungHyunScene"));
    }
}
