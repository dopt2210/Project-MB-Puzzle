using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public LayerMask GroundLayer;

    public GameObject playerPrefab;

    [Header("Movement")]
    public float moveSpeed = 15f;
    public float runSpeed = 25f;
    public float crouchSpeed = 9f;
    public float Acceleration = 5f;
    public float Deceleration = 3f;

    public float rotationSpeed;
    public float maxSpeed;

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
    }
}
