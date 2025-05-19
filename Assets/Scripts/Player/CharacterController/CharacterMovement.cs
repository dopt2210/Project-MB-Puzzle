using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private Animator animator;
    public Transform cam;

    private CharacterController controller;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isGrounded;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    private Vector2 _movementInput;
    private bool _running;

    int IsJumpingHash;
    int IsRunningHash;

    private void Reset()
    {
        playerSO = Resources.Load<PlayerSO>("Scriptable/playerSO");
        cam = GetComponentInChildren<CinemachineCamera>().transform;
        animator = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        controller = GetComponent<CharacterController>();
        IsRunningHash = Animator.StringToHash("IsRunning");
        IsJumpingHash = Animator.StringToHash("IsJumping");
        if (cam == null)
            cam = Camera.main.transform;
    }

    void Update()
    {
        _running = InputManager.Instance.Movement.IsRunning;
        _movementInput = new Vector2(InputManager.Instance.Movement.Move.x, InputManager.Instance.Movement.Move.y);
        Move();
    }

    void Move()
    {

        if (MouseLock.Instance != null && MouseLock.Instance.IsMouseFree) return;

        isGrounded = controller.isGrounded;
        bool IsJumping = animator.GetBool(IsJumpingHash);
        bool IsRunning = animator.GetBool(IsRunningHash);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 direction = new Vector3(_movementInput.x, 0f, _movementInput.y).normalized;
        float speed = direction.magnitude;
        animator.SetFloat("Speed", speed);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * playerSO.moveSpeed * Time.deltaTime);

        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(playerSO.JumpHeight * -2f * playerSO.gravity);
            animator.SetBool(IsJumpingHash, true);

        }
        else if (isGrounded)
        {
            animator.SetBool(IsJumpingHash, false);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool(IsRunningHash, true);
        }
        else
        {
            animator.SetBool(IsRunningHash, false);
        }

        velocity.y += playerSO.gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}