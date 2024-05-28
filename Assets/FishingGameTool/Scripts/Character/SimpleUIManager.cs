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

        // ������ �� �� ���� ������ ���� Ŭ����
        [System.Serializable]
        public class CastForceBar
        {
            [InfoBox("A reference to a user interface object that will be enabled or disabled.")]
            public GameObject _UIObject;   // UI ������Ʈ�� ���� ����
            public Transform _castBar;     // ĳ��Ʈ ���� Ʈ������
            public FillDirection _fillDirection;   // ä��� ����

            [Space, AddButton("Enable Color Gradient", "_castForceBar._enableColorGradient")]
            public bool _enableColorGradient = false;   // ���� �׶���Ʈ ��� ����

            [Space, ShowVariable("_enableColorGradient")]
            [InfoBox("The image in which the color will be changed.")]
            public Image _castBarImage;   // ������ ����� �̹���
            [ShowVariable("_enableColorGradient")]
            public Color _minCastForceColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // �ּ� ĳ��Ʈ ����
            [ShowVariable("_enableColorGradient")]
            public Color _maxCastForceColor = new Color { r = 255, g = 255, b = 255, a = 255 };   // �ִ� ĳ��Ʈ ����
        }

        [BetterHeader("UI Settings", 20)]
        private FishingSystem _fishingSystem;   // ���� �ý��ۿ� ���� ����
        public TMP_Text _lootInfoText;   // ����ǰ ���� �ؽ�Ʈ
        public GameObject _FGTMenu;   // �޴� UI ������Ʈ
        [Space]
        public FishingLineLoadBar _fishingLineLoadBar;   // ������ ���� �� ����
        [Space]
        public CastForceBar _castForceBar;   // ������ �� �� ����

        public CastForceBar _castForceTestBar;
        

        #region PRIVATE VARIABLES

        private FishingLineStatus _lineStatus;   // ������ ����
        private bool _showMenu = true;   // �޴� ǥ�� ����

        #endregion

        private void Awake()
        {
            _fishingSystem = GameObject.Find("AR Session Origin/AR Camera/Character").GetComponent<FishingSystem>();
            _lineStatus = _fishingSystem._fishingRod._lineStatus;   // ������ ���� �ʱ�ȭ
        }

        private void Update()
        {
            ControlFishingLineLoadBar();   // ������ ���� �� ����
            ControlCastBar();   // ������ �� �� ����
            ControlMenu();   // �޴� ����
            ControlPowerBar();
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

        private void ControlCastBar()
        {
            if (_fishingSystem._advanced._caughtLoot || !_fishingSystem._castInput)   // ����ǰ�� ��Ұų� ������ �Է��� ������
            {
                _castForceBar._UIObject.SetActive(false);   // ĳ��Ʈ �� ��Ȱ��ȭ
                return;
            }

            _castForceBar._UIObject.SetActive(true);   // ĳ��Ʈ �� Ȱ��ȭ

            float castProgress = CalculateProgess(_fishingSystem._currentCastForce, _fishingSystem._maxCastForce);   // ĳ��Ʈ ���൵ ���

            if (_castForceBar._enableColorGradient)   // ���� �׶���Ʈ�� Ȱ��ȭ�� ���
            {
                Color currentCastBarColor = Color32.Lerp(_castForceBar._minCastForceColor, _castForceBar._maxCastForceColor, castProgress);   // ���� ĳ��Ʈ �� ���� ���
                _castForceBar._castBarImage.color = currentCastBarColor;   // ĳ��Ʈ �� �̹��� ���� ����
            }

            SetBarScale(_castForceBar._fillDirection, _castForceBar._castBar, castProgress);   // ĳ��Ʈ �� ũ�� ����
        }
        private void ControlPowerBar()
        {
            if (_fishingSystem._advanced._caughtLoot || !_fishingSystem._castInput)   // ����ǰ�� ��Ұų� ������ �Է��� ������
            {
                _castForceTestBar._UIObject.SetActive(false);   // ĳ��Ʈ �� ��Ȱ��ȭ
                return;
            }

            _castForceTestBar._UIObject.SetActive(true); // ĳ��Ʈ �� Ȱ��ȭ

            // _dragDistance ���� ����Ͽ� �� ���
            float inputPower = (_fishingSystem.dragDistance / Screen.width) * _fishingSystem._maxCastForce;
            float castProgress = CalculateProgess(inputPower, _fishingSystem._maxCastForce);
            SetBarScale(_castForceTestBar._fillDirection, _castForceTestBar._castBar, castProgress);

            // ���İ� ���� �ڷ�ƾ ����
            StartCoroutine(PowerBarFadeOut());
        }

        private IEnumerator PowerBarFadeOut()
        {
            Image castBarImage = _castForceTestBar._UIObject.GetComponent<Image>();
            Color originalColor = castBarImage.color;

            float elapsedTime = 0f;
            float duration = 2f; // 2�� ���� ���İ� ����

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                castBarImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            // 2�� �� ĳ��Ʈ �� ��Ȱ��ȭ
            _castForceTestBar._UIObject.SetActive(false);
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
