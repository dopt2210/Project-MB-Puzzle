using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public MazeSO mazeSO;

    public Button startGame;
    public Button resetGame;
    public Button randomLevel;
    public Button findPath;

    public TMP_InputField[] sizeOfMaze;

    public TMP_Dropdown level;
    private System.Action[] mazeGenerators;
    private void Awake()
    {
        CreatEvent();
    }
    private void Start()
    {
        startGame.onClick.AddListener(StartGame);
        resetGame.onClick.AddListener(ResetMaze);
        randomLevel.onClick.AddListener(RandomMaze);
        findPath.onClick.AddListener(FindPath);

        sizeOfMaze[0].onEndEdit.AddListener(UpdateWidth);
        sizeOfMaze[1].onEndEdit.AddListener(UpdateHeight);
        sizeOfMaze[2].onEndEdit.AddListener(UpdateDepth);

        sizeOfMaze[0].text = mazeSO.Width.ToString();
        sizeOfMaze[1].text = mazeSO.Height.ToString();
        sizeOfMaze[2].text = mazeSO.Depth.ToString();

        level.onValueChanged.AddListener(OnDropDownChanged);
        DeActiveButton();

    }
    private void DeActiveButton()
    {
        startGame.interactable = true;

        level.interactable = false;
        resetGame.interactable = false;
        randomLevel.interactable = false;
        findPath.interactable = false;
    }
    private void ActiveButton()
    {
        startGame.interactable = false;

        level.interactable = true;
        resetGame.interactable = true;
        randomLevel.interactable = true;
        findPath.interactable = true;
    }
    public void StartGame()
    {
        if (!ValidValue()) return;
        MazeGenerator.Instance.CreateGrid();
        MazeGenerator.Instance.CreatPlayer();
        ActiveButton();
    }
    void CreatEvent()
    {
        mazeGenerators = new System.Action[]
        {
        () => new DFS(mazeSO).GenerateMazeInstant(),
        () => new BinaryTree(mazeSO).GenerateMazeInstant(),
        () => new Sidewinder(mazeSO).GenerateMazeInstant(),
        () => new AldousBroder(mazeSO).GenerateMazeInstant(),
        () => new HuntandKill(mazeSO).GenerateMazeInstant(),
        () => new RandomPrims(mazeSO).GenerateMazeInstant(),
        () => new RandomKruskal(mazeSO).GenerateMazeInstant(),
        () => new Eller(mazeSO).GenerateMazeInstant(),
        };
    }
    public void RandomMaze()
    {
        int index = Random.Range(0, 8);
        if (MazeGenerator.Instance.isDoneCreatOne)
        {
            Debug.LogError("Already created one");
            return;
        }
        if (index >= 0 && index < mazeGenerators.Length)
        {
            mazeGenerators[index]?.Invoke();
            Debug.Log($"Start {level.options[index].text}");
        }

    }
    public void OnDropDownChanged(int index)
    {
        int previousIndex = 0;
        if (MazeGenerator.Instance.isDoneCreatOne)
        {
            Debug.LogError("Already created one");
            level.value = previousIndex;
            return;
        }

        if (index >= 0 && index < mazeGenerators.Length)
        {
            mazeGenerators[index]?.Invoke();
            Debug.Log($"Start {level.options[index].text}");
            previousIndex = index;
        }
        else
        {
            Debug.LogWarning("Invalid selection");
            level.value = previousIndex;
        }
    }
    public void FindPath()
    {
        new PathFinding_Astar(mazeSO).ColorPath(Color.yellow);
        
    }
    public void ResetMaze()
    {
        if (!ValidValue()) return;
        MazeGenerator.Instance.ResetGrid();
    }
    private bool ValidValue()
    {
        int fixedAxes = (mazeSO.Width == 1 ? 1 : 0) +
                (mazeSO.Height == 1 ? 1 : 0) +
                (mazeSO.Depth == 1 ? 1 : 0);

        if (fixedAxes != 1)
        {
            Debug.Log("Maze must have exactly one fixed axis.");
            return false;
        }

        if (mazeSO.Width < 1 || mazeSO.Height < 1 || mazeSO.Depth < 1)
        {
            Debug.Log("All axes must be at least 1.");
            return false;
        }
        return true;
    }
    private void UpdateWidth(string value)
    {
        if (int.TryParse(value, out int newWidth))
        {
            if (newWidth < 1) return;
            mazeSO.Width = newWidth;

        }
    }
    private void UpdateDepth(string value)
    {
        if (int.TryParse(value, out int newDepth))
        {
            if (newDepth < 1) return;
            mazeSO.Depth = newDepth;
        }
    }
    private void UpdateHeight(string value)
    {
        if (int.TryParse(value, out int newHeight))
        {
            if (newHeight < 1) return;
            mazeSO.Height = newHeight;
        }
    }
}
