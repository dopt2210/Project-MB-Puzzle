using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private static UIHandler instance;
    public static UIHandler Instance {  get { return instance; } }
    
    public Button setting;

    public static bool IsPaused { get; set; }

    [Header("Sub UI Controllers")]
    [SerializeField] private UIDebug uiDebuger;
    [SerializeField] private UIPause uiPauser;
    [SerializeField] private UIInformation uiInformation;
    [SerializeField] private UICheatSheet uICheatSheet;


    private void Start()
    {
        uiDebuger.Show();
        uiPauser.Hide();
        uiInformation.Show();
        setting.onClick.AddListener(PauseGame);
    }

    public void ToggleDebug()
    {
        uiDebuger.Toggle();
    }

    public void PauseGame()
    {
        uiPauser.Show();
        Time.timeScale = 0;
        IsPaused = true;
    }

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }
}
