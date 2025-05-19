using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    private static UIPause instance;
    public static UIPause Instance {  get { return instance; } }
    public Button[] buttons;


    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        buttons = GetComponentsInChildren<Button>();
        SetButtonEvent();
    }
    private void SetButtonEvent()
    {
        RemoveButtonEvent();
        buttons[0].onClick.AddListener(Resume);
        buttons[1].onClick.AddListener(ResetGame);
        buttons[2].onClick.AddListener(SaveGame);
        buttons[3].onClick.AddListener(Options);
        buttons[4].onClick.AddListener(QuitGame);

        foreach (var button in buttons)
        {
            UITools.AddEventTrigger(button.gameObject, EventTriggerType.PointerEnter, OnPointerEnter);
            UITools.AddEventTrigger(button.gameObject, EventTriggerType.PointerClick, OnPointerClick);
        }

    }

    private void RemoveButtonEvent()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        InputManager.InputPlayer.SwitchCurrentActionMap("UI");
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        InputManager.InputPlayer.SwitchCurrentActionMap("Player");
    }
    private void Resume()
    {
        Hide();
        UIHandler.IsPaused = false;
    }
    private void ResetGame()
    {
        Resume();
        UIHandler.IsPaused = false;
        GameManager.Instance.ResetMaze();

    }
    private void SaveGame()
    {
        Debug.Log("SaveGame");

    }
    private void Options()
    {
        Debug.Log("Options");

    }
    private void QuitGame()
    {
        Debug.Log("QuitGame");

    }
    private void OnPointerEnter(BaseEventData data)
    {
        SoundManager.Instance.PlaySound2D("Hover");
    }
    private void OnPointerClick(BaseEventData data)
    {
        SoundManager.Instance.PlaySound2D("Click");
    }
}
