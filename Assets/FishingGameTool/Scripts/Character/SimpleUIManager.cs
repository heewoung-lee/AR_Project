using FishingGameTool.Fishing;
using FishingGameTool.Fishing.Line;
using UnityEngine;
using UnityEngine.UI;
using FishingGameTool.CustomAttribute;
using FishingGameTool.Fishing.LootData;
using TMPro;
using DG.Tweening;
using System;

namespace FishingGameTool.Example
{
    // ä��� ������ ��Ÿ���� ������
    public enum FillDirection
    {
        Vertical,    // ���� ����
        Horizontal   // ���� ����
    };

    public class SimpleUIManager : MonoBehaviour
    {
        // ������ ���� �� ���� ������ ���� Ŭ����
        [System.Serializable]
        public class FishingLineLoadBar
        {
            [InfoBox("A reference to a user interface object that will be enabled or disabled.")]
            public GameObject _UIObject;   // UI ������Ʈ�� ���� ����
            public Transform _loadBar;     // �ε� ���� Ʈ������
            public FillDirection _fillDirection;   // ä��� ����

            [Space, AddButton("Enable Color Gradient", "_fishingLineLoadBar._enableColorGradient")]
            public bool _enableColorGradient = false;   // ���� �׶���Ʈ ��� ����

            [Space, ShowVariable("_enableColorGradient")]
            [InfoBox("The image in which the color will be changed.")]
            public Image _loadBarImage;   // ������ ����� �̹���
            [ShowVariable("_enableColorGradient")]
            public Color _minLineLoadColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // �ּ� �ε� ����
            [ShowVariable("_enableColorGradient")]
            public Color _maxLineLoadColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // �ִ� �ε� ����
            [ShowVariable("_enableColorGradient")]
            public Color _overloadLineColor = new Color { r = 255, g = 255, b = 255, a = 255 };  // ������ ����
        }

        [BetterHeader("UI Settings", 20)]
        private FishingSystem _fishingSystem;   // ���� �ý��ۿ� ���� ����
        public TMP_Text _lootInfoText;   // ����ǰ ���� �ؽ�Ʈ
        public GameObject _FGTMenu;   // �޴� UI ������Ʈ
        [Space]
        public FishingLineLoadBar _fishingLineLoadBar;   // ������ ���� �� ����
        [Space]
        //public CastForceBar _castForceBar;   // ������ �� �� ����
        [Space]
        private Slider _powerSlider;
        Color fillcolor = new Color(1, 0, 0, 1);
        Color backgroundColor = new Color(1, 1, 1, 1);
        private Button _caughtFishButton;

        public int fishID = 500;
        public event Action<int> FishInventoryIn;
        public event Action FishLoadLineEnable;

        GameObject loot;
        GameObject fishingLine;
        Camera _catchingCamera;
        Image catchingWord1;
        Image catchingWord2;
        #region PRIVATE VARIABLES

        private FishingLineStatus _lineStatus;   // ������ ����
        private bool _showMenu = true;   // �޴� ǥ�� ����

        #endregion

        private void Awake()
        {
            _fishingSystem = GameObject.Find("AR Session Origin/AR Camera/Character").GetComponent<FishingSystem>();
            _powerSlider = transform.Find("PowerSlider").GetComponent<Slider>();
            _powerSlider.gameObject.SetActive(false);
            _powerSlider.value = 50;
            _lineStatus = _fishingSystem._fishingRod._lineStatus;   // ������ ���� �ʱ�ȭ
            _fishingSystem.showPowerbarEvent += ShowPowerBar;
            _caughtFishButton = transform.Find("CaughtfishConfirmButton").GetComponent<Button>();
            _caughtFishButton.gameObject.SetActive(false);

            _fishingSystem.viewFishCaughtButtonEvent += (loot, fishingLine, Camera, bigCatchWordImage1, bigCatchWordImage2) =>
            {
                this.loot = loot;
                this.fishingLine = fishingLine;
                _catchingCamera = Camera;
                catchingWord1 = bigCatchWordImage1;
                catchingWord2 = bigCatchWordImage2;
                _caughtFishButton.gameObject.SetActive(true);
            };//����� ������ ��ư �ߵ��� �̺�Ʈ ����

            _caughtFishButton.onClick.AddListener(() =>
            {
                FishCaughtButtonClicked(loot, fishingLine, _catchingCamera, catchingWord1, catchingWord2);
                FishInventoryIn?.Invoke(loot.GetComponent<FishScripts>().fishNumber);
                FishLoadLineEnable?.Invoke();//������ �ٽ� ����
            });
        }

        //����� ���� ���̵� �� -> ��ȣ�� 1 �������̴ϱ� int�� �Ѱ� �ָ�Ǵ°� �ƴѰ�?
        //public void FishData(int fishID)
        //{
        //    // �κ��丮 ������Ʈ �����ϱ� 
        //    // ������ ���� ������Ʈ ���ְ�
        //    this.inventorylist[0].GetComponent<Image>.sprite = ItemDataBase.ItemData[fishID].itemImage;
        //    this inventorylist[0].itemID = fishID;
        //    ShopFishUpdate.Invoke(fishID); // �������� �Ȱ��� ���Ű� 
        //}

        //�� �Ű������� �ȿ� �޴� ���� �ٲ�°� ? X 
        public void FishCaughtButtonClicked(GameObject loot, GameObject fishingLine, Camera camera, Image bigCatchWordImage1, Image bigCatchWordImage2)//����⸦ ������� �ߴ� ��ư�� �̺�Ʈ ����
        {
            Destroy(loot);
            Destroy(fishingLine);
            camera.enabled = false;
            bigCatchWordImage1.enabled = false;
            bigCatchWordImage2.enabled = false;
            _caughtFishButton.gameObject.SetActive(false);
            camera.GetComponent<LookatFish>().enabled = false; 

            //TODO: �κ��丮�� ���� ����Ⱑ ���Բ� ����
        }

        private void Update()
        {
            ControlFishingLineLoadBar();   // ������ ���� �� ����
            ControlMenu();   // �޴� ����
        }

        private void ControlMenu()
        {
            if (Input.GetKeyDown(KeyCode.Tab))   // Tab Ű�� ������
                _showMenu = !_showMenu;   // �޴� ǥ�� ���θ� ���

            if (_showMenu)   // �޴��� ǥ�õǸ�
            {
                _FGTMenu.SetActive(true);   // �޴� Ȱ��ȭ
                Cursor.lockState = CursorLockMode.Confined;   // Ŀ�� ����
            }
            else   // �޴��� ��������
            {
                _FGTMenu.SetActive(false);   // �޴� ��Ȱ��ȭ
                Cursor.lockState = CursorLockMode.Locked;   // Ŀ�� ���
            }
        }

        private float ShowPowerBar()
        {
            if (_fishingSystem.castInput == false)
            {
                _powerSlider.gameObject.SetActive(true);
                _powerSlider.value = (_fishingSystem.dragDistance / Screen.height) * _fishingSystem._maxCastForce;
                Debug.Log(_fishingSystem.dragDistance / Screen.height * _fishingSystem._maxCastForce);


                _powerSlider.fillRect.GetComponent<Image>().DOFade(0, 1).OnComplete(() =>
                {
                    setimage(_powerSlider.gameObject, _powerSlider.fillRect.GetComponent<Image>(), fillcolor);
                });
                _powerSlider.GetComponentInChildren<Image>().DOFade(0, 1).OnComplete(() =>
                {
                    setimage(_powerSlider.gameObject, _powerSlider.GetComponentInChildren<Image>(), backgroundColor);
                });
            }
            return _powerSlider.value;
        }

        public void setimage(GameObject gameobject, Image image, Color color)
        {
            gameobject.SetActive(false);
            image.color = color;
        }

        private void ControlFishingLineLoadBar()
        {
            if (!_fishingSystem._advanced._caughtLoot)   // ����ǰ�� ���� �ʾ�����
            {
                _fishingLineLoadBar._UIObject.SetActive(false);   // ������ ���� �� ��Ȱ��ȭ
                _lootInfoText.gameObject.SetActive(false);   // ����ǰ ���� �ؽ�Ʈ ��Ȱ��ȭ
                return;
            }

            _lootInfoText.gameObject.SetActive(true);   // ����ǰ ���� �ؽ�Ʈ Ȱ��ȭ
            _fishingLineLoadBar._UIObject.SetActive(true);   // ������ ���� �� Ȱ��ȭ

            ShowLootInfo(_fishingSystem._advanced._caughtLootData, _lootInfoText);   // ����ǰ ���� ǥ��

            float loadProgress = CalculateProgess(_lineStatus._currentLineLoad, _lineStatus._maxLineLoad);   // ������ �ε� ���൵ ���

            if (_fishingLineLoadBar._enableColorGradient)   // ���� �׶���Ʈ�� Ȱ��ȭ�� ���
            {
                Color currentLoadBarColor = Color32.Lerp(_fishingLineLoadBar._minLineLoadColor, _fishingLineLoadBar._maxLineLoadColor, loadProgress);   // ���� �ε� �� ���� ���

                if (_lineStatus._currentOverLoad != 0)   // �����ϰ� ������
                {
                    float overloadProgress = CalculateProgess(_lineStatus._currentOverLoad, _lineStatus._overLoadDuration);   // ������ ���൵ ���
                    currentLoadBarColor = Color32.Lerp(_fishingLineLoadBar._maxLineLoadColor, _fishingLineLoadBar._overloadLineColor, overloadProgress);   // ������ ���� ���
                }

                _fishingLineLoadBar._loadBarImage.color = currentLoadBarColor;   // �ε� �� �̹��� ���� ����
            }

            SetBarScale(_fishingLineLoadBar._fillDirection, _fishingLineLoadBar._loadBar, loadProgress);   // �ε� �� ũ�� ����
        }

        private void ShowLootInfo(FishingLootData lootData, TMP_Text infoGameObject)
        {
            infoGameObject.text = lootData._lootName + " | " + lootData._lootTier.ToString() + " | " + lootData._lootDescription;   // ����ǰ ���� �ؽ�Ʈ ����
        }

        private void SetBarScale(FillDirection fillDirection, Transform barTransform, float progress)
        {
            Vector3 barScale = Vector3.zero;   // ���� �⺻ ũ�⸦ 0���� �ʱ�ȭ

            if (fillDirection == FillDirection.Vertical)   // ä��� ������ �����̸�
                barScale = new Vector3(barTransform.localScale.x, progress, barTransform.localScale.z);   // �������� ũ�� ����
            else if (fillDirection == FillDirection.Horizontal)   // ä��� ������ �����̸�
                barScale = new Vector3(progress, barTransform.localScale.y, barTransform.localScale.z);   // �������� ũ�� ����

            barTransform.localScale = barScale;   // ���� ũ�� ����
        }

        private static float CalculateProgess(float input, float max)
        {
            float x = Mathf.InverseLerp(0f, max, input);   // �Է°��� 0���� 1 ������ ������ ��ȯ
            float value = Mathf.Lerp(0f, 1f, x);   // ��ȯ�� ���� ����Ͽ� ���� ���൵ ���

            return value;   // ���� ���൵ ��ȯ
        }
    }
}
