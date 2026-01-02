using System;
using UnityEngine;

public class GameManager : MonoBehaviour, IGameData
{
    public static GameManager Instance {  get; private set; }

    #region Action callback
    public static event System.Action OnLevelUpgraded;
    public static event System.Action OnLevelReset;
    public static event System.Action<Cell> OnPlayerCellChanged;
    public static event System.Action OnPlayerGoalReached;
    public static event System.Action<int, int> OnScoreChanged; // playerScore, aiScore
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
    private int _playerMazeCount = 0;
    private int _aiMazeCount = 0;
    private bool _isRaceActive = false;
    private bool _playerFinished = false;


    #endregion

    #region Public vars
    [Header("Datas")]
    public PlayerSO _playerSO;
    public MazeSO _mazeSO;
    [Header("Pool For Item")]
    [Tooltip("Assign pool for spawn puzzle item")]
    public Transform PoolClone;
    public GameObject PlayerObj ;//{ get; private set; }
    [Header("AI Settings")]
    [Tooltip("Assign AI prefab for chase player")]
    public GameObject AIObj ;//{ get; private set; }
    public GameObject AIprefab;
    public GameObject GoalObj { get; private set; }
    public CharacterController CharacterCtrl { get; private set; }
    public int CurrentLevel => _currentLevelIndex + 1;
    public int PlayerMazeCount => _playerMazeCount;
    public int AIMazeCount => _aiMazeCount;
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

    private void OnEnable()
    {
        CharacterAIMovement.OnAIGoalReached += HandleAIGoalReached;
    }

    private void OnDisable()
    {
        CharacterAIMovement.OnAIGoalReached -= HandleAIGoalReached;
    }

    private void Start()
    {
        CreateSpawnPoint(MazeGenerator.MazeGrid[0, 0, 0], 
            MazeGenerator.MazeGrid[_mazeSO.Width - 1, _mazeSO.Height - 1, _mazeSO.Depth - 1]);
        CreatePlayer();
        _fog.SetUpSize(_mazeSO, _mazeSO.GetSizeScale);
        _mapFixed.ResizeMap(_mazeSO);
        _isRaceActive = true;
        
    }
    private void Update()
    {
        if (PlayerObj == null) return;

        CurrentCell = GetTargetCell(PlayerObj);
        if (!CurrentCell.flagVisited)
        {
            CurrentCell.flagVisited = true;
            CurrentCell.HighlightForMiniMap(Color.red);
        }

        // Check if player reached goal
        if (_isRaceActive && !_playerFinished)
        {
            var goalCell = GetTargetCell();
            if (CurrentCell == goalCell)
            {
                _playerFinished = true;
                HandlePlayerGoalReached();
            }
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

    public void FindPlayerToTarget(Cell target)
    {
        MazeTools.ColorPath(Color.yellow, CurrentCell, target, _mazeSO);
    }
    public Cell GetTargetCell(GameObject obj)
    {
        var cell = MazeTools.GetCellFromGameObject(obj, MazeGenerator.MazeGrid, _mazeSO.BoxSize, _mazeSO.GetSizeScale);
        return cell;

    }
    public Cell GetTargetCell()
    {
        var cell = MazeTools.GetCellFromGameObject(GoalObj, MazeGenerator.MazeGrid, _mazeSO.BoxSize, _mazeSO.GetSizeScale);
        return cell;

    }

    #region Maze Handler
    private void CreatePlayer()
    {
        PlayerObj = Instantiate(_playerSO.playerPrefab, PlayerSpawnPoint, Quaternion.identity, transform);
        AIObj = Instantiate(AIprefab, PlayerSpawnPoint, Quaternion.identity, transform);

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
        // AIObj position sẽ được set bởi ResetAIState(), không set ở đây để tránh conflict
    }

    private void ResetAIControllers()
    {
        var aiControllers = FindObjectsByType<CharacterAIMovement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (var ai in aiControllers)
        {
            ai.ResetAIState(PlayerSpawnPoint);
        }
    }
    private void LoadNewLevel()
    {
        _mazeSO = Resources.Load<MazeSO>($"Scriptable/MazeLevel/Level{_currentLevelIndex + 1}");
        MazeGenerator.Instance.CreateGrid(_mazeSO);
        _mazeSO.Generate();
    }

    private void HandlePlayerGoalReached()
    {
        _playerMazeCount++;
        Debug.Log($"Player hoàn thành mê cung! Tổng: {_playerMazeCount}");
        OnPlayerGoalReached?.Invoke();
        OnScoreChanged?.Invoke(_playerMazeCount, _aiMazeCount);
        AutoProgressToNextMaze("Player");
    }

    private void HandleAIGoalReached()
    {
        if (!_isRaceActive) return;
        _aiMazeCount++;
        Debug.Log($"AI hoàn thành mê cung! Tổng: {_aiMazeCount}");
        OnScoreChanged?.Invoke(_playerMazeCount, _aiMazeCount);
        AutoProgressToNextMaze("AI");
    }

    private void AutoProgressToNextMaze(string winner)
    {
        Debug.Log($"{winner} về đích trước! Chuyển sang mê cung mới...");
        
        if (_currentLevelIndex < MazeCount() - 1)
        {
            _currentLevelIndex++;
            _playerFinished = false;
            ResetMaze();
        }
        else
        {
            // Hết level, kết thúc cuộc đua
            _isRaceActive = false;
            string finalWinner = _playerMazeCount > _aiMazeCount ? "Player" : 
                                _aiMazeCount > _playerMazeCount ? "AI" : "Hòa";
            Debug.Log($"Kết thúc! {finalWinner} thắng! Player: {_playerMazeCount}, AI: {_aiMazeCount}");
        }
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
        CharacterAIMovement.AiEnabled = false;
        MazeGenerator.Instance.ResetGrid();

        OnLevelReset?.Invoke();
        LoadNewLevel();
        ResetSpawnPoint();
        _fog.SetUpSize(_mazeSO, _mazeSO.GetSizeScale);
        _fog.ResetFog();
        _mapFixed.ResizeMap(_mazeSO);
        ResetAIControllers();
        CharacterCtrl.enabled = true;
        _isRaceActive = true;
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
    public string GetFormattedTime(float timeUsed)
    {
        TimeSpan time = TimeSpan.FromSeconds(timeUsed);

        if (time.TotalHours >= 1)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
        else
            return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
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