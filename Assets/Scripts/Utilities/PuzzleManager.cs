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
    public void PlayTileSwapPuzzle()
    {
        tilePuzzleTransform.gameObject.SetActive(true);
    }
    public void PlayPairPathPuzzle()
    {
        pairPuzzleTransform.gameObject.SetActive(true);

    }
    public void PlayWordlePuzzle()
    {
        wordlePuzzleTransform.gameObject.SetActive(true);

    }

}
