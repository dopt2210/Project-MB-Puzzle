using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    private static MazeGenerator instance;
    public static MazeGenerator Instance { get { return instance; } }
    public bool isDoneCreatOne { get; private set; } = false;
    public Vector3 cellSize { get; private set; }
    
    public static Cell[,,] grid;
    private DynamicAxes? _dynamicAxes;

    [SerializeField] private MazeSO _mazeSO;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        
    }
    private void OnEnable()
    {
        _mazeSO.OnDataChanged += RecreateGrid;
    }

    private void OnDisable()
    {
        _mazeSO.OnDataChanged -= RecreateGrid;
    }
    public void RecreateGrid()
    {
        Debug.Log("reset required");
    }

    public void CreateGrid()
    {
        isDoneCreatOne = false;
        grid = new Cell[_mazeSO.Width, _mazeSO.Height, _mazeSO.Depth];
        cellSize = _mazeSO.cellPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size;

        for (int x = 0; x < _mazeSO.Width; x++)
        {
            for (int y = 0; y < _mazeSO.Height; y++)
            {
                for (int z = 0; z < _mazeSO.Depth; z++)
                {
                    Vector3 position = new Vector3(x , y, z) * cellSize.x;
                    GameObject cellObj = Instantiate(_mazeSO.cellPrefab, position, Quaternion.identity, transform);
                    cellObj.name = $"Cell ({x},{y},{z})";

                    grid[x, y, z] = cellObj.GetComponent<Cell>();
                    grid[x, y, z].x = x;
                    grid[x, y, z].y = y;
                    grid[x, y, z].z = z;
                }
            }
        }

        _dynamicAxes = MazeTools.IdentifyDynamicAxes(_mazeSO.Width, _mazeSO.Height, _mazeSO.Depth);
        if (_dynamicAxes.HasValue) Debug.Log($"Co truc ({_dynamicAxes.Value.primary}, {_dynamicAxes.Value.secondary})");
        else Debug.Log("Khong hop le");
    }

    public void ResetGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        CreateGrid();
        UIInformation.Instance.ResetTime(0);
    }

    public void CreateExitPaths()
    {
        Vector3Int fixedAxis = MazeTools.GetFixedAxis(_mazeSO.Width, _mazeSO.Height, _mazeSO.Depth);
        
        Vector3Int entrance = Vector3Int.zero;
        Vector3Int exit = new Vector3Int(_mazeSO.Width - 1, _mazeSO.Height - 1, _mazeSO.Depth - 1);

        Vector3Int entranceDirection = MazeTools.CreateDirection(-1, 0, _dynamicAxes.Value); // Mở theo hướng lùi
        Vector3Int exitDirection = MazeTools.CreateDirection(1, 0, _dynamicAxes.Value); // Mở theo hướng tiến

        grid[entrance.x, entrance.y, entrance.z].RemoveWall(entranceDirection);

        grid[exit.x, exit.y, exit.z].RemoveWall(exitDirection);

        foreach (var cell in grid)
        {
            cell.RemoveWall(fixedAxis);
        }
        isDoneCreatOne = true;
    }

    public DynamicAxes? GetDynamicAxes()
    {
        return _dynamicAxes;
    }
}

