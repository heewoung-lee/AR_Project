using UnityEngine;
using UnityEngine.SceneManagement;

public class AquariumGotoMain : MonoBehaviour
{
    ButtonEventComponent _buttonEvent;

    private void Start()
    {
        _buttonEvent = gameObject.AddComponent<ButtonEventComponent>();
        _buttonEvent.ButtonAction(() => SceneManager.LoadScene("PlayScene"));
    }
}
