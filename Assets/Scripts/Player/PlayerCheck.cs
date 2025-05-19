using UnityEngine;

[ExecuteInEditMode]
public class PlayerCheck : MonoBehaviour
{
    [SerializeField] private PlayerSO _playerSO;
    [Tooltip("Whether this transform is grounded now.")]
    public bool isGrounded = true;
    /// <summary>
    /// Called when the ground is touched again.
    /// </summary>
    public event System.Action Grounded;
    
    Vector3 RaycastOrigin => transform.position + Vector3.up * _playerSO.OriginOffset;
    private void Reset()
    {
        _playerSO = Resources.Load<PlayerSO>("Scriptable/PlayerSO");
    }

    void LateUpdate()
    {
        // Check if we are grounded now.
        bool isGroundedNow = Physics.Raycast(RaycastOrigin, Vector3.down, _playerSO.distanceThreshold * 2);

        // Call event if we were in the air and we are now touching the ground.
        if (isGroundedNow && !isGrounded)
        {
            Grounded?.Invoke();
        }

        // Update isGrounded.
        isGrounded = isGroundedNow;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a line in the Editor to show whether we are touching the ground.
        Debug.DrawLine(RaycastOrigin, RaycastOrigin + Vector3.down * _playerSO.RaycastDistance, isGrounded ? Color.white : Color.red);
    }
}
