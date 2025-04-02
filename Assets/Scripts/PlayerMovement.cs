using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float maxSpeed, rotationSpeed;
    public Transform cameraTransform;

    public Vector2 movementInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        maxSpeed = (moveSpeed * 0.3f) / (rb.linearDamping + 1);
        rotationSpeed = 3 + (moveSpeed * 0.1f);
    }
    void Update()
    {
        // Lấy input từ người chơi
        movementInput = new Vector2(InputManager.Instance.Move.x, InputManager.Instance.Move.y);
    }
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            // Xoay hướng di chuyển theo góc camera
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * moveDir.z + cameraRight * moveDir.x).normalized;

            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
            }
            // Xoay nhân vật theo hướng di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
