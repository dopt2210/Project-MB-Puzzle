using UnityEngine;

public class PuzzleBoardBase<T> : MonoBehaviour, IGameData, IBoardButton where T : ScriptableObject
{
    public T _levelData {  get; private set; }
    private PuzzleState _currentState = PuzzleState.NotStarted;
    private bool _isSolved = false;
    public virtual void LoadLevelData(T data, GameData gameData = null)
    {
        _levelData = data;

        if (gameData != null)
        {
            string uid = GetUIDFromLevelData(_levelData);
            if (gameData.puzzleStates.TryGetValue(uid, out bool solved))
            {
                _isSolved = solved;
                _currentState = solved ? PuzzleState.Solved : PuzzleState.NotStarted;
            }
        }
    }

    public virtual void StartGame()
    {
        if (_levelData == null)
        {
            NotifyManager.Instance.Notify("Get data failed");
            return;
        }
        if (_isSolved)
        {
            NotifyManager.Instance.Notify("You have completed game");
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
        if (_isSolved)
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
        _isSolved = true;
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
    protected virtual string GetUIDFromLevelData(T data)
    {
        if (data is TileSwapSO tile) return tile.uniqueId;
        if (data is PairPathSO pair) return pair.uniqueId;
        if (data is WordleSO wordle) return wordle.uniqueId;

        Debug.LogWarning("Unknown puzzle type.");
        return "";
    }

    public virtual void LoadData(GameData gameData)
    {
        if (_levelData == null) return;
        string uid = GetUIDFromLevelData(_levelData);
        if (gameData.puzzleStates.TryGetValue(uid, out bool solved))
        {
            Debug.Log($"Puzzle state loaded for {uid}: {solved}");
            _isSolved = solved;
        }
    }

    public virtual void SaveData(ref GameData gameData)
    {
        if (_levelData == null) return;

        string uid = GetUIDFromLevelData(_levelData);
        if (gameData.puzzleStates.ContainsKey(uid))
            gameData.puzzleStates.Set(uid, _isSolved);
        else
            gameData.puzzleStates.Add(uid, _isSolved);
    }
}
