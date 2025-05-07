using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    [SerializeField] private MiniMapCamera miniMapCamera;
    [SerializeField] private FogOfWarMask fog;

    #region Action callback
    public static event System.Action OnLevelUpgraded;
    public static event System.Action OnLevelReset;
    public static event System.Action<Cell> OnPlayerCellChanged;
    #endregion

    #region Const data
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private MazeSO mazeSO;
    public GameObject player { get; private set; }
    private CharacterController controller;
    public Vector3 playerSpawnPoint {  get; set; }
    #endregion

    #region Level des
    private MazeAlgorithm[] mazeAlgorithms;
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
    #endregion

    #region For Ui
    private float cellSize;
    private int _currentLevelIndex = 0;
    public int CurrentLevel => _currentLevelIndex + 1;
    public string CurrentAlgorithmName => _currentLevelIndex < mazeAlgorithms.Length
    ? mazeAlgorithms[_currentLevelIndex].Name : "Unknown";
    #endregion

    private void Reset()
    {
        playerSO = Resources.Load<PlayerSO>("Scriptable/playerSO");
        mazeSO = Resources.Load<MazeSO>("Scriptable/MazeSO");
        miniMapCamera = GetComponentInChildren<MiniMapCamera>();
        fog = GetComponent<FogOfWarMask>();
    }
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        MazeGenerator.Instance.CreateGrid();
        CreateEvent();
        CreateLevel();
    }
    private void Start()
    {
        CreateSpawnPoint(MazeGenerator.grid[0, 0, 0]);
        CreatePlayer();
        cellSize = mazeSO.cellPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
        fog.SetUpSize(mazeSO, cellSize);
    }
    private void Update()
    {
        if (player == null) return;

        CurrentCell = MazeTools.GetCellFromGameObject(player, MazeGenerator.grid, mazeSO.boxSize, cellSize);
        if (!CurrentCell.flagVisited)
        {
            CurrentCell.flagVisited = true;
            CurrentCell.HighlightForMiniMap(Color.red);
        }
        
    }
    private void LateUpdate()
    {
        miniMapCamera.FollowCamera(player);
        //List<Cell> revealCell = MazeTools.GetNeighborsInSquare(CurrentCell, MazeGenerator.grid, mazeSO.boxSize);
        //fog.RevealCells(revealCell);
        fog.Reveal(player.transform.position);
    }
    #region Maze Handler
    public void PlayGame()
    {

    }
    public void ResetMaze()
    {
        MazeGenerator.Instance.ResetGrid();
        fog.ResetFog();
        controller.enabled = false;

        player.transform.position = playerSpawnPoint;

        controller.enabled = true;

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
        controller = player.GetComponent<CharacterController>();
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
    #endregion

    #region UI Handler
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
    #endregion


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
