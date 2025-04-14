using UnityEngine;

public class UIHandler : MonoBehaviour
{
    //[SerializeField] private Button setting;
    [SerializeField] private GameObject map;

    public static bool IsPaused { get; set; }
    public static bool IsDebug { get; set; }
     
    [Header("Sub UI Controllers")]
    [SerializeField] private UIDebug uiDebuger;
    [SerializeField] private UIPause uiPauser;
    [SerializeField] private UIInformation uiInformation;
    [SerializeField] private UICheatSheet uICheatSheet;


    private void Start()
    {
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
        if(InputManager.Instance.kOpenPause) PauseGame();
        if(InputManager.Instance.kClosePause) ResumeGame();

        if (InputManager.Instance.kOpenDebug) ToggleDebug();

        if (InputManager.Instance.kOpenMap) OpenMap();

        //if (InputManager.Instance.kOpenMouse) HandleMouseToggle();

    }

    private void ToggleDebug()
    {
        uiDebuger.Toggle();
        uICheatSheet.Toggle();
        IsDebug = false;
    }

    private void PauseGame()
    {
        uiPauser.Show();
        IsPaused = true;
    }
    private void ResumeGame()
    {
        uiPauser.Hide();
        IsPaused = false;
    }
    private void OpenMap()
    {
        map.gameObject.SetActive(!map.gameObject.activeSelf);
    }
    private void HandleMouseToggle()
    {
        bool showMouse = InputManager.Instance.kOpenMouse;

        Cursor.visible = showMouse;
        Cursor.lockState = showMouse ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
