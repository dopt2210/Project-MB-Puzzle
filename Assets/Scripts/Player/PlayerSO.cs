using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public LayerMask GroundLayer;

    public GameObject playerPrefab;

    public float moveSpeed = 15f;

    public float Acceleration = 16f;
    public float Deceleration = 13f;


    public float maxSpeed;
    public float rotationSpeed;
    private void OnValidate()
    {
        maxSpeed = (moveSpeed * 0.3f) / (0 + 1);
        rotationSpeed = 3 + (moveSpeed * 0.1f);
    }
}
