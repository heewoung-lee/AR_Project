using System;
using UnityEngine;

public class FishRodActionButton : MonoBehaviour
{
    ButtonEventComponent buttonEvent;
    int _fishRodID;
    public event Action<int> FishRodButtonAction;

    private void Start()
    {
        buttonEvent = GetComponent<ButtonEventComponent>();
        buttonEvent.ButtonAction(() => FishRodButtonAction.Invoke(_fishRodID));
    }
}
