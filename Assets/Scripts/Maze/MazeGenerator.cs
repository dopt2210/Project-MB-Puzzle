using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance {  get; private set; }
    public static Cell[,,] MazeGrid { get; private set; }

    private bool isDoneCreatOne = false;
    private float _cellSize;
    private DynamicAxes? _dynamicAxes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #region Grid
    public void ResetGrid(MazeSO mazeSO)
    {
        if (!isDoneCreatOne) return;
        isDoneCreatOne = false;

        for (int x = 0; x < mazeSO.Width; x++)
        {
            for (int y = 0; y < mazeSO.Height; y++)
            {
                for (int z = 0; z < mazeSO.Depth; z++)
                {
                    MazeGrid[x, y, z].ResetState();
                }
            }
        }
    }

    public void ResetGrid()
    {
        if (!isDoneCreatOne) return;
        isDoneCreatOne = false;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateGrid(MazeSO mazeSO)
    {
        MazeGrid = new Cell[mazeSO.Width, mazeSO.Height, mazeSO.Depth];
        _cellSize = mazeSO.CellMap.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;

        isDoneCreatOne = false;

        for (int x = 0; x < mazeSO.Width; x++)
        {
            for (int y = 0; y < mazeSO.Height; y++)
            {
                for (int z = 0; z < mazeSO.Depth; z++)
                {
                    Vector3 position = new Vector3(x , y, z) * _cellSize;
                    GameObject cellObj = Instantiate(mazeSO.CellMap, position, Quaternion.identity, transform);
                    cellObj.name = $"Cell ({x},{y},{z})";

                    MazeGrid[x, y, z] = cellObj.GetComponent<Cell>();
                    MazeGrid[x, y, z].x = x;
                    MazeGrid[x, y, z].y = y;
                    MazeGrid[x, y, z].z = z;
                }
            }
        }

        _dynamicAxes = MazeTools.IdentifyDynamicAxes(mazeSO.Width, mazeSO.Height, mazeSO.Depth);
    }

    public void CreateExitPaths(int w, int h, int d)
    {
        Vector3Int fixedAxis = MazeTools.GetFixedAxis(w, h, d);

        //Vector3Int entrance = Vector3Int.zero;
        //Vector3Int exit = new Vector3Int(mazeSO.Width - 1, mazeSO.Height - 1, mazeSO.Depth - 1);

        //Vector3Int entranceDirection = MazeTools.CreateDirection(-1, 0, _dynamicAxes.Value); // Mở theo hướng lùi
        //Vector3Int exitDirection = MazeTools.CreateDirection(1, 0, _dynamicAxes.Value); // Mở theo hướng tiến

        //boardLayout[entrance.x, entrance.y, entrance.z].RemoveWall(entranceDirection);

        //boardLayout[exit.x, exit.y, exit.z].RemoveWall(exitDirection);

        foreach (var cell in MazeGrid)
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

