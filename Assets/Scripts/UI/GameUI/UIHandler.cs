using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }
    [Header("Map UI")]
    [Tooltip("Drag mini map here")]
    [SerializeField] private Transform _miniMap;
    private IInteractable _interactable;
    public static bool IsPaused { get; set; } = false;
    public static bool IsDebug { get; set; }
    public static bool IsPlaying { get; set; }
    [Header("Sub UI Controllers")]
    [SerializeField] private UIDebug uiDebuger;
    [SerializeField] private PauseMenuUI uiPauser;
    [SerializeField] private UIInformation uiInformation;
    [SerializeField] private Transform uiInteract;
    private void Reset()
    {
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
        _miniMap.gameObject.SetActive(false);
        UpdateUIInfomation();
        uiDebuger.Hide();
        uiPauser.Hide();
        uiInformation.Show();
    }
    private void Update()
    {
        if (InputManager.Instance.Action.Pause && !IsPaused) {  uiPauser.Show(); PauseGame(); }
        else if (InputManager.Instance.Action.Resume && IsPaused) { uiPauser.Hide(); ResumeGame(); }

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
        if (_interactable != null)
        {
            _interactable.OnRangeChanged -= HandleRangeChanged;
        }
    }
    public void RegisterTarget(IInteractable target)
    {
        if (_interactable != null)
        {
            _interactable.OnRangeChanged -= HandleRangeChanged;
        }

        _interactable = target;

        if (_interactable != null)
        {
            _interactable.OnRangeChanged += HandleRangeChanged;
            uiInteract.gameObject.SetActive(_interactable.IsInRange); // cập nhật ngay UI
        }
    }
    private void HandleRangeChanged(bool inRange)
    {
        uiInteract.gameObject.SetActive(inRange);
    }
    #region CanvasUI
    private void UpdateUIInfomation()
    {
        int level = GameManager.Instance.CurrentLevel;
        uiInformation.UpdateLevel(level);

        string name = GameManager.Instance._mazeSO.CurrentAlgorithmName;
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
        GameManager.Instance.SwitchOn();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        GameManager.Instance.SwitchOff();
    }
    public void BackToMainMenu()
    {
        IsPaused = false;
        SceneLoadManager.Instance.LoadSceneWithLoading("MenuScene");
    }
    public void ToggleMap()
    {
        _miniMap.gameObject.SetActive(!_miniMap.gameObject.activeSelf);
    }
    public void ShowHint(string text)
    {
        uiInformation.ShowHint(text);
    }
    #endregion
}
