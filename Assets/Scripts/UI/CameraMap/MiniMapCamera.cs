using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    public Vector3 offset = new Vector3(0, 100, 0);
    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }
    public void FollowCamera(GameObject target)
    {
        _camera.transform.position = target.transform.position + offset;

    }

}
