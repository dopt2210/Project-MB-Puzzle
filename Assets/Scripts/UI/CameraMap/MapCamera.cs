using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }
    public void ResizeMap(MazeSO mazeSO)
    {
        float scale = mazeSO.CellMap.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
        Vector3 pos = _camera.transform.position;
        float x = (mazeSO.Width - 1) * scale / 2f;
        float z = (mazeSO.Depth - 1) * scale / 2f;
        float newSize = Mathf.Max(x, z);
        Vector3 mazeCenter = new Vector3(x, pos.y, z);
        _camera.transform.position = mazeCenter;
        _camera.orthographicSize = newSize + scale;
    }
}
