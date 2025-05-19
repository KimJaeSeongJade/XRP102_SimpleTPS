using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _avatar;
    [SerializeField] private Transform _aim;

    private Rigidbody _rigidbody;
    private PlayerStatus _playerStatus;

    [Header("Mouse Config")]
    [SerializeField][Range(-90, 0)] private float _minPitch;
    [SerializeField][Range(0, 90)] private float _maxPitch;
    [SerializeField][Range(0, 5)] private float _mouseSensitivity = 1;

    private Vector2 _currentRotation;
    public Vector2 InputDirection { get; private set; }
    public Vector2 MouseDirection { get; private set; }

    private void Awake() => Init();

    private void Init()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerStatus = GetComponent<PlayerStatus>();
    }

    public Vector3 SetMove(float moveSpeed)
    {
        Vector3 moveDirection = GetMoveDirection();

        Vector3 velocity = _rigidbody.velocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;

        _rigidbody.velocity = velocity;

        return moveDirection;
    }

    public Vector3 SetAimRotation()
    {
        // Vector2 inputDir = GetMouseDirection();

        // // X방향 회전은 각도 제한 없음.
        _currentRotation.x += MouseDirection.x;

        // y���� ��쿣 ���� ������ �ɾ�� ��.
        _currentRotation.y = Mathf.Clamp(
            _currentRotation.y + MouseDirection.y,
            _minPitch,
            _maxPitch
            );

        // ĳ���� ������Ʈ�� ��쿡�� Y�� ȸ���� �ݿ�
        transform.rotation = Quaternion.Euler(0, _currentRotation.x, 0);

        // ������ ��� ���� ȸ�� �ݿ�
        Vector3 currentEuler = _aim.localEulerAngles;
        _aim.localEulerAngles = new Vector3(_currentRotation.y, currentEuler.y, currentEuler.z);

        // ȸ�� ���� ���� ��ȯ
        Vector3 rotateDirVector = transform.forward;
        rotateDirVector.y = 0;
        return rotateDirVector.normalized;
    }

    public void SetAvatarRotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        _avatar.rotation = Quaternion.Lerp(
            _avatar.rotation,
            targetRotation,
            _playerStatus.RotateSpeed * Time.deltaTime
            );
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 direction =
           (transform.right * InputDirection.x) +
           (transform.forward * InputDirection.y);

        return direction.normalized;
    }

    public void OnMove(InputValue value)
    {
        InputDirection = value.Get<Vector2>();
    }

    public void OnRotate(InputValue value)
    {
        Vector2 mouseDir = value.Get<Vector2>();
        mouseDir.y *= -1;
        MouseDirection = mouseDir * _mouseSensitivity;
    }

    // public Vector3 GetInputDirection()
    // {
    //     float x = Input.GetAxisRaw("Horizontal");
    //     float z = Input.GetAxisRaw("Vertical");

    //     return new Vector3(x, 0, z);
    // }
    
    // private Vector2 GetMouseDirection()
    // {
    //     float mouseX = Input.GetAxis("Mouse X");
    //     float mouseY = -Input.GetAxis("Mouse Y");

    //     return new Vector2(mouseX, mouseY);
    // }
}


















