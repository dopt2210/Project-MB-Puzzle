using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    #region Action callback
    public static event System.Action OnLevelUpgraded;
    public static event System.Action OnLevelReset;
    public static event System.Action<Cell> OnPlayerCellChanged;
    #endregion

    [Header("Map Cameras")]
    [SerializeField] private MiniMapCamera _mapFollow;
    [SerializeField] private MapCamera _mapFixed;

    [Header("Map Fog")]
    [SerializeField] private FogOfWarMask _fog;

    #region Const data
    [Header("Datas")]
    [SerializeField] public PlayerSO _playerSO;
    [SerializeField] public MazeSO _mazeSO;
    public GameObject PlayerObj { get; private set; }
    public GameObject GoalObj { get; private set; }
    public Vector3 PlayerSpawnPoint {  get; private set; }
    public Vector3 GoalSpawnPoint {  get; private set; }
    public CharacterController CharacterCtrl { get; private set; }
    #endregion

    #region For puzzle
    [Header("Pool For Item")]
    public Transform PoolClone;
    #endregion

    #region Cell

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

    #region Level

    private int _currentLevelIndex = 0;
    public int CurrentLevel => _currentLevelIndex + 1;

    #endregion

    private void Reset()
    {
        _mazeSO = Resources.Load<MazeSO>($"Scriptable/MazeLevel/Level{_currentLevelIndex + 1}");
        _playerSO = Resources.Load<PlayerSO>("Scriptable/PlayerSO");
        _mapFollow = GetComponentInChildren<MiniMapCamera>();
        _mapFixed = GetComponentInChildren<MapCamera>();
        _fog = GetComponent<FogOfWarMask>();
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadNewLevel();
    }
    private void Start()
    {
        CreateSpawnPoint(MazeGenerator.MazeGrid[0, 0, 0], 
            MazeGenerator.MazeGrid[_mazeSO.Width - 1, _mazeSO.Height - 1, _mazeSO.Depth - 1]);
        CreatePlayer();

        _fog.SetUpSize(_mazeSO, _mazeSO.GetSizeScale);
        _mapFixed.ResizeMap(_mazeSO);
    }
    private void Update()
    {
        if (PlayerObj == null) return;

        CurrentCell = MazeTools.GetCellFromGameObject(PlayerObj, MazeGenerator.MazeGrid, _mazeSO.BoxSize, _mazeSO.GetSizeScale);
        if (!CurrentCell.flagVisited)
        {
            CurrentCell.flagVisited = true;
            CurrentCell.HighlightForMiniMap(Color.red);
        }
        
    }
    private void LateUpdate()
    {
        _mapFollow.FollowCamera(PlayerObj.transform);
        _fog.Reveal(PlayerObj.transform.position);
        //For reveal by cellsize
        //List<Cell> revealCell = MazeTools.GetNeighborsInSquare(CurrentCell, MazeGenerator.boardLayout, _mazeSO.BoxSize);
        //_fog.RevealCells(revealCell);
    }
    #region Maze Handler
    public void ResetMaze()
    {
        foreach (Transform child in PoolClone)
        {
            child.gameObject.SetActive(false);
        }

        CharacterCtrl.enabled = false;
        MazeGenerator.Instance.ResetGrid();

        OnLevelReset?.Invoke();
        LoadNewLevel();
        ResetSpawnPoint();
        _fog.SetUpSize(_mazeSO, _mazeSO.GetSizeScale);
        _fog.ResetFog();
        _mapFixed.ResizeMap(_mazeSO);
        CharacterCtrl.enabled = true;

    }
    public void CreatePlayer()
    {
        PlayerObj = Instantiate(_playerSO.playerPrefab, PlayerSpawnPoint, Quaternion.identity, transform);
        GoalObj = Instantiate(_mazeSO.GoalPrefab, GoalSpawnPoint, Quaternion.identity, transform);
        CharacterCtrl = PlayerObj.GetComponent<CharacterController>();
    }
    public void CreateSpawnPoint(Cell cellStart, Cell cellEnd)
    {
        PlayerSpawnPoint = cellStart.transform.Find("SpawnPoint").position;
        GoalSpawnPoint = cellEnd.transform.Find("SpawnPoint").position;
    }
    public void ResetSpawnPoint()
    {
        CreateSpawnPoint(MazeGenerator.MazeGrid[0, 0, 0],
    MazeGenerator.MazeGrid[_mazeSO.Width - 1, _mazeSO.Height - 1, _mazeSO.Depth - 1]);

        PlayerObj.transform.position = PlayerSpawnPoint;
        GoalObj.transform.position = GoalSpawnPoint;
        

    }
    public void LoadNewLevel()
    {
        _mazeSO = Resources.Load<MazeSO>($"Scriptable/MazeLevel/Level{_currentLevelIndex + 1}");
        MazeGenerator.Instance.CreateGrid(_mazeSO);
        _mazeSO.Generate();
    }
    #endregion

    #region UI Handler
    public void LevelUpgrade()
    {
        if (_currentLevelIndex <  MazeCount() - 1)
        {
            _currentLevelIndex++;
        }
        else
        {
            Debug.Log("You win! All levels completed!");
            return;
        }

        OnLevelUpgraded?.Invoke(); 
        ResetMaze();
    }
    #endregion
    int MazeCount() => System.Enum.GetValues(typeof(MazeAlgorithmType)).Length;
}


