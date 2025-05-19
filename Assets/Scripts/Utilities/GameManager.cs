using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private MazeSO mazeSO;
    private System.Action[] mazeGenerators;
    [SerializeField] private int currentLevelIndex = 0;

    private Cell currentPlayerCell;
    public GameObject player { get; set; }
    public Vector3 playerSpawnPoint {  get; set; }
    private int maxLevel = 8;
    private float cellSize;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

    }

    private void Start()
    {
        MazeGenerator.Instance.CreateGrid();
        CreateEvent();
        CreateLevel();
        CreateSpawnPoint();
        CreatePlayer();
        cellSize = mazeSO.cellPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;


    }
    private void Update()
    {
        if (player == null) return;

        currentPlayerCell = MazeTools.GetCellFromGameObject(player, MazeGenerator.grid, mazeSO.boxSize, cellSize);
        UIDebug.Instance.UpdateCoord(currentPlayerCell);

    }
    void LoadComponents()
    {

    }
    public void CreateSpawnPoint()
    {
        Cell cell = MazeGenerator.grid[0, 0, 0];
        playerSpawnPoint = cell.transform.Find("SpawnPoint").position;
    }
    public void CreatePlayer()
    {
        Cursor.visible = false;
        player = Instantiate(playerSO.playerPrefab, playerSpawnPoint, Quaternion.identity, transform);
    }
    public void ResetMaze()
    {
        MazeGenerator.Instance.ResetGrid();
        player.transform.position = playerSpawnPoint;
        CreateLevel();
    }
    public void CreateLevel()
    {
        mazeGenerators[currentLevelIndex]?.Invoke();
    }
    public void LevelUpgrade()
    {
        if (currentLevelIndex < maxLevel - 1)
        {
            currentLevelIndex++;
        }
        if (currentLevelIndex >= mazeGenerators.Length)
        {
            Debug.Log("You win! All levels completed!");
            return;
        }
        ResetMaze();
    }
    void CreateEvent()
    {
        mazeGenerators = new System.Action[]
        {
        () => new DFS(mazeSO).GenerateMazeInstant(),
        () => new BinaryTree(mazeSO).GenerateMazeInstant(),
        () => new Sidewinder(mazeSO).GenerateMazeInstant(),
        () => new AldousBroder(mazeSO).GenerateMazeInstant(),
        () => new HuntandKill(mazeSO).GenerateMazeInstant(),
        () => new RandomPrims(mazeSO).GenerateMazeInstant(),
        () => new RandomKruskal(mazeSO).GenerateMazeInstant(),
        () => new Eller(mazeSO).GenerateMazeInstant(),
        };
    }
}
