using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
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
    [SerializeField] private float _repulseAmountAttack;
    [SerializeField] private float _repulseAmountPlayerCollision;
    [SerializeField] private float _repulseAmountObstacleCollision;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _hitDelay;
    [SerializeField] private float _gravityFactor;
    [SerializeField] private float _captureSpeedFactor;
    [SerializeField] private float _stunSpeedFactor;
    [SerializeField] private float _playerCollisionDelay;
    [SerializeField] private float _obstacleCollisionDelay;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _circleCollider;
    [SerializeField] private Collider2D _boxCollider;
    [SerializeField] private ParticleSystem _hitParticleSystem;
    [SerializeField] private Animator _playerAnimator;

    [SerializeField] private int _baseScoreGain;
    [SerializeField] private int _timeBeforeUpgrade;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    [SerializeField] private TextMeshProUGUI _oneTimeScoreText;
    [SerializeField] private ParticleSystem particleCloud;

    [SerializeField] private Transform _shadow;
   
    public TextMeshProUGUI TextMeshProObject { get; set; }
    public SpriteRenderer Loupiote { get; set; }
    public Player OtherPlayer { get; set; }
    public Animator PlayerAnimator => _playerAnimator;

    private float _beforeJumpHeigth;
    private bool _canAttack = true;
    private bool _otherInRange;
    private bool _canBeHit = true;
    private bool _isInZone = false;

    private float _oneTimeScore;

    private float _score;
    private float _baseShadowHeigth;

    [SerializeField] private PlayerInput _playerInput;

    private enum MovementState
    {
        DRIVE,
        JUMP,
        CAPTURE,
        STUNNED
    }

    void Start()
    {
        _playerInput.actions["Move"].performed += OnMovePerformed;
        _playerInput.actions["Move"].canceled += OnMoveCanceled;
        _playerInput.actions["Jump"].started += OnJumpStarted;
        _playerInput.actions["Capture"].performed += OnCapturePerformed;
        _playerInput.actions["Capture"].canceled += OnCaptureCanceled;
        _playerInput.actions["Attack"].performed += OnAttackPerformed;

        particleCloud.Play();
        _baseShadowHeigth = _shadow.localPosition.y;
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
       
        if (collider.tag != tag && collider.tag != "obstacle")
        {
            _otherInRange = true;
        }
        if (collider.tag == "obstacle" && _movementState != MovementState.JUMP && _canBeHit == true)
        {
            _canBeHit = false;
            _canAttack = false;
            _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            _movementState = MovementState.STUNNED;
            StartCoroutine(CooldownCoroutine(_obstacleCollisionDelay, OnObstacleCooldownFinish));

        }
        if (collider.tag == "Zone")
        {
            _isInZone = true;
            _oneTimeScore = 0;
            if (_movementState == MovementState.CAPTURE)
            {
                _oneTimeScoreText.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag != tag)
        {
            _otherInRange = false;
        }
        if (collider.tag == "Zone")
        {
            _isInZone = false;
            _oneTimeScoreText.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag != tag && _movementState != MovementState.JUMP && _canBeHit == true) 
        {
            _canBeHit = false;
            _canAttack = false;
            Vector2 force = transform.position - collision.transform.position;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.AddForce(force.normalized * _repulseAmountPlayerCollision, ForceMode2D.Impulse);
            _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            StartCoroutine(CooldownCoroutine(_playerCollisionDelay, OnHitCooldownFinish));

            _hitParticleSystem.Play();
            _hitParticleSystem.transform.position = collision.GetContact(0).point;

            _impulseSource.GenerateImpulse(0.15f);
        }
    }


    public void HitPlayer (Vector3 otherPosition)
    {
        if (_canBeHit && _movementState != MovementState.JUMP)
        {
            _canBeHit = false;
            _canAttack = false;

            Color baseColor = _spriteRenderer.color;
            baseColor.a = 0.5f;
            _spriteRenderer.color = baseColor;

            Vector2 force = transform.position - otherPosition;
            _rigidbody.velocity += force.normalized * _repulseAmountAttack;
            StartCoroutine(CooldownCoroutine(_hitDelay, OnHitCooldownFinish));

            _impulseSource.GenerateImpulse(0.25f);
        }
    }

    private void OnObstacleCooldownFinish()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 1f);
        _canBeHit = true;
        _canAttack = true;
        _movementState = MovementState.DRIVE;
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
        if ( _movementState == MovementState.CAPTURE)
        {
            _movementState = MovementState.DRIVE;
            _oneTimeScoreText.gameObject.SetActive(false);
            _playerAnimator.SetBool("isRecording", false);
        }
    }

    public void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        if (_movementState == MovementState.DRIVE)
        {
            _movementState = MovementState.JUMP;
            _beforeJumpHeigth = transform.position.y;
            _rigidbody.velocity += new Vector2(0, _jumpAmount);
            _circleCollider.enabled = false;
            _boxCollider.enabled = false;
            _playerAnimator.SetBool("isJumping", true);
            _shadow.parent = null;
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
            _playerAnimator.SetTrigger("attack");
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
        if (_movementState == MovementState.DRIVE)
        {
            _movementState = MovementState.CAPTURE;
            _playerAnimator.SetBool("isRecording", true);

            if (_isInZone)
            {
                _oneTimeScoreText.gameObject.SetActive(true);
            }
        }
    }

    private void OnLanded()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _movementState = MovementState.DRIVE;
        _circleCollider.enabled = true;
        _boxCollider.enabled = true;

        _playerAnimator.SetBool("isJumping", false);

        _shadow.SetParent(transform);
        _shadow.localPosition = Vector3.up * _baseShadowHeigth;
    }

    void FixedUpdate()
    {
        Vector3 inputVelocity = _movementValue * _speed;

        switch (_movementState)
        {
            case MovementState.DRIVE:
                _rigidbody.velocity += (Vector2) (inputVelocity * Time.fixedDeltaTime);
                break;
                
            case MovementState.JUMP:
                _rigidbody.velocity += new Vector2(_movementValue.x * _airControl, -9.81f * _gravityFactor) * Time.fixedDeltaTime;
                _shadow.position = new Vector3(transform.position.x, _shadow.position.y, 0);

                if (transform.position.y < _beforeJumpHeigth && _rigidbody.velocity.y < 0)
                {
                    OnLanded();
                }
                break;

            case MovementState.CAPTURE:
                _rigidbody.velocity += (Vector2)(inputVelocity * _captureSpeedFactor * Time.fixedDeltaTime);
                if (_isInZone && _canBeHit)
                {
                    _oneTimeScore += Time.deltaTime * _baseScoreGain;
                    _oneTimeScoreText.text = _oneTimeScore.ToString("F2");
                    _oneTimeScoreText.transform.DOShakeScale(Time.fixedDeltaTime, 0.2f, 2);

                    _score += Time.deltaTime * _baseScoreGain;
                    TextMeshProObject.text = _score.ToString("F2");
                    TextMeshProObject.transform.DOShakeScale(Time.fixedDeltaTime,0.1f, 1);

                    Loupiote.color = new Color(0.9f, 0, 0, Mathf.Abs(Mathf.Sin(Time.time * 2)));
                }
                break;

            case MovementState.STUNNED:
                _rigidbody.velocity += (Vector2)(inputVelocity * _stunSpeedFactor * Time.fixedDeltaTime);
                break;

            default:
                break;
        }
    }
}
