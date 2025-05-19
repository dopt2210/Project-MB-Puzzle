using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;

    private void Reset()
    {
        character = GetComponentInParent<PlayerMovement>().transform;
    }
    private void Update()
    {
        CameraLookSetUp();   
    }
    private void CameraLookSetUp()
    {
        if (MouseLock.Instance != null && MouseLock.Instance.IsMouseFree) return;
        Vector2 mouseDelta = InputManager.Instance.Look;

        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
