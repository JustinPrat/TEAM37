using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
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
    [SerializeField] private float _repulseAmount;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _hitDelay;
    [SerializeField] private float _gravityFactor;
    [SerializeField] private float _captureSpeedFactor;
    [SerializeField] private Transform _shadowTransform;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public Player OtherPlayer { get; set; }

    private float _beforeJumpShadowYPos;
    private float _beforeJumpHeigth;
    private bool _canAttack = true;
    private bool _otherInRange;
    private bool _canBeHit = true;

    [SerializeField] private PlayerInput _playerInput;

    private enum MovementState
    {
        DRIVE,
        JUMP,
        CAPTURE
    }

    void Start()
    {
        _playerInput.actions["Move"].performed += OnMovePerformed;
        _playerInput.actions["Move"].canceled += OnMoveCanceled;
        _playerInput.actions["Jump"].started += OnJumpStarted;
        _playerInput.actions["Capture"].performed += OnCapturePerformed;
        _playerInput.actions["Capture"].canceled += OnCaptureCanceled;
        _playerInput.actions["Attack"].performed += OnAttackPerformed;

    }


    private void OnDestroy()
    {
        _playerInput.actions["Move"].performed -= OnMovePerformed;
        _playerInput.actions["Move"].canceled -= OnMoveCanceled;
        _playerInput.actions["Jump"].started -= OnJumpStarted;
        _playerInput.actions["Capture"].performed -= OnCapturePerformed;
        _playerInput.actions["Capture"].canceled -= OnCaptureCanceled;
        _playerInput.actions["Attack"].performed -= OnAttackPerformed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != tag)
        {
            _otherInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != tag)
        {
            _otherInRange = false;
        }
    }

    public void HitPlayer (Vector3 otherPosition)
    {
        if (_canBeHit && _movementState != MovementState.JUMP)
        {
            _canBeHit = false;
            _canAttack = false;
            Vector2 force = transform.position - otherPosition;
            _rigidbody.AddForce(force.normalized * _repulseAmount, ForceMode2D.Impulse);
            Color baseColor = _spriteRenderer.color;
            baseColor.a = 0.5f;
            _spriteRenderer.color = baseColor;
            StartCoroutine(CooldownCoroutine(_hitDelay, OnHitCooldownFinish));
        }
    }

    private void OnHitCooldownFinish()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 1f);
        _canBeHit = true;
        _canAttack = true;
    }

    public void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _movementValue = Vector2.zero;
    }

    public void OnCaptureCanceled(InputAction.CallbackContext ctx)
    {
        _movementState = MovementState.DRIVE;
    }

    public void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        if (_movementState == MovementState.DRIVE)
        {
            _movementState = MovementState.JUMP;
            _beforeJumpHeigth = transform.position.y;
            _beforeJumpShadowYPos = _shadowTransform.position.y;
            _rigidbody.velocity += new Vector2(0, _jumpAmount);
        }
    }

    public void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _movementValue = ctx.ReadValue<Vector2>();
    }

    public void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if ( _movementState == MovementState.DRIVE && _canAttack)
        {
            if (_otherInRange)
            {
                OtherPlayer.HitPlayer(transform.position);
            }

            _canAttack = false;
            _spriteRenderer.color = new Color(1, 0, 0, _spriteRenderer.color.r);
            StartCoroutine(CooldownCoroutine(_attackDelay, OnAttackCooldownFinish));
        }
    }

    private IEnumerator CooldownCoroutine (float cooldownTime, Action callBack)
    {
        yield return new WaitForSeconds(cooldownTime);
        callBack?.Invoke();
    }


    private void OnAttackCooldownFinish()
    {
        _canAttack = true;
        _spriteRenderer.color = new Color(1, 1, 1, _spriteRenderer.color.r);
    }


    public void OnCapturePerformed(InputAction.CallbackContext ctx)
    {
        if (_movementState != MovementState.JUMP)
        {
            _movementState = MovementState.CAPTURE;
        }
    }

    private void OnLanded ()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _movementState = MovementState.DRIVE;
    }


    void FixedUpdate()
    {
        Vector3 inputVelocity = _movementValue * _speed;

        switch (_movementState)
        {
            case MovementState.DRIVE:
                _rigidbody.MovePosition(transform.position + inputVelocity * Time.fixedDeltaTime);
                break;

            case MovementState.JUMP:
                _rigidbody.velocity += new Vector2(_movementValue.x * _airControl, -9.81f * _gravityFactor) * Time.fixedDeltaTime;

                if (transform.position.y < _beforeJumpHeigth)
                {
                    OnLanded();
                }
                break;

            case MovementState.CAPTURE:
                _rigidbody.MovePosition(transform.position + inputVelocity * _captureSpeedFactor * Time.fixedDeltaTime);
                break;

            default:
                break;
        }
    }
}
