using UnityEngine;
using FishingGameTool.CustomAttribute;
using System;

namespace FishingGameTool.Example
{
    [RequireComponent(typeof(Animator), typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        [BetterHeader("Character Movement Settings", 20), Space]
        public float _moveSpeed;
        public float _turnSmoothTime;
        public float _gravity;
        public float _gravityAccel;
        [Space]
        public LayerMask _groundMask;
        public Vector3 _groundCheckerSize;
        [Space]
        [BetterHeader("Camera Settings", 20)]
        public Transform _fppCamera;

        #region PRIVATE VARIABLES

        private Vector2 _moveInput;
        private Vector3 _moveVel;
        private Vector3 _gravityVel;
        private float _currentGravityAccel;

        private CharacterController _characterController;
        private Animator _animator;

        #endregion

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();

            // _fppCamera가 할당되지 않은 경우 기본값 설정
            if (_fppCamera == null)
            {
                _fppCamera = Camera.main.transform;
            }

            _fppCamera.gameObject.SetActive(true);
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            Movement();
            Gravity();
        }

        private void Gravity()
        {
            if (IsGrounded())
            {
                _currentGravityAccel = 0f;
                float constantGravity = 2f;
                _gravityVel = Vector3.up * -constantGravity;
            }
            else
            {
                _currentGravityAccel = Mathf.Lerp(_currentGravityAccel, 1f, _gravityAccel * Time.fixedDeltaTime);
                _gravityVel += Vector3.up * _gravity * _currentGravityAccel * Time.fixedDeltaTime;
            }

            _characterController.Move(_gravityVel * Time.fixedDeltaTime);
        }

        private void Movement()
        {
            Vector3 dir = transform.right * _moveInput.x + transform.forward * _moveInput.y;

            AnimationControl(dir);

            _characterController.Move(dir * _moveSpeed * Time.fixedDeltaTime);
        }

        private void AnimationControl(Vector3 dir)
        {
            _animator.SetFloat("Walk", dir.magnitude);
        }

        private bool IsGrounded()
        {
            bool isGrounded = Physics.CheckBox(transform.position, new Vector3(_groundCheckerSize.x / 2, _groundCheckerSize.y / 2, _groundCheckerSize.z / 2), transform.rotation, _groundMask);
            return isGrounded;
        }

        private void HandleInput()
        {
            _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        public Transform GetCurrentCam()
        {
            return _fppCamera;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (IsGrounded())
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position, _groundCheckerSize);
        }

#endif
    }
}
