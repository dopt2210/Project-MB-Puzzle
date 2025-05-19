using UnityEngine;
using UnityEngine.UI;

public class UICheatSheet : MonoBehaviour
{
    [SerializeField] private MazeSO mazeSO;
    public Button[] buttons;

    private void Reset()
    {
        LoadComponents();
    }
    private void LoadComponents()
    {
        mazeSO = Resources.Load<MazeSO>("Scriptable/MazeSO");
        buttons = GetComponentsInChildren<Button>();

    }
    private void OnEnable()
    {
        AddEventClick();
    }
    private void OnDisable()
    {
        RemoveEventClick();
    }
    void AddEventClick()
    {
        buttons[0].onClick.AddListener(LevelUp);
        buttons[1].onClick.AddListener(FindPath);
    }
    void RemoveEventClick()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    public void FindPath()
    {
        Debug.Log($"Finding");
        Cell start = GameManager.Instance.CurrentCell;
        Cell end = MazeGenerator.grid[mazeSO.Width - 1, mazeSO.Height - 1, mazeSO.Depth - 1];
        MazeTools.ColorPath(Color.yellow, start, end, mazeSO);
    }
    public void LevelUp() => GameManager.Instance.LevelUpgrade();
    public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
