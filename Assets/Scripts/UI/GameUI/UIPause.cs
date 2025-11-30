using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

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
        buttons[0].onClick.AddListener(ResumeGame);
        buttons[1].onClick.AddListener(ResetGame);
        buttons[2].onClick.AddListener(OpitonGame);
        buttons[3].onClick.AddListener(QuitGame);
    }
    private void ResumeGame()
    {
        UIHandler.Instance.ResumeGame();
        Hide();
    }
    private void ResetGame()
    {
        GameManager.Instance.ResetMaze();
        ResumeGame();
    }
    private void OpitonGame()
    {
        UIInstance.Instance.ShowOptionUI();
    }
    private void QuitGame()
    {
        GameDataManager.Instance.SaveGame();
        UIHandler.Instance.BackToMainMenu();
    }
    #region Ultilities
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    #endregion
}
