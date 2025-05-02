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
    private bool _running;
    private void Reset()
    {
        _playerSO = Resources.Load<PlayerSO>("Scriptable/playerSO");
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _running = InputManager.Instance.Movement.IsRunning;
        _movementInput = new Vector2(InputManager.Instance.Movement.Move.x, InputManager.Instance.Movement.Move.y);
    }
    void FixedUpdate()
    {
        MoveByVelocity();
    }
    void MoveByVelocity()
    {
        IsRunning = canRun && _running;

        float targetMovingSpeed = IsRunning ? _playerSO.runSpeed : _playerSO.moveSpeed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = _movementInput.normalized * targetMovingSpeed;

        _rb.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.y);
    }
}
