using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _followOffset = new Vector3(0, 100, 0);
    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }
    public void FollowCamera(Transform target)
    {
        _camera.transform.position = target.position + _followOffset;
    }

}
