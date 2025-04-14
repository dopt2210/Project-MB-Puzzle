using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private MazeSO _mazeSO;
    private void Start()
    {
        if (_camera != null && _camera.orthographic)
        {
            ResizeMap();
        }
    }
    void ResizeMap()
    {
        Vector3 pos = _camera.transform.localPosition;
        float delta = _mazeSO.cellPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
        float x = 5 * delta - 640;
        float z = 5 * delta;
        float newSize = 7.5f * delta;
        _camera.transform.localPosition = new Vector3(x, pos.y, z);
        _camera.orthographicSize = newSize;
    }
}
