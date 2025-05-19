using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamagable
{
    public bool IsControlActivate { get; set; } = true;

    private PlayerStatus _status;
    private PlayerMovement _movement;
    private Animator _animator;
    private Image _aimImage;
    private InputAction _aimInputAction;
    private InputAction _shootInputAction;

    [SerializeField] private CinemachineVirtualCamera _aimCamera;
    [SerializeField] private Gun _gun;
    [SerializeField] private Animator _aimAnimator;
    [SerializeField] private HpGuageUI _hpUI;

    [SerializeField] private KeyCode _aimKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode _shootKey = KeyCode.Mouse0;

    private void Awake() => Init();
    private void OnEnable() => SubscribeEvents();
    private void Update() => HandlePlayerControl();
    private void OnDisable() => UnsubscribeEvents();


    private void Init()
    {
        _status = GetComponent<PlayerStatus>();
        _movement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
        _aimImage = _aimAnimator.GetComponent<Image>();
        _aimInputAction = GetComponent<PlayerInput>().actions["Aim"];
        _shootInputAction = GetComponent<PlayerInput>().actions["Shoot"];

        _hpUI.SetImageFillAmount(1);
        _status.CurrentHp.Value = _status.MaxHP;
    }

    private void HandlePlayerControl()
    {
        if (!IsControlActivate) return;

        HandleMovement();
        // HandleAiming();
        HandleShooting();
    }

    private void HandleShooting()
    {
        // _shootInputAction.WasPressedThisFrame() => 이번 프레임에 눌렸는가? (GetKeyDown)
        // _shootInputAction.WasReleasedThisFrame() => 이번 프레임에 떼어졌는가? (GetKeyUp)
        // _shootInputAction.IsPressed() => 지금 눌려있는가? (GetKey)

        if (_status.IsAiming.Value && _shootInputAction.IsPressed())
        {
            _status.IsAttacking.Value = _gun.Shoot();
        }
        else
        {
            _status.IsAttacking.Value = false;
        }
    }

    private void HandleMovement()
    {
        Vector3 camRotateDir = _movement.SetAimRotation();

        float moveSpeed;
        if (_status.IsAiming.Value) moveSpeed = _status.WalkSpeed;
        else moveSpeed = _status.RunSpeed;

        Vector3 moveDir = _movement.SetMove(moveSpeed);
        _status.IsMoving.Value = (moveDir != Vector3.zero);

        Vector3 avatarDir;
        if (_status.IsAiming.Value) avatarDir = camRotateDir;
        else avatarDir = moveDir;

        _movement.SetAvatarRotation(avatarDir);

        // Aim 상태일 때만.
        if (_status.IsAiming.Value)
        {
            // Vector3 input = _movement.GetInputDirection();
            // _animator.SetFloat("X", input.x);
            // _animator.SetFloat("Z", input.z);

            _animator.SetFloat("X", _movement.InputDirection.x);
            _animator.SetFloat("Z", _movement.InputDirection.y);
        }
    }

    private void HandleAiming(InputAction.CallbackContext ctx)
    {
        // _status.IsAiming.Value = Input.GetKey(_aimKey);
        _status.IsAiming.Value = ctx.started;

        // ctx.started => 키 입력이 시작됐는지 판별
        // ctx.performed => 키 입력이 진행중인지 판별
        // ctx.canceled => 키 입력이 취소됐는지(떼어졌는지) 판별
    }

    public void TakeDamage(int value)
    {
        _status.CurrentHp.Value -= value;

        if (_status.CurrentHp.Value <= 0) Dead();
    }

    public void RecoveryHp(int value)
    {
        int hp = _status.CurrentHp.Value + value;

        _status.CurrentHp.Value = Mathf.Clamp(
            hp,
            0,
            _status.MaxHP
        );
    }

    public void Dead()
    {
        Debug.Log("플레이어 사망 처리");
    }

    public void SubscribeEvents()
    {
        _status.CurrentHp.Subscribe(SetHpUIGuage);

        _status.IsMoving.Subscribe(SetMoveAnimation);

        _status.IsAiming.Subscribe(_aimCamera.gameObject.SetActive);
        _status.IsAiming.Subscribe(SetAimAnimation);

        _status.IsAttacking.Subscribe(SetAttackAnimation);

        // inputs----
        _shootInputAction.Enable();
        _aimInputAction.Enable();
        _aimInputAction.started += HandleAiming;
        _aimInputAction.canceled += HandleAiming;
    }

    public void UnsubscribeEvents()
    {
        _status.CurrentHp.Unsubscribe(SetHpUIGuage);

        _status.IsMoving.Unsubscribe(SetMoveAnimation);

        _status.IsAiming.Unsubscribe(_aimCamera.gameObject.SetActive);
        _status.IsAiming.Unsubscribe(SetAimAnimation);

        _status.IsAttacking.Unsubscribe(SetAttackAnimation);

        // inputs----
        _aimInputAction.Disable();
        _shootInputAction.Disable();
        _aimInputAction.started -= HandleAiming;
        _aimInputAction.canceled -= HandleAiming;
    }

    private void SetAimAnimation(bool value)
    {
        if (!_aimImage.enabled) _aimImage.enabled = true;
        _animator.SetBool("IsAim", value);
        _aimAnimator.SetBool("IsAim", value);
    }
    private void SetMoveAnimation(bool value) => _animator.SetBool("IsMove", value);
    private void SetAttackAnimation(bool value) => _animator.SetBool("IsAttack", value);
    private void SetHpUIGuage(int currentHp)
    {
        float hp = currentHp / (float)_status.MaxHP;
        _hpUI.SetImageFillAmount(hp);
    }
}














