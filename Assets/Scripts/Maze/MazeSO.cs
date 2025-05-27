using UnityEngine;

[CreateAssetMenu(fileName = "MazeSO", menuName = "Scriptable Objects/MazeSO")]
public class MazeSO : ScriptableObject
{
    #region Maze Size
    [Header("Maze")]
    [Tooltip("Choose size for maze")]
    [SerializeField] private int width = 15;
    [HideInInspector] private int height = 1;
    [SerializeField] private int depth = 15;
    [HideInInspector] public Vector3Int BoxSize;
    public int Width
    {
        get => width;
        set
        {
            if (width != value)
            {
                width = value;
            }
        }
    }

    public int Height
    {
        get => height;
        set
        {
            if (height != value)
            {
                height = value;
            }
        }
    }

    public int Depth
    {
        get => depth;
        set
        {
            if (depth != value)
            {
                depth = value;
            }
        }
    }
    #endregion

    [Header("Cell Prefab")]
    [Tooltip("Automatically fetch the required prefab when resetting the component")]
    public GameObject CellMap;
    public GameObject CellPuzzle;

    [Header("Goal Prefab")]
    [Tooltip("Automatically fetch the required prefab when resetting the component")]
    public GameObject GoalPrefab;

    [Header("Level Mode")]
    [Tooltip("Select algorithm for level")]
    [SerializeField] public MazeAlgorithm MazeAlgorithm;

    #region Function
    public float GetSizeScale 
        => CellMap.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
    public void Generate() => MazeAlgorithm.Generate(this, GetSizeScale);
    public string CurrentAlgorithmName => MazeAlgorithm.algorithmType.ToString();
    #endregion

    private void OnValidate()
    {
        BoxSize = new Vector3Int(width, height, depth);
    }
    private void Reset()
    {
        CellMap = Resources.Load<Cell>("Prefab/Maze/CellMap").gameObject;
        //CellPuzzle = Resources.Load<TriggerActionCtrl>("Prefab/Maze/CellPuzzle").gameObject;
        GoalPrefab = Resources.Load<TriggerActionCtrl>("Prefab/Maze/Ending").gameObject;
    }

}
public enum MazeAlgorithmType
{
    DFS,
    BinaryTree,
    Sidewinder,
    AldousBroder,
    HuntandKill,
    RandomPrims,
    RandomKruskal,
    Eller
}
public interface IMazeGenerator
{
    void GenerateMazeInstant();
}
[System.Serializable]
public struct MazeAlgorithm
{
    public MazeAlgorithmType algorithmType;
    public void Generate(MazeSO mazeSO, float cellSize)
    {
        IMazeGenerator generator = CreateGenerator(mazeSO, cellSize);
        generator?.GenerateMazeInstant();
    }
    private IMazeGenerator CreateGenerator(MazeSO mazeSO, float cellSize)
    {
        switch (algorithmType)
        {
            case MazeAlgorithmType.DFS:
                return new DFS(mazeSO, cellSize);
            case MazeAlgorithmType.BinaryTree:
                return new BinaryTree(mazeSO, cellSize);
            case MazeAlgorithmType.Sidewinder:
                return new Sidewinder(mazeSO, cellSize);
            case MazeAlgorithmType.AldousBroder:
                return new AldousBroder(mazeSO, cellSize);
            case MazeAlgorithmType.HuntandKill:
                return new HuntandKill(mazeSO, cellSize);
            case MazeAlgorithmType.RandomPrims:
                return new RandomPrims(mazeSO, cellSize);
            case MazeAlgorithmType.RandomKruskal:
                return new RandomKruskal(mazeSO, cellSize);
            case MazeAlgorithmType.Eller:
                return new Eller(mazeSO, cellSize);
            default:
                Debug.LogWarning("Unknown algorithm puzzleType!");
                return null;
        }
    }
}