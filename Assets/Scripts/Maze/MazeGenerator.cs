using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    private static MazeGenerator instance;
    public static MazeGenerator Instance { get { return instance; } }
    public bool isDoneCreatOne { get; private set; } = false;
    public Vector3 cellSize { get; private set; }
    
    public static Cell[,,] grid;
    private DynamicAxes? _dynamicAxes;

    [SerializeField] private MazeSO mazeSO;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        
    }
    private void OnEnable()
    {
        mazeSO.OnDataChanged += RecreateGrid;
    }

    private void OnDisable()
    {
        mazeSO.OnDataChanged -= RecreateGrid;
    }
    private void Reset()
    {
        mazeSO = Resources.Load<MazeSO>("Scriptable/MazeSO");
    }
    #region Grid
    public void RecreateGrid()
    {
        Debug.Log("reset required");
    }

    public void CreateGrid()
    {
        isDoneCreatOne = false;
        grid = new Cell[mazeSO.Width, mazeSO.Height, mazeSO.Depth];
        cellSize = mazeSO.cellPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size;

        for (int x = 0; x < mazeSO.Width; x++)
        {
            for (int y = 0; y < mazeSO.Height; y++)
            {
                for (int z = 0; z < mazeSO.Depth; z++)
                {
                    Vector3 position = new Vector3(x , y, z) * cellSize.x;
                    GameObject cellObj = Instantiate(mazeSO.cellPrefab, position, Quaternion.identity, transform);
                    cellObj.name = $"Cell ({x},{y},{z})";

                    grid[x, y, z] = cellObj.GetComponent<Cell>();
                    grid[x, y, z].x = x;
                    grid[x, y, z].y = y;
                    grid[x, y, z].z = z;
                }
            }
        }

        _dynamicAxes = MazeTools.IdentifyDynamicAxes(mazeSO.Width, mazeSO.Height, mazeSO.Depth);
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
    }

    public void CreateExitPaths()
    {
        Vector3Int fixedAxis = MazeTools.GetFixedAxis(mazeSO.Width, mazeSO.Height, mazeSO.Depth);
        
        Vector3Int entrance = Vector3Int.zero;
        Vector3Int exit = new Vector3Int(mazeSO.Width - 1, mazeSO.Height - 1, mazeSO.Depth - 1);

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
    #endregion

    public DynamicAxes? GetDynamicAxes()
    {
        return _dynamicAxes;
    }
}

