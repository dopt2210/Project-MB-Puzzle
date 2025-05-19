using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PairPathBoard : MonoBehaviour, IBoardButton
{
    [Header("Refs")]
    [SerializeField] private PairPathSO levelData;
    [SerializeField] private TilePath tilePrefab;

    [SerializeField] GridLayoutGroup boardLayout;

    #region Process vars
    int _size;
    [SerializeField] bool _isEndGame = true;
    TilePath[,] _tiles;
    HashSet<int> remainingPairIds;
    #endregion

    private void Reset()
    {
        boardLayout = GetComponentInChildren<GridLayoutGroup>();
    }
    private void OnEnable()
    {
        if (levelData == null || tilePrefab == null)
        {
            Debug.LogWarning("You must drag prefab Resources/Prefab/PuzzleGame/PairPath/TilePath");
            Debug.LogWarning("You must drag level data Resources/Scriptabel/PairPathSO");
            return;
        }
        remainingPairIds = new(levelData.pairs.Select(p => p.id));
        _size = levelData.boardSize;

        LineDraw.OnThisPairCompleted += PairIsSolved;
    }

    #region UI function
    public void StartGame()
    {
        if (_isEndGame)
        {
            BuildBoard();
        }
        else Debug.Log("You must finish game first");
    }

    public void CloseGame() => transform.gameObject.SetActive(false);
    
    public void ResetGame()
    {
        // 1) Xoá tile cũ
        foreach (Transform t in boardLayout.transform) Destroy(t.gameObject);

        // 2) Dọn LineDraw
        LineDraw.Instance.ResetLines();

        // 3) Tạo lại danh sách id còn lại
        remainingPairIds = new(levelData.pairs.Select(p => p.id));

        // 4) Build lại lưới
        BuildBoard();
    }

    #endregion

    #region Build game
    private void BuildBoard()
    {
        _isEndGame = false;
        _tiles = new TilePath[_size, _size];
        if (levelData.seed >= 0) Random.InitState(levelData.seed);

        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                var tileCoord = new Vector2Int(x, y);
                var tile = Instantiate(tilePrefab, boardLayout.transform);
                tile.InitBlank(tileCoord);
                _tiles[x, y] = tile;
            }
        }
        foreach (var p in levelData.pairs)
        {
            PlaceTile(p.start, p);
            PlaceTile(p.end, p);
        }
    }

    private void PlaceTile(Vector2Int pos, PairPathSO.Pair data)
    {
        var cell = _tiles[pos.x, pos.y];
        cell.InitTile(pos, data.id, data.color);
    }

    private void PairIsSolved(int pairId)
    {
        remainingPairIds.Remove(pairId);
        if (remainingPairIds.Count == 0)
            OnPuzzleSolved();
    }

    private void OnPuzzleSolved()
    {
        Debug.Log("Tile‑Path-Pair completed!");
        // TODO: Play SFX, invoke UnityEvent, unlock door, etc.
        _isEndGame = true;
    }

    #endregion
}