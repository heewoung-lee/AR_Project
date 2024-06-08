using UnityEngine;
using System.Collections.Generic;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager _instance;

    public static UI_Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UI_Manager>();
            }
            return _instance;
        }
    }
    /// <summary>
    /// 수정부 
    /// </summary>
    private Dictionary<string, Canvas> _uiElements = new Dictionary<string, Canvas>(); // 메인 UI들
    private Dictionary<string, GameObject> _subUiElements = new Dictionary<string, GameObject>(); // 메인 UI 안에 있는 애들

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterUI(string name, Canvas canvas)
    {
        if (!_uiElements.ContainsKey(name))
        {
            _uiElements.Add(name, canvas);
        }
    }



    public void ToggleUI(string name)
    {
        if (_uiElements[name].enabled) { _uiElements[name].enabled = false;  return; }

        if (_uiElements.ContainsKey(name)) 
        {
            foreach (var ui in _uiElements.Values)  
            {
                ui.enabled = false;
            }
            _uiElements[name].enabled = true; 
        }
    }

    public void SubRegisterUI(string name, GameObject canvas)
    {
        if (!_subUiElements.ContainsKey(name))
        {
            _subUiElements.Add(name, canvas);
        }
    }

    public void SubToggleUI(string name)
    {

        if (_subUiElements.ContainsKey(name))
        {
            foreach (var ui in _subUiElements.Values)
            {
                ui.SetActive(false);
            }
            _subUiElements[name].SetActive(true);
        }
    }
}
