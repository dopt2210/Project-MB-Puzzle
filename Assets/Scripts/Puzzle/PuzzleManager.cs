using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    [SerializeField] private Transform tilePuzzleTransform;
    [SerializeField] private Transform pairPuzzleTransform;
    [SerializeField] private Transform wordlePuzzleTransform;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Reset()
    {

    }
    private void Start()
    {
        tilePuzzleTransform.gameObject.SetActive(false);
        pairPuzzleTransform.gameObject.SetActive(false);
        wordlePuzzleTransform.gameObject.SetActive(false);

    }
    public void PlayTileSwapPuzzle(TileSwapSO data)
    {
        var gameData = GameDataManager.Instance.GetGameData();

        CameraSwitch.Instance.SwitchPuzzleCamera();
        tilePuzzleTransform.gameObject.SetActive(true);
        tilePuzzleTransform.GetComponent<TileSwapBoard>().LoadLevelData(data, gameData);
    }
    public void PlayPairPathPuzzle(PairPathSO data)
    {
        var gameData = GameDataManager.Instance.GetGameData();
        
        CameraSwitch.Instance.SwitchPuzzleCamera();
        pairPuzzleTransform.gameObject.SetActive(true);
        pairPuzzleTransform.GetComponent<PairPathBoard>().LoadLevelData(data, gameData);
    }
    public void PlayWordlePuzzle(WordleSO data)
    {
        var gameData = GameDataManager.Instance.GetGameData();

        CameraSwitch.Instance.SwitchPuzzleCamera();
        wordlePuzzleTransform.gameObject.SetActive(true);
        wordlePuzzleTransform.GetComponent<WordleBoard>().LoadLevelData(data, gameData);
    }

}
public enum PuzzleType
{
    None,
    TilePuzzle,
    WordlePuzzle,
    PairPuzzle
}
public enum PuzzleState
{
    NotStarted,
    InProgress,
    Solved
}
[System.Serializable]
public class PuzzlePrefabs
{
    public MazeAlgorithmType algorithmType;
    public List<GameObject> puzzlePrefabs;
}
