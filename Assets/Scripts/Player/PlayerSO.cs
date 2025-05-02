using UnityEngine;

[CreateAssetMenu(fileName = "playerSO", menuName = "Scriptable Objects/playerSO")]
public class PlayerSO : ScriptableObject
{
    public LayerMask GroundLayer;

    public GameObject playerPrefab;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float runSpeed = 15f;
    public float crouchSpeed = 5f;
    [Header("Jump")]
    public float JumpHeight = 2f;
    public int JumpCount = 0;
    public float JumpCutOffMultipiler = 0.5f;
    public float JumpApexTime = 1.2f;
    public float JumpForce;

    public float Acceleration = 5f;
    public float Deceleration = 3f;

    public float rotationSpeed;
    public float maxSpeed;

    [Header("Gravity")]
    [HideInInspector] public float DefaultGravityScale;
    [HideInInspector] public float GravityStrength;
    public float JumpFallGravity = 1.9f;
    public float gravity = -9.81f;

    [Header("Check")]
    [Tooltip("Maximum distance from the ground.")]
    public float distanceThreshold = .15f;
    public float RaycastDistance;
    public float OriginOffset = .001f;

    private void OnValidate()
    {
        maxSpeed = (moveSpeed * 0.3f) / (0 + 1);
        rotationSpeed = 3 + (moveSpeed * 0.1f);
        RaycastDistance = distanceThreshold + OriginOffset;

        GravityStrength = -(2 * JumpHeight) / (JumpApexTime * JumpApexTime);

        DefaultGravityScale = GravityStrength / gravity;

        JumpForce = Mathf.Abs(GravityStrength) * JumpApexTime;
    }
}
