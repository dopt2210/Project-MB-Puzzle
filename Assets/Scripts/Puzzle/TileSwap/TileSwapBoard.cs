using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileSwapBoard : MonoBehaviour, IBoardButton
{
    [Header("Refs")]
    [SerializeField] private TileSwapSO levelData;
    [SerializeField] private TileSwap tilePrefab;
    [SerializeField] private TileSwap blankPrefab;

    [SerializeField] private GridLayoutGroup boardLayout;

    #region Process vars
    int _size;
    float _cellSize;
    [SerializeField] bool _isEndGame = true;
    TileSwap[,] _tiles;              
    Vector2Int _emptyCoord;
    #endregion

    private void Reset()
    {
        boardLayout = GetComponentInChildren<GridLayoutGroup>();
    }
    private void OnEnable()
    {
        if (levelData == null || tilePrefab == null || blankPrefab == null)
        {
            Debug.LogWarning("You must drag prefab Resources/Prefab/PuzzleGame/TileSwap/TileSwap");
            Debug.LogWarning("You must drag prefab Resources/Prefab/PuzzleGame/TileSwap/Blank");
            Debug.LogWarning("You must drag level data Resources/Scriptabel/TileSwapSO");
            return;
        }

        _size = levelData.boardSize;    
        _cellSize = boardLayout.cellSize.x;
    }

    #region UI function
    public void StartGame()
    {
        if (_isEndGame)
        {
            BuildTiled();
        }
        else Debug.Log("You must finish game first");
    }

    public void ResetGame()
    {
        foreach (Transform child in boardLayout.transform)
            Destroy(child.gameObject);

        _tiles = null;
        _emptyCoord = Vector2Int.zero;

        BuildTiled();
    }

    public void CloseGame() => transform.gameObject.SetActive(false);

    #endregion

    #region Build game
    private void BuildTiled()
    {
        _isEndGame = false;
        boardLayout.constraintCount = _size;
        _tiles = new TileSwap[_size, _size];

        var pieces = SliceSprite(levelData.sourceImage, _size);

        int[] order = ShuffleSolvable(_size, levelData.seed);
        //If not shuffle use _size*_size instead
        //int[] disorder = new int[_size * _size];
        for (int idx = 0; idx < order.Length; idx++)
        {
            int x = idx % _size;
            int y = idx / _size;
            var tileCoord = new Vector2Int(x, y);

            if (order[idx] == 0)          // 0 = blank
            {
                var blank = Instantiate(blankPrefab, boardLayout.transform);
                blank.name = "Blank";
                _tiles[x, y] = null;
                _emptyCoord = tileCoord;
                PlaceAtGrid(blank, tileCoord);
                continue;
            }

            var tile = Instantiate(tilePrefab, boardLayout.transform);
            tile.name = $"{tileCoord}";
            tile.Init(pieces[order[idx] - 1], tileCoord, order[idx]);
            _tiles[x, y] = tile;
            PlaceAtGrid(tile, tileCoord);
        }
    }
    public void RequestSwap(TileSwap tile)
    {
        if (IsAdjacent(tile.Coord, _emptyCoord))
        {
            StartCoroutine(SwapCoroutine(tile));
        }
    }
    private IEnumerator SwapCoroutine(TileSwap tile)
    {
        var targetLocal = GridToLocal(_emptyCoord);

        Vector2Int oldPos = tile.Coord;
        Vector2Int newPos = _emptyCoord;

        _tiles[newPos.x, newPos.y] = tile;
        _tiles[oldPos.x, oldPos.y] = null;

        tile.Coord = newPos;
        _emptyCoord = oldPos;

        yield return tile.MoveToLocal(targetLocal);
        if (IsSolved()) OnPuzzleSolved();
    }
    private void PlaceAtGrid(TileSwap tile, Vector2Int c) =>
        tile.transform.localPosition = GridToLocal(c);
    private bool IsSolved()
    {
        if (_emptyCoord != new Vector2Int(_size - 1, _size - 1))
            return false;

        for (int y = 0; y < _size; y++)
            for (int x = 0; x < _size; x++)
            {
                var t = _tiles[x, y];
                if (t == null) continue;          
                int goalId = y * _size + x + 1;    
                if (t.Id != goalId)
                    return false;
            }
        return true;
    }
    private void OnPuzzleSolved()
    {
        Debug.Log("Tile‑Swap completed!");
        // TODO: Play SFX, invoke UnityEvent, unlock door, etc.
        _isEndGame = true;
    }
    #endregion

    #region Ultilities function
    private static List<Sprite> SliceSprite(Sprite source, int n)
    {
        var list = new List<Sprite>();
        var tex = source.texture;
        int size = (int)(source.rect.width / n);
        for (int row = 0; row < n; row++)
            for (int col = 0; col < n; col++)
            {
                var flippedRow = (n - 1) - row;
                var r = new Rect(source.rect.x + col * size,
                                 source.rect.y + flippedRow * size,
                                 size, size);
                list.Add(Sprite.Create(tex, r, new Vector2(.5f, .5f), source.pixelsPerUnit));
            }
        return list;
    }
    private bool IsAdjacent(Vector2Int a, Vector2Int b) =>
        Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    private Vector3 GridToLocal(Vector2Int c) => new(
            (c.x - _size / 2f + .5f) * _cellSize,
            (-(c.y - _size / 2f + .5f)) * _cellSize,
            0);
    private int[] ShuffleSolvable(int N, int seed = -1)
    {
        int total = N * N;
        int[] arr = Enumerable.Range(0, total).ToArray();   // 0 … N²-1 (0 = blank)

        // 1) Fisher–Yates
        System.Random rnd = new(seed == -1 ? Environment.TickCount : seed);
        for (int i = total - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }

        // 2) nếu vô nghiệm, hoán 2 tile đầu (không phải 0)
        if (!IsSolvable(arr, N)) 
        {
            if (arr[0] == 0 || arr[1] == 0)
                (arr[2], arr[3]) = (arr[3], arr[2]);
            else
                (arr[0], arr[1]) = (arr[1], arr[0]);
        }
        return arr;
    }
    private bool IsSolvable(int[] flat, int N)
    {
        int inversions = 0;
        for (int i = 0; i < flat.Length; i++)
            for (int j = i + 1; j < flat.Length; j++)
                if (flat[i] > 0 && flat[j] > 0 && flat[i] > flat[j])
                    inversions++;

        if (N % 2 == 1)          // bảng lẻ
            return inversions % 2 == 0;
        else                     // bảng chẵn
        {
            int rowFromBottom = N - (Array.IndexOf(flat, 0) / N);
            return (inversions + rowFromBottom) % 2 == 0;
        }
    }

    #endregion
}
