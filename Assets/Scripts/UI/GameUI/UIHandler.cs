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
    [SerializeField] private CustomFocusRing uiPauser;
    [SerializeField] private UIInformation uiInformation;
    [SerializeField] private UICheatSheet uICheatSheet;
    [SerializeField] private UIInventory uIInventory;
    private void Reset()
    {
        map = GameObject.FindGameObjectWithTag("MapHolder");
        uiDebuger = GetComponentInChildren<UIDebug>();
        uiPauser = GetComponentInChildren<CustomFocusRing>();
        uiInformation = GetComponentInChildren<UIInformation>();
        uICheatSheet = GetComponentInChildren<UICheatSheet>();
        uIInventory = GetComponentInChildren<UIInventory>();
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
        uICheatSheet.Hide();
        uiInformation.Show();
        uIInventory.Hide();
    }
    private void Update()
    {
        if (InputManager.Instance.Action.Pause && !IsPaused) PauseGame();
        else if (InputManager.Instance.Action.Resume && IsPaused) ResumeGame();
   
        if (InputManager.Instance.Action.OpenDebug) ToggleUI();

        if (InputManager.Instance.Action.OpenMap) OpenMap();
        
        if(InputManager.Instance.Action.OpenItem) ToggleBag();
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
    private void ExcuteCheatSheet()
    {

    }
    private void ToggleUI()
    {
        uiDebuger.Toggle();
        uICheatSheet.Toggle();
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0;
        InputManager.InputPlayer.SwitchCurrentActionMap("UI");
        MouseLock.Instance.AutoHandleMouseLockByPause(IsPaused);
        uiPauser.Show();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1;
        InputManager.InputPlayer.SwitchCurrentActionMap("Player");
        MouseLock.Instance.AutoHandleMouseLockByPause(IsPaused);
        uiPauser.Hide();
    }
    public void BackToMainMenu()
    {
        IsPaused = false;
        Time.timeScale = 1;
        InputManager.InputPlayer.SwitchCurrentActionMap("Player");
        SceneManager.LoadScene("MenuScene");
        MusicManager.Instance.PlayMusic("Start");
    }
    private void OpenMap()
    {
        map.gameObject.SetActive(!map.gameObject.activeSelf);
    }
    private void ToggleBag()
    {
        uIInventory.Toggle();
    }
}
