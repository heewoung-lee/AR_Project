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
    // 채우기 방향을 나타내는 열거형
    public enum FillDirection
    {
        Vertical,    // 수직 방향
        Horizontal   // 수평 방향
    };

    public class SimpleUIManager : MonoBehaviour
    {
        // 낚싯줄 상태 바 관련 설정을 위한 클래스
        [System.Serializable]
        public class FishingLineLoadBar
        {
            [InfoBox("A reference to a user interface object that will be enabled or disabled.")]
            public GameObject _UIObject;   // UI 오브젝트에 대한 참조
            public Transform _loadBar;     // 로드 바의 트랜스폼
            public FillDirection _fillDirection;   // 채우기 방향

            [Space, AddButton("Enable Color Gradient", "_fishingLineLoadBar._enableColorGradient")]
            public bool _enableColorGradient = false;   // 색상 그라디언트 사용 여부

            [Space, ShowVariable("_enableColorGradient")]
            [InfoBox("The image in which the color will be changed.")]
            public Image _loadBarImage;   // 색상이 변경될 이미지
            [ShowVariable("_enableColorGradient")]
            public Color _minLineLoadColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // 최소 로드 색상
            [ShowVariable("_enableColorGradient")]
            public Color _maxLineLoadColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // 최대 로드 색상
            [ShowVariable("_enableColorGradient")]
            public Color _overloadLineColor = new Color { r = 255, g = 255, b = 255, a = 255 };  // 과부하 색상
        }

        [BetterHeader("UI Settings", 20)]
        private FishingSystem _fishingSystem;   // 낚시 시스템에 대한 참조
        public TMP_Text _lootInfoText;   // 전리품 정보 텍스트
        public GameObject _FGTMenu;   // 메뉴 UI 오브젝트
        [Space]
        public FishingLineLoadBar _fishingLineLoadBar;   // 낚싯줄 상태 바 설정
        [Space]
        //public CastForceBar _castForceBar;   // 던지기 힘 바 설정
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

        private FishingLineStatus _lineStatus;   // 낚싯줄 상태
        private bool _showMenu = true;   // 메뉴 표시 여부

        #endregion

        private void Awake()
        {
            _fishingSystem = GameObject.Find("AR Session Origin/AR Camera/Character").GetComponent<FishingSystem>();
            _powerSlider = transform.Find("PowerSlider").GetComponent<Slider>();
            _powerSlider.gameObject.SetActive(false);
            _powerSlider.value = 50;
            _lineStatus = _fishingSystem._fishingRod._lineStatus;   // 낚싯줄 상태 초기화
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
            };//물고기 잡으면 버튼 뜨도록 이벤트 실행

            _caughtFishButton.onClick.AddListener(() =>
            {
                FishCaughtButtonClicked(loot, fishingLine, _catchingCamera, catchingWord1, catchingWord2);
                FishInventoryIn?.Invoke(loot.GetComponent<FishScripts>().fishNumber);
                FishLoadLineEnable?.Invoke();//낚시줄 다시 생성
            });
        }

        //물고기 정보 아이디 값 -> 번호가 1 정수형이니까 int만 넘겨 주면되는거 아닌가?
        //public void FishData(int fishID)
        //{
        //    // 인벤토리 업데이트 햇으니까 
        //    // 상점도 따라서 업데이트 해주고
        //    this.inventorylist[0].GetComponent<Image>.sprite = ItemDataBase.ItemData[fishID].itemImage;
        //    this inventorylist[0].itemID = fishID;
        //    ShopFishUpdate.Invoke(fishID); // 상점에도 똑같이 들어갈거고 
        //}

        //이 매개변수가 안에 받는 값이 바뀌는가 ? X 
        public void FishCaughtButtonClicked(GameObject loot, GameObject fishingLine, Camera camera, Image bigCatchWordImage1, Image bigCatchWordImage2)//물고기를 잡았을때 뜨는 버튼의 이벤트 구현
        {
            Destroy(loot);
            Destroy(fishingLine);
            camera.enabled = false;
            bigCatchWordImage1.enabled = false;
            bigCatchWordImage2.enabled = false;
            _caughtFishButton.gameObject.SetActive(false);
            camera.GetComponent<LookatFish>().enabled = false; 

            //TODO: 인벤토리에 잡은 물고기가 들어가게끔 수정
        }

        private void Update()
        {
            ControlFishingLineLoadBar();   // 낚싯줄 상태 바 제어
            ControlMenu();   // 메뉴 제어
        }

        private void ControlMenu()
        {
            if (Input.GetKeyDown(KeyCode.Tab))   // Tab 키를 누르면
                _showMenu = !_showMenu;   // 메뉴 표시 여부를 토글

            if (_showMenu)   // 메뉴가 표시되면
            {
                _FGTMenu.SetActive(true);   // 메뉴 활성화
                Cursor.lockState = CursorLockMode.Confined;   // 커서 고정
            }
            else   // 메뉴가 숨겨지면
            {
                _FGTMenu.SetActive(false);   // 메뉴 비활성화
                Cursor.lockState = CursorLockMode.Locked;   // 커서 잠금
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
            if (!_fishingSystem._advanced._caughtLoot)   // 전리품을 잡지 않았으면
            {
                _fishingLineLoadBar._UIObject.SetActive(false);   // 낚싯줄 상태 바 비활성화
                _lootInfoText.gameObject.SetActive(false);   // 전리품 정보 텍스트 비활성화
                return;
            }

            _lootInfoText.gameObject.SetActive(true);   // 전리품 정보 텍스트 활성화
            _fishingLineLoadBar._UIObject.SetActive(true);   // 낚싯줄 상태 바 활성화

            ShowLootInfo(_fishingSystem._advanced._caughtLootData, _lootInfoText);   // 전리품 정보 표시

            float loadProgress = CalculateProgess(_lineStatus._currentLineLoad, _lineStatus._maxLineLoad);   // 낚싯줄 로드 진행도 계산

            if (_fishingLineLoadBar._enableColorGradient)   // 색상 그라디언트가 활성화된 경우
            {
                Color currentLoadBarColor = Color32.Lerp(_fishingLineLoadBar._minLineLoadColor, _fishingLineLoadBar._maxLineLoadColor, loadProgress);   // 현재 로드 바 색상 계산

                if (_lineStatus._currentOverLoad != 0)   // 과부하가 있으면
                {
                    float overloadProgress = CalculateProgess(_lineStatus._currentOverLoad, _lineStatus._overLoadDuration);   // 과부하 진행도 계산
                    currentLoadBarColor = Color32.Lerp(_fishingLineLoadBar._maxLineLoadColor, _fishingLineLoadBar._overloadLineColor, overloadProgress);   // 과부하 색상 계산
                }

                _fishingLineLoadBar._loadBarImage.color = currentLoadBarColor;   // 로드 바 이미지 색상 설정
            }

            SetBarScale(_fishingLineLoadBar._fillDirection, _fishingLineLoadBar._loadBar, loadProgress);   // 로드 바 크기 설정
        }

        private void ShowLootInfo(FishingLootData lootData, TMP_Text infoGameObject)
        {
            infoGameObject.text = lootData._lootName + " | " + lootData._lootTier.ToString() + " | " + lootData._lootDescription;   // 전리품 정보 텍스트 설정
        }

        private void SetBarScale(FillDirection fillDirection, Transform barTransform, float progress)
        {
            Vector3 barScale = Vector3.zero;   // 바의 기본 크기를 0으로 초기화

            if (fillDirection == FillDirection.Vertical)   // 채우기 방향이 수직이면
                barScale = new Vector3(barTransform.localScale.x, progress, barTransform.localScale.z);   // 수직으로 크기 설정
            else if (fillDirection == FillDirection.Horizontal)   // 채우기 방향이 수평이면
                barScale = new Vector3(progress, barTransform.localScale.y, barTransform.localScale.z);   // 수평으로 크기 설정

            barTransform.localScale = barScale;   // 바의 크기 설정
        }

        private static float CalculateProgess(float input, float max)
        {
            float x = Mathf.InverseLerp(0f, max, input);   // 입력값을 0에서 1 사이의 값으로 변환
            float value = Mathf.Lerp(0f, 1f, x);   // 변환된 값을 사용하여 최종 진행도 계산

            return value;   // 계산된 진행도 반환
        }
    }
}
