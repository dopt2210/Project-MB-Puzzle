using UnityEngine;

public class GameManager : MonoBehaviour, IGameData
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

    #region Private vars
    private Vector3 PlayerSpawnPoint;
    private Vector3 GoalSpawnPoint;

    private Cell _currentPlayerCell;

    private int _currentLevelIndex = 0;


    #endregion

    #region Public vars
    [Header("Datas")]
    public PlayerSO _playerSO;
    public MazeSO _mazeSO;
    [Header("Pool For Item")]
    [Tooltip("Assign pool for spawn puzzle item")]
    public Transform PoolClone;
    public GameObject PlayerObj { get; private set; }
    public GameObject GoalObj { get; private set; }
    public CharacterController CharacterCtrl { get; private set; }
    public int CurrentLevel => _currentLevelIndex + 1;
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
    private void Reset()
    {
        _mazeSO = Resources.Load<MazeSO>($"Scriptable/MazeLevel/Level{_currentLevelIndex + 1}");
        _playerSO = Resources.Load<PlayerSO>("Scriptable/PlayerSO");
        _mapFollow = GetComponentInChildren<MiniMapCamera>();
        _mapFixed = GetComponentInChildren<MapCamera>();
        _fog = GetComponent<FogOfWarMask>();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
    private void CreatePlayer()
    {
        PlayerObj = Instantiate(_playerSO.playerPrefab, PlayerSpawnPoint, Quaternion.identity, transform);
        GoalObj = Instantiate(_mazeSO.GoalPrefab, GoalSpawnPoint, Quaternion.identity, transform);
        CharacterCtrl = PlayerObj.GetComponent<CharacterController>();
    }
    private void CreateSpawnPoint(Cell cellStart, Cell cellEnd)
    {
        PlayerSpawnPoint = cellStart.transform.Find("SpawnPoint").position;
        GoalSpawnPoint = cellEnd.transform.Find("SpawnPoint").position + new Vector3(0, -2.5f, 2f);
    }
    private void ResetSpawnPoint()
    {
        CreateSpawnPoint(MazeGenerator.MazeGrid[0, 0, 0],
    MazeGenerator.MazeGrid[_mazeSO.Width - 1, _mazeSO.Height - 1, _mazeSO.Depth - 1]);

        PlayerObj.transform.position = PlayerSpawnPoint;
        GoalObj.transform.position = GoalSpawnPoint;
        

    }
    private void LoadNewLevel()
    {
        _mazeSO = Resources.Load<MazeSO>($"Scriptable/MazeLevel/Level{_currentLevelIndex + 1}");
        MazeGenerator.Instance.CreateGrid(_mazeSO);
        _mazeSO.Generate();
    }
    #endregion

    #region Public UI Handler
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
    int MazeCount() => System.Enum.GetValues(typeof(MazeAlgorithmType)).Length;
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
        SceneLoadManager.Instance.LoadSceneWithLoading();
    }
    public void SwitchOn()
    {
        MouseLock.Instance.UnlockMouse();
        InputManager.InputPlayer.SwitchCurrentActionMap("UI");
    }
    public void SwitchOff()
    {
        MouseLock.Instance.LockMouse();
        InputManager.InputPlayer.SwitchCurrentActionMap("Player");
    }

    public void LoadData(GameData gameData)
    {
        if (gameData == null) return;
        _currentLevelIndex = gameData.currentLevelIndex;

        LoadNewLevel();
    }

    public void SaveData(ref GameData gameData)
    {
        if (gameData == null) return;
        gameData.currentLevelIndex = _currentLevelIndex;
    }
    #endregion
}