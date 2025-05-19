using UnityEngine;
using UnityEngine.UI;

public class UICheatSheet : MonoBehaviour
{
    private static UICheatSheet instance;
    public static UICheatSheet Instance {  get { return instance; } }
    [SerializeField] private MazeSO mazeSO;
    public Button[] buttons;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }
    private void Start()
    {
        LoadComponents();
    }
    void LoadComponents()
    {
        buttons = GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(LevelUp);
        buttons[1].onClick.AddListener(FindPath);
    }
    public void FindPath()
    {
        Cell start = MazeGenerator.grid[0, 0, 0];
        Cell end = MazeGenerator.grid[mazeSO.Width - 1, mazeSO.Height - 1, mazeSO.Depth - 1];
        MazeTools.ColorPath(Color.yellow, start, end, mazeSO);

    }
    public void LevelUp() => GameManager.Instance.LevelUpgrade();
    public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
