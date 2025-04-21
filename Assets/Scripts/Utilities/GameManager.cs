using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    public static event System.Action OnLevelUpgraded;
    public static event System.Action OnLevelReset;
    public static event System.Action<Cell> OnPlayerCellChanged;

    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private MazeSO mazeSO;
    public GameObject player { get; private set; }
    public Vector3 playerSpawnPoint {  get; set; }
    private float cellSize;

    private int _currentLevelIndex = 0;
    public int CurrentLevel => _currentLevelIndex + 1;

    private MazeAlgorithm[] mazeAlgorithms;
    public string CurrentAlgorithmName => _currentLevelIndex < mazeAlgorithms.Length
    ? mazeAlgorithms[_currentLevelIndex].Name : "Unknown";

    private Cell _currentPlayerCell;
    public Cell CurrentCell
    {
        get => _currentPlayerCell;
        private set
        {
            if (_currentPlayerCell != value)
            {
                _currentPlayerCell = value;
                OnPlayerCellChanged?.Invoke(_currentPlayerCell);
            }
        }
    }

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        MazeGenerator.Instance.CreateGrid();
        CreateEvent();
        CreateLevel();
    }
    private void Reset()
    {
        playerSO = Resources.Load<PlayerSO>("Scriptable/PlayerSO");
        mazeSO = Resources.Load<MazeSO>("Scriptable/MazeSO");
    }
    private void Start()
    {
        CreateSpawnPoint(MazeGenerator.grid[0, 0, 0]);
        CreatePlayer();
        cellSize = mazeSO.cellPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
    }
    private void Update()
    {
        if (player == null) return;

        CurrentCell = MazeTools.GetCellFromGameObject(player, MazeGenerator.grid, mazeSO.boxSize, cellSize);
    }
    public void ResetMaze()
    {
        MazeGenerator.Instance.ResetGrid();
        player.transform.position = playerSpawnPoint;
        OnLevelReset?.Invoke();
        CreateLevel();
    }
    public void CreateSpawnPoint(Cell cellPosition)
    {
        playerSpawnPoint = cellPosition.transform.Find("SpawnPoint").position;
    }
    public void CreatePlayer()
    {
        player = Instantiate(playerSO.playerPrefab, playerSpawnPoint, Quaternion.identity, transform);
    }
    public void CreateLevel()
    {
        mazeAlgorithms[_currentLevelIndex].Generate?.Invoke();
    }
    private void CreateEvent()
    {
        mazeAlgorithms = new MazeAlgorithm[]
        {
        new MazeAlgorithm("DFS",            () => new DFS(mazeSO).GenerateMazeInstant()),
        new MazeAlgorithm("Binary Tree",    () => new BinaryTree(mazeSO).GenerateMazeInstant()),
        new MazeAlgorithm("Sidewinder",     () => new Sidewinder(mazeSO).GenerateMazeInstant()),
        new MazeAlgorithm("Aldous-Broder",  () => new AldousBroder(mazeSO).GenerateMazeInstant()),
        new MazeAlgorithm("Hunt and Kill",  () => new HuntandKill(mazeSO).GenerateMazeInstant()),
        new MazeAlgorithm("Prim's",         () => new RandomPrims(mazeSO).GenerateMazeInstant()),
        new MazeAlgorithm("Kruskal's",      () => new RandomKruskal(mazeSO).GenerateMazeInstant()),
        new MazeAlgorithm("Eller's",        () => new Eller(mazeSO).GenerateMazeInstant())
        };
    }
    public void LevelUpgrade()
    {
        if (_currentLevelIndex < mazeSO.mazeLevel - 1)
        {
            _currentLevelIndex++;
        }

        if (_currentLevelIndex >= mazeAlgorithms.Length)
        {
            Debug.Log("You win! All levels completed!");
            return;
        }

        OnLevelUpgraded?.Invoke(); 
        ResetMaze();
    }
}
public struct MazeAlgorithm
{
    public string Name;
    public System.Action Generate;

    public MazeAlgorithm(string name, System.Action generate)
    {
        Name = name;
        Generate = generate;
    }
}
