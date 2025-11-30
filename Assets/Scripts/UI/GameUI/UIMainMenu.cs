using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private Transform panelSelect;
    private void Reset()
    {
        buttons = GetComponentsInChildren<Button>(true);
    }
    private void OnEnable()
    {
        LoadButtons();
    }
    private void OnDisable()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    private void LoadButtons()
    {
        foreach (var button in buttons)
        {
            UITools.AddEventTrigger(button.gameObject, EventTriggerType.PointerEnter, UITools.OnPointerEnter);
            UITools.AddEventTrigger(button.gameObject, EventTriggerType.PointerClick, UITools.OnPointerClick);
        }
        buttons[0].onClick.AddListener(StartGame); // Start Game
        buttons[1].onClick.AddListener(OpitonGame); // Option Game
        buttons[2].onClick.AddListener(IntroductionGame); // Introduction Game
        buttons[3].onClick.AddListener(QuitGame); // Quit Game
        buttons[4].onClick.AddListener(NewGame); // New Game
        buttons[5].onClick.AddListener(LoadGame); // Load Game
        buttons[6].onClick.AddListener(() => panelSelect.gameObject.SetActive(false)); // Back to Main Menu
    }
    private void StartGame()
    {
        panelSelect.gameObject.SetActive(true);
    }
    public void NewGame()
    {
        GameDataManager.Instance.NewGame();
        SceneLoadManager.Instance.LoadSceneWithLoading("GameScene");
        MusicManager.Instance.PlayMusic("BGM");
        Hide();
    }
    public void LoadGame()
    {
        GameDataManager.Instance.LoadGame();
        SceneLoadManager.Instance.LoadSceneWithLoading("GameScene");
        MusicManager.Instance.PlayMusic("BGM");
        Hide();
    }
    private void IntroductionGame()
    {
        Debug.Log("Introduction Game Clicked");
    }
    private void OpitonGame()
    {
        UIInstance.Instance.ShowOptionUI();
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    #region Ultilities
    public void Show() => gameObject.SetActive(true);
    public void Hide()
    {
        gameObject.SetActive(false);
        panelSelect.gameObject.SetActive(false);
    }

    #endregion
}
