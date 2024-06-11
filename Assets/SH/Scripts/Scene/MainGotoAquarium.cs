using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGotoAquarium : MonoBehaviour
{
    ButtonEventComponent _buttonevent;
    public event Action OnAquariumClick;

    private void Start()
    {
        _buttonevent = gameObject.AddComponent<ButtonEventComponent>();
        _buttonevent.ButtonAction(() => { SceneManager.LoadScene("ARAquarium"); OnAquariumClick?.Invoke(); });
    }
}
