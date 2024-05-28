using FishingGameTool.Fishing;
using FishingGameTool.Fishing.Line;
using UnityEngine;
using UnityEngine.UI;
using FishingGameTool.CustomAttribute;
using FishingGameTool.Fishing.LootData;
using TMPro;
using System.Collections;

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

        // 던지기 힘 바 관련 설정을 위한 클래스
        [System.Serializable]
        public class CastForceBar
        {
            [InfoBox("A reference to a user interface object that will be enabled or disabled.")]
            public GameObject _UIObject;   // UI 오브젝트에 대한 참조
            public Transform _castBar;     // 캐스트 바의 트랜스폼
            public FillDirection _fillDirection;   // 채우기 방향

            [Space, AddButton("Enable Color Gradient", "_castForceBar._enableColorGradient")]
            public bool _enableColorGradient = false;   // 색상 그라디언트 사용 여부

            [Space, ShowVariable("_enableColorGradient")]
            [InfoBox("The image in which the color will be changed.")]
            public Image _castBarImage;   // 색상이 변경될 이미지
            [ShowVariable("_enableColorGradient")]
            public Color _minCastForceColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // 최소 캐스트 색상
            [ShowVariable("_enableColorGradient")]
            public Color _maxCastForceColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // 최대 캐스트 색상
        }

        [BetterHeader("UI Settings", 20)]
        private FishingSystem _fishingSystem;   // 낚시 시스템에 대한 참조
        public TMP_Text _lootInfoText;   // 전리품 정보 텍스트
        public GameObject _FGTMenu;   // 메뉴 UI 오브젝트
        [Space]
        public FishingLineLoadBar _fishingLineLoadBar;   // 낚싯줄 상태 바 설정
        [Space]
        public CastForceBar _castForceBar;   // 던지기 힘 바 설정

        public CastForceBar _castForceTestBar;
        

        #region PRIVATE VARIABLES

        private FishingLineStatus _lineStatus;   // 낚싯줄 상태
        private bool _showMenu = true;   // 메뉴 표시 여부

        #endregion

        private void Awake()
        {
            _fishingSystem = GameObject.Find("AR Session Origin/AR Camera/Character").GetComponent<FishingSystem>();
            _lineStatus = _fishingSystem._fishingRod._lineStatus;   // 낚싯줄 상태 초기화
        }

        private void Update()
        {
            ControlFishingLineLoadBar();   // 낚싯줄 상태 바 제어
            ControlCastBar();   // 던지기 힘 바 제어
            ControlMenu();   // 메뉴 제어
            ControlPowerBar();
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

        private void ControlCastBar()
        {
            if (_fishingSystem._advanced._caughtLoot || !_fishingSystem._castInput)   // 전리품을 잡았거나 던지기 입력이 없으면
            {
                _castForceBar._UIObject.SetActive(false);   // 캐스트 바 비활성화
                return;
            }

            _castForceBar._UIObject.SetActive(true);   // 캐스트 바 활성화

            float castProgress = CalculateProgess(_fishingSystem._currentCastForce, _fishingSystem._maxCastForce);   // 캐스트 진행도 계산

            if (_castForceBar._enableColorGradient)   // 색상 그라디언트가 활성화된 경우
            {
                Color currentCastBarColor = Color32.Lerp(_castForceBar._minCastForceColor, _castForceBar._maxCastForceColor, castProgress);   // 현재 캐스트 바 색상 계산
                _castForceBar._castBarImage.color = currentCastBarColor;   // 캐스트 바 이미지 색상 설정
            }

            SetBarScale(_castForceBar._fillDirection, _castForceBar._castBar, castProgress);   // 캐스트 바 크기 설정
        }
        private void ControlPowerBar()
        {
            if (_fishingSystem._advanced._caughtLoot || !_fishingSystem._castInput)   // 전리품을 잡았거나 던지기 입력이 없으면
            {
                _castForceTestBar._UIObject.SetActive(false);   // 캐스트 바 비활성화
                return;
            }

            _castForceTestBar._UIObject.SetActive(true); // 캐스트 바 활성화

            // _dragDistance 값을 사용하여 힘 계산
            float inputPower = (_fishingSystem.dragDistance / Screen.width) * _fishingSystem._maxCastForce;
            float castProgress = CalculateProgess(inputPower, _fishingSystem._maxCastForce);
            SetBarScale(_castForceTestBar._fillDirection, _castForceTestBar._castBar, castProgress);

            // 알파값 조정 코루틴 시작
            StartCoroutine(PowerBarFadeOut());
        }

        private IEnumerator PowerBarFadeOut()
        {
            Image castBarImage = _castForceTestBar._UIObject.GetComponent<Image>();
            Color originalColor = castBarImage.color;

            float elapsedTime = 0f;
            float duration = 2f; // 2초 동안 알파값 감소

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                castBarImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            // 2초 후 캐스트 바 비활성화
            _castForceTestBar._UIObject.SetActive(false);
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
