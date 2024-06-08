using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGotoAquarium : MonoBehaviour
{
    ButtonEventComponent _buttonevent;

    private void Start()
    {
        _buttonevent = gameObject.AddComponent<ButtonEventComponent>();
        _buttonevent.ButtonAction(() => SceneManager.LoadScene("AquariumFurniture"));
    }
}
