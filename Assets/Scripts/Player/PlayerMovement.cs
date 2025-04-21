using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]private PlayerSO _playerSO;
    public bool IsRunning { get; private set; }
    public bool canRun = true;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private Vector2 _movementInput;
    private Rigidbody _rb;
    private void Reset()
    {
        _playerSO = Resources.Load<PlayerSO>("Scriptable/PlayerSO");
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _movementInput = new Vector2(InputManager.Instance.Move.x, InputManager.Instance.Move.y);
    }
    void FixedUpdate()
    {
        MoveByVelocity();
    }
    void MoveByVelocity()
    {
        IsRunning = canRun && InputManager.Instance.kRun;

        float targetMovingSpeed = IsRunning ? _playerSO.runSpeed : _playerSO.moveSpeed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = _movementInput.normalized * targetMovingSpeed;

        _rb.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.y);
    }

    //void Move()
    //{
    //    Vector3 moveDir = new Vector3(_movementInput.x, 0, _movementInput.y).normalized;

    //    if (moveDir.magnitude > 0.1f)
    //    {
    //        // Xoay hướng di chuyển theo góc cm
    //        Vector3 cameraForward = cameraTransform.forward;
    //        cameraForward.y = 0;
    //        cameraForward.Normalize();

    //        Vector3 cameraRight = cameraTransform.right;
    //        cameraRight.y = 0;
    //        cameraRight.Normalize();

    //        Vector3 moveDirection = (cameraForward * moveDir.z + cameraRight * moveDir.x).normalized;

    //        if (_rb.linearVelocity.magnitude < _playerSO.maxSpeed)
    //        {
    //            _rb.AddForce(moveDirection * _playerSO.moveSpeed, ForceMode.Acceleration);
    //        }
    //        // Xoay nhân vật theo hướng di chuyển
    //        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _playerSO.rotationSpeed * Time.deltaTime);
    //    }
    //}
}
