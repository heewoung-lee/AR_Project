using TMPro;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public static int PlayerGolds
    {  get => _playerGold; 
       set
       {
            _playerGold = value;
            if (_playerGold <= 0) _playerGold = 0;
            _playerGoldText.text = _playerGold.ToString();
       }
    }
    public static TMP_Text _playerGoldText;
    public static int _playerGold = 20000;

    private void Start()
    {
        _playerGoldText = GetComponent<TMP_Text>();
        _playerGoldText.text = _playerGold.ToString();
    }

}
