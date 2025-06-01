using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PairPathBoard : PuzzleBoardBase<PairPathSO> 
{
    [Header("Refs")]
    [SerializeField] private TilePath tilePrefab;

    [SerializeField] GridLayoutGroup boardLayout;

    #region Process vars
    int _size;
    float _cellSize;
    TilePath[,] _tiles;
    HashSet<int> remainingPairIds;
    #endregion

    private void Reset()
    {
        boardLayout = GetComponentInChildren<GridLayoutGroup>();
    }
    private void OnEnable()
    {
        if (tilePrefab == null)
        {
            Debug.LogWarning("You must drag prefab Resources/Prefab/PuzzleGame/PairPath/TilePath");
            return;
        }

        LineDraw.OnThisPairCompleted += PairIsSolved;
    }
    private void OnDisable()
    {
        LineDraw.OnThisPairCompleted -= PairIsSolved;

    }

    #region UI function
    protected override void ResetState()
    {
        foreach (Transform t in boardLayout.transform) Destroy(t.gameObject);

        LineDraw.Instance.ResetLines();
    }

    public override void ResetGame()
    {
        ResetState();
        remainingPairIds = new(_levelData.pairs.Select(p => p.id));

        BuildBoard(_levelData);
    }
    #endregion

    #region Build game
    protected override void BuildBoard(PairPathSO levelData)
    {
        remainingPairIds = new(levelData.pairs.Select(p => p.id));
        _size = levelData.boardSize;

        _cellSize = boardLayout.GetComponent<RectTransform>().rect.width / _size;
        boardLayout.cellSize = new Vector2(_cellSize, _cellSize);

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


    #endregion
}