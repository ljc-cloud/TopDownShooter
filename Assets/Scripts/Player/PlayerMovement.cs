using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerControls _playerControls;
    private CharacterController _characterController;
    private Animator _animator;
    private Player _player;

    [Header("Movement Info")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    private float _speed;
    private Vector3 _movementDirection;
    private float _verticalVelocity;
    private Vector2 _moveInput;
    private bool _isRunning;
    
    public Vector2 MoveInput => _moveInput;
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
        _speed = walkSpeed;
        
        AssignInput();
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
        ControlAnimation();
    }

    private void ControlAnimation()
    {
        // 获取移动向量在transform.right的投影长度
        float xVelocity = Vector3.Dot(_movementDirection.normalized, transform.right);
        // 获取移动向量在transform.forward的投影长度
        float zVelocity = Vector3.Dot(_movementDirection.normalized, transform.forward);

        // xVelocity = _movementDirection.x;
        // zVelocity = _movementDirection.z;
        
        _animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        _animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);
        bool playerRunAnimation = _isRunning && _movementDirection.sqrMagnitude > 0f;
        _animator.SetBool("isRunning", playerRunAnimation);
    }

    private void ApplyRotation()
    {
        Vector3 lookDirection = _player.Aim.GetMouseHit().point - transform.position;
        lookDirection.y = 0f;
        lookDirection.Normalize();
        
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        _movementDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;

        ApplyGravity();
        if (_movementDirection.sqrMagnitude > 0f)
        {
            _characterController.Move(_movementDirection * (_speed * Time.deltaTime));
        }
    }

    private void ApplyGravity()
    {
        if (!_characterController.isGrounded)
        {
            _verticalVelocity -= 9.81f * Time.deltaTime;
            _movementDirection.y = _verticalVelocity;
        }
        else
        {
            _verticalVelocity = 0f;
        }
    }
    

    #region Input Register

    private void AssignInput()
    {
        _playerControls = _player.Controls; 
        _playerControls.Character.Movement.performed += MovementPerformed;
        _playerControls.Character.Movement.canceled += MovementCanceled;
        _playerControls.Character.Run.performed += RunPerformed;
        _playerControls.Character.Run.canceled += RunCanceled;
    }
    private void MovementPerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    private void MovementCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
        
    }
   
    private void RunPerformed(InputAction.CallbackContext context)
    {
        _isRunning = true;
        _speed = runSpeed;
    }
    private void RunCanceled(InputAction.CallbackContext context)
    {
        _isRunning = false;
        _speed = walkSpeed;
    }

    #endregion
}
