using UnityEngine;

public class PuzzleBoardBase<T> : MonoBehaviour, IBoardButton where T : ScriptableObject
{
    public T _levelData {  get; private set; }
    private PuzzleState _currentState = PuzzleState.NotStarted;
    public virtual void LoadLevelData(T data)
    {
        _levelData = data;
    }

    public virtual void StartGame()
    {
        if (_levelData == null)
        {
            NotifyManager.Instance.Notify("Get data failed");
            return;
        }
        switch (_currentState)
        {
            case PuzzleState.Solved:
                NotifyManager.Instance.Notify("You have completed game");
                return;

            case PuzzleState.InProgress:
                NotifyManager.Instance.Notify("You must complete game first");
                return;

            case PuzzleState.NotStarted:
                BuildBoard(_levelData);
                _currentState = PuzzleState.InProgress;
                break;
        }
    }
    public virtual void ResetGame()
    {
        if (_currentState == PuzzleState.Solved)
        {
            NotifyManager.Instance.Notify("You have completed game");
            return;
        }

        ResetState();
        BuildBoard(_levelData);
        _currentState = PuzzleState.InProgress;
    }
    public virtual void CloseGame()
    {
        ResetState();
        _currentState = PuzzleState.NotStarted;

        transform.gameObject.SetActive(false);
        CameraSwitch.Instance.SwitchInventoryCamera();

    }

    protected virtual void OnPuzzleSolved()
    {
        _currentState = PuzzleState.Solved;

        NotifyManager.Instance.Notify("Clear!");
        CloseGame();
    }
    protected virtual void OnPuzzleUnSolved()
    {
        _currentState = PuzzleState.NotStarted;
        NotifyManager.Instance.Notify("Try again!");
    }
    protected virtual void ResetState() { }
    protected virtual void BuildBoard(T levelData) { }
}
