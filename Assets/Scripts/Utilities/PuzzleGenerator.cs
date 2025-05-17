using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    public static PuzzleGenerator Instance { get; private set; }

    [SerializeField] private List<PuzzlePrefabs> _puzzleLibrary;

    private Dictionary<MazeAlgorithmType, List<GameObject>> _puzzleDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        BuildDictionary();
    }
    private void BuildDictionary()
    {
        _puzzleDict = new Dictionary<MazeAlgorithmType, List<GameObject>>();

        foreach (var entry in _puzzleLibrary)
        {
            if (!_puzzleDict.ContainsKey(entry.algorithmType))
            {
                _puzzleDict.Add(entry.algorithmType, entry.puzzlePrefabs);
            }
            else
            {
                Debug.LogWarning($"Duplicate algorithm entry: {entry.algorithmType}");
            }
        }
    }
    public List<GameObject> GetPuzzlePrefabs(MazeAlgorithmType algorithm)
    {
        if (_puzzleDict.TryGetValue(algorithm, out var prefabs))
        {
            return prefabs;
        }

        Debug.LogWarning($"No prefabs found for algorithm: {algorithm}");
        return null;
    }
    public GameObject GetRandomPuzzlePrefab(MazeAlgorithmType algorithm)
    {
        var list = GetPuzzlePrefabs(algorithm);
        if (list != null && list.Count > 0)
        {
            return list[Random.Range(0, list.Count)];
        }

        return null;
    }

}

[System.Serializable]
public class PuzzlePrefabs
{
    public MazeAlgorithmType algorithmType;
    public List<GameObject> puzzlePrefabs; 
}