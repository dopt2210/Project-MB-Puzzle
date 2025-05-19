using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;
    [SerializeField] private GameObject map;
    public static bool IsPaused { get; set; } = false;
    public static bool IsDebug { get; set; }
    public static bool IsPlaying { get; set; }

    [Header("Sub UI Controllers")]
    [SerializeField] private UIDebug uiDebuger;
    [SerializeField] private PauseMenuUI uiPauser;
    [SerializeField] private UIInformation uiInformation;
    private void Reset()
    {
        map = GameObject.FindGameObjectWithTag("MapHolder");
        uiDebuger = GetComponentInChildren<UIDebug>();
        uiPauser = GetComponentInChildren<PauseMenuUI>();
        uiInformation = GetComponentInChildren<UIInformation>();
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
        map.SetActive(false);
        UpdateUIInfomation();
        uiDebuger.Hide();
        uiPauser.Hide();
        uiInformation.Show();
    }
    private void Update()
    {
        if (InputManager.Instance.Action.Pause && !IsPaused) PauseGame();
        else if (InputManager.Instance.Action.Resume && IsPaused) ResumeGame();

        if (IsPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }


        if (InputManager.Instance.Action.OpenDebug) ToggleUI();

        if (InputManager.Instance.Action.OpenMap) ToggleMap();
        

    }
    private void LateUpdate()
    {
        
    }
    private void OnEnable()
    {
        GameManager.OnLevelUpgraded += UpdateUIInfomation;
        GameManager.OnLevelReset += UpdateUIInfomation;
        GameManager.OnPlayerCellChanged += UpdatePlayerPosition;
    }

    private void OnDisable()
    {
        GameManager.OnLevelUpgraded -= UpdateUIInfomation;
        GameManager.OnLevelReset -= UpdateUIInfomation;
        GameManager.OnPlayerCellChanged -= UpdatePlayerPosition;
    }

    #region CanvasUI
    private void UpdateUIInfomation()
    {
        int level = GameManager.Instance.CurrentLevel;
        uiInformation.UpdateLevel(level);

        string name = GameManager.Instance.CurrentAlgorithmName;
        uiDebuger.UpdateAlgo(name);

        uiInformation.ResetTime();
    }
    private void UpdatePlayerPosition(Cell newCell)
    {
        uiDebuger.UpdateCoord(newCell);
    }
    private void ToggleUI()
    {
        uiDebuger.Toggle();
    }

    public void PauseGame()
    {
        IsPaused = true;
        MouseLock.Instance.AutoHandleMouseLockByPause(IsPaused);
        InputManager.InputPlayer.SwitchCurrentActionMap("UI");
        uiPauser.Show();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        MouseLock.Instance.AutoHandleMouseLockByPause(IsPaused);
        InputManager.InputPlayer.SwitchCurrentActionMap("Player");
        uiPauser.Hide();
    }
    public void BackToMainMenu()
    {
        IsPaused = false;
        SceneManager.LoadScene("MenuScene");
        MusicManager.Instance.PlayMusic("Start");
    }
    public void ToggleMap()
    {
        map.gameObject.SetActive(!map.gameObject.activeSelf);
    }
    #endregion
}
