using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class ThrowPower : MonoBehaviour, PlayerInputActions.IUIActions
{
    private const float POWER_MAXVALUE = 100f;
    private const float POWER_MINVALUE = 0f;

    private Slider _throwPowerSlider;
    private float _throwPowerValue = 0;
    private float _increasePower = 30f;

    private bool _isCharging = false;
    private bool _isThrowing = false;

    private PlayerInputActions _inputActions;
    private Animator _animator;

    public Transform fishingRod; // 낚싯대의 Transform
    public GameObject fishingFloatPrefab; // 찌의 Prefab
    public Transform lineAttachment; // 낚싯줄이 붙어있는 위치
    private GameObject fishingFloatInstance; // 생성된 찌 인스턴스
    private LineRenderer lineRenderer; // 낚싯줄을 그리는 LineRenderer

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.UI.SetCallbacks(this);
        _inputActions.Enable();
        _animator = fishingRod.GetComponent<Animator>();
        lineRenderer = fishingRod.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _throwPowerSlider = GetComponentInChildren<Slider>();
        _throwPowerSlider.value = POWER_MINVALUE;

        // 낚싯대의 초기 회전값 설정
        fishingRod.localRotation = Quaternion.Euler(-30f, fishingRod.localEulerAngles.y, fishingRod.localEulerAngles.z);
    }

    private void Update()
    {
        if (_isCharging)
        {
            _throwPowerValue += _increasePower * Time.deltaTime;
            if (_throwPowerValue > POWER_MAXVALUE)
            {
                _throwPowerValue = POWER_MAXVALUE;
            }
            _throwPowerSlider.value = _throwPowerValue;
        }

        // 현재 애니메이션 상태 확인
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("CastFloat") && stateInfo.normalizedTime < 1.0f)
        {
            // 애니메이션이 재생 중일 때는 입력을 무시
            _isThrowing = true;
        }
        else
        {
            _isThrowing = false;
        }

        // 낚싯줄 업데이트
        if (fishingFloatInstance != null)
        {
            lineRenderer.SetPosition(0, lineAttachment.position);
            lineRenderer.SetPosition(1, fishingFloatInstance.transform.position);
        }
    }

    public void CastBobber()
    {
        _animator.Play("CastFloat");
        StartCoroutine(SpawnAndThrowFloat());
    }

    private IEnumerator SpawnAndThrowFloat()
    {
        yield return new WaitForSeconds(0.3f); // 애니메이션이 시작된 후 약간의 딜레이
        Vector3 spawnPoint = lineAttachment.position;
        Vector3 castDirection = fishingRod.forward + Vector3.up;
        fishingFloatInstance = Instantiate(fishingFloatPrefab, spawnPoint, Quaternion.identity);
        Rigidbody fishingFloatRb = fishingFloatInstance.GetComponent<Rigidbody>();
        float castForce = _throwPowerValue / POWER_MAXVALUE * 20f; // 던지는 힘을 파워에 비례하여 설정
        fishingFloatRb.AddForce(castDirection * castForce, ForceMode.Impulse);
    }

    // IUIActions 인터페이스 구현
    public void OnNavigate(InputAction.CallbackContext context) { }
    public void OnSubmit(InputAction.CallbackContext context) { }
    public void OnCancel(InputAction.CallbackContext context) { }
    public void OnPoint(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (_isThrowing)
        {
            // 애니메이션이 재생 중일 때는 입력을 무시
            return;
        }

        if (context.ReadValueAsButton())
        {
            _isCharging = true;
        }
        else
        {
            _isCharging = false;
            CastBobber();
            _throwPowerValue = 0f;  // 파워 초기화
            _throwPowerSlider.value = _throwPowerValue;
        }
    }
    public void OnScrollWheel(InputAction.CallbackContext context) { }
    public void OnMiddleClick(InputAction.CallbackContext context) { }
    public void OnRightClick(InputAction.CallbackContext context) { }
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
}
