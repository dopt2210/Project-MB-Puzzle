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
    public void IntroductionGame()
    {
        Debug.Log("Introduction Game Clicked");
    }
    public void OptionGame()
    {
        UIInstance.Instance.ShowOptionUI();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    #region Utilities
    public void Show() => gameObject.SetActive(true);
    public void Hide()
    {
        gameObject.SetActive(false);
        panelSelect.gameObject.SetActive(false);
    }

    #endregion
}
