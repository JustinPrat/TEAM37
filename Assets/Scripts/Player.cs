using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 _movementValue;
    private MovementState _movementState;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Vector2 _speed;
    [SerializeField] private float _jumpAmount;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _airControl;
    private float _beforeJumpHeigth;

    [SerializeField] private PlayerInput _playerInput;

    private enum MovementState
    {
        DRIVE,
        JUMP
    }

    void Start()
    {
        _playerInput.actions["Move"].performed += OnMovePerformed;
        _playerInput.actions["Move"].canceled += OnMoveCanceled;
        _playerInput.actions["Jump"].started += OnJumpStarted;
    }


    private void OnDestroy()
    {
        _playerInput.actions["Move"].performed -= OnMovePerformed;
        _playerInput.actions["Move"].canceled -= OnMoveCanceled;
        _playerInput.actions["Jump"].started -= OnJumpStarted;
    }
    public void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        _movementValue = Vector2.zero;
    }

    public void OnJumpStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (_movementState == MovementState.DRIVE)
        {
            _movementState = MovementState.JUMP;
            _beforeJumpHeigth = transform.position.y;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _rigidbody.AddForce(new Vector2(0, _jumpAmount), ForceMode2D.Impulse);
        }
    }

    public void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        _movementValue = ctx.ReadValue<Vector2>();
    }

    private void OnLanded ()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _movementState = MovementState.DRIVE;
    }


    void Update()
    {
        Vector2 inputVelocity = _movementValue * _speed;

        switch (_movementState)
        {
            case MovementState.DRIVE:
                _rigidbody.velocity = inputVelocity;
                break;

            case MovementState.JUMP:
                _rigidbody.velocity += new Vector2(inputVelocity.x * _airControl, 0);
                _rigidbody.velocity = new Vector2(Mathf.Clamp(_rigidbody.velocity.x, -_maxSpeed, _maxSpeed) ,_rigidbody.velocity.y);
                _rigidbody.velocity -= new Vector2(0, 9.81f * Time.deltaTime);

                if (transform.position.y < _beforeJumpHeigth)
                {
                    OnLanded();
                }
                break;

            default:
                break;
        }
    }
}
