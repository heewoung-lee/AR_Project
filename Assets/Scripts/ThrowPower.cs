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

    public Transform fishingRod; // ���˴��� Transform
    public GameObject fishingFloatPrefab; // ���� Prefab
    public Transform lineAttachment; // �������� �پ��ִ� ��ġ
    private GameObject fishingFloatInstance; // ������ �� �ν��Ͻ�
    private LineRenderer lineRenderer; // �������� �׸��� LineRenderer

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

        // ���˴��� �ʱ� ȸ���� ����
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

        // ���� �ִϸ��̼� ���� Ȯ��
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("CastFloat") && stateInfo.normalizedTime < 1.0f)
        {
            // �ִϸ��̼��� ��� ���� ���� �Է��� ����
            _isThrowing = true;
        }
        else
        {
            _isThrowing = false;
        }

        // ������ ������Ʈ
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
        yield return new WaitForSeconds(0.3f); // �ִϸ��̼��� ���۵� �� �ణ�� ������
        Vector3 spawnPoint = lineAttachment.position;
        Vector3 castDirection = fishingRod.forward + Vector3.up;
        fishingFloatInstance = Instantiate(fishingFloatPrefab, spawnPoint, Quaternion.identity);
        Rigidbody fishingFloatRb = fishingFloatInstance.GetComponent<Rigidbody>();
        float castForce = _throwPowerValue / POWER_MAXVALUE * 20f; // ������ ���� �Ŀ��� ����Ͽ� ����
        fishingFloatRb.AddForce(castDirection * castForce, ForceMode.Impulse);
    }

    // IUIActions �������̽� ����
    public void OnNavigate(InputAction.CallbackContext context) { }
    public void OnSubmit(InputAction.CallbackContext context) { }
    public void OnCancel(InputAction.CallbackContext context) { }
    public void OnPoint(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (_isThrowing)
        {
            // �ִϸ��̼��� ��� ���� ���� �Է��� ����
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
            _throwPowerValue = 0f;  // �Ŀ� �ʱ�ȭ
            _throwPowerSlider.value = _throwPowerValue;
        }
    }
    public void OnScrollWheel(InputAction.CallbackContext context) { }
    public void OnMiddleClick(InputAction.CallbackContext context) { }
    public void OnRightClick(InputAction.CallbackContext context) { }
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
}
