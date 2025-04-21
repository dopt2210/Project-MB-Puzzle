using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject map;
    public static bool IsPaused { get; set; }
    public static bool IsDebug { get; set; }
    public static bool IsPlaying { get; set; }
     
    [Header("Sub UI Controllers")]
    [SerializeField] private UIDebug uiDebuger;
    [SerializeField] private CustomFocusRing uiPauser;
    [SerializeField] private UIInformation uiInformation;
    [SerializeField] private UICheatSheet uICheatSheet;
    private void Reset()
    {
        map = GameObject.FindGameObjectWithTag("MapHolder");
        uiDebuger = GetComponentInChildren<UIDebug>();
        uiPauser = GetComponentInChildren<CustomFocusRing>();
        uiInformation = GetComponentInChildren<UIInformation>();
        uICheatSheet = GetComponentInChildren<UICheatSheet>();
    }
    private void Start()
    {
        map.SetActive(false);
        UpdateUIInfomation();
        uiDebuger.Hide();
        uiPauser.Hide();
        uICheatSheet.Hide();
        uiInformation.Show();
    }
    private void Update()
    {
        if (IsPaused)
        {
            Time.timeScale = 0;
        }
        else Time.timeScale = 1;


        if (InputManager.Instance.kOpenPause) PauseGame();
        if (InputManager.Instance.kClosePause) ResumeGame();

        if (InputManager.Instance.kOpenDebug) ToggleUI();

        if (InputManager.Instance.kOpenMap) OpenMap();

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

    private void PauseGame()
    {
        IsPaused = true;
        uiPauser.Show();
        MouseLock.Instance.AutoHandleMouseLockByPause(IsPaused);
        //uiPauser.ShowOptionPanel();
    }
    private void ResumeGame()
    {
        IsPaused = false;
        uiPauser.HideOptionPanel();
        uiPauser.Hide();
        MouseLock.Instance.AutoHandleMouseLockByPause(IsPaused);
    }
    private void OpenMap()
    {
        map.gameObject.SetActive(!map.gameObject.activeSelf);
    }
}
