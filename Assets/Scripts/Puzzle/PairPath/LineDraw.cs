using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    public static LineDraw Instance { get; private set; }

    [Header("Prefabs & Refs")]
    [SerializeField] LineRenderer linePrefab;   
    [SerializeField] RectTransform cloneTo;
    [SerializeField] RectTransform canvasRect;

    public static event System.Action<int> OnThisPairCompleted;

    #region Proces var
    readonly HashSet<Vector2Int> occupiedCells = new();
    readonly HashSet<Vector2Int> currentCells = new();

    readonly List<Vector3> points = new();
    readonly List<TilePath> cells = new();      

    LineRenderer _currentLine;
    TilePath startCell;
    bool isDrawing;
    int firstPairId = -1;
    #endregion

    private void Reset()
    {
        canvasRect = transform.root.GetComponent<RectTransform>();
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    #region Action function
    public void CellClicked(TilePath cell)
    {
        if (cell.IsBlank) return;

        // —— BẮT ĐẦU đường mới ——
        if (!isDrawing)
        {
            // ô thuộc cặp đã hoàn thành?  → bỏ
            if (occupiedCells.Contains(cell.Coord)) return;

            StartNewLine(cell);
            startCell = cell;
            firstPairId = cell.Id;      // lưu cặp đang vẽ
        }
        // —— KẾT THÚC đường —— (chỉ cho kết thúc khi cùng Id)
        else
        {
            if (cell == startCell) return;
            if (cell.Id != firstPairId) return; // khác đầu kia → không cho kết thúc

            AddPoint(cell);
            CommitCurrentPath();
        }
    }

    public void CellHovered(TilePath cell)
    {
        if (!isDrawing) return;
        if (!cell.IsBlank) return;
        if (cell == cells[^1]) return;


        int dupIndex = cells.FindIndex(c => c == cell);
        if (dupIndex >= 0)
        {
            TraceBack(dupIndex);
            return;
        }

        if (occupiedCells.Contains(cell.Coord)) return;

        AddPoint(cell);
    }

    #endregion

    #region Build line
    void TraceBack(int idx)
    {
        for (int i = points.Count - 1; i > idx; i--)
        {
            currentCells.Remove(cells[i].Coord);
            points.RemoveAt(i);
            cells.RemoveAt(i);
        }

        _currentLine.positionCount = points.Count;
    }
    void StartNewLine(TilePath cell)
    {
        isDrawing = true;
        points.Clear();
        cells.Clear();
        currentCells.Clear();

        Color col = cell.TileColor;
        linePrefab.startColor = col;
        linePrefab.endColor = col;

        _currentLine = Instantiate(linePrefab, cloneTo);
        _currentLine.positionCount = 0;
        _currentLine.enabled = true;


        AddPoint(cell);
    }
    void AddPoint(TilePath cell)
    {
        Vector3 p = CellToWorld(cell) + Vector3.back;
        points.Add(p);
        cells.Add(cell);
        currentCells.Add(cell.Coord);

        _currentLine.positionCount = points.Count;
        _currentLine.SetPosition(points.Count - 1, p);
    }
    void CommitCurrentPath()
    {
        isDrawing = false;
        foreach (var c in currentCells)
            occupiedCells.Add(c);

        OnThisPairCompleted?.Invoke(firstPairId);

        firstPairId = -1;
        _currentLine = null;
        points.Clear();
        cells.Clear();
        currentCells.Clear();
    }
    #endregion

    #region Ultilities functionn
    Vector3 CellToWorld(TilePath cell)
    {
        var rt = (RectTransform)cell.transform;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, rt.position),
            null,
            out Vector3 world);
        return world;
    }
    public void ResetLines()
    {
        foreach (Transform child in cloneTo)
        {
            if (child != linePrefab.transform) Destroy(child.gameObject);
        }

        occupiedCells.Clear();
        currentCells.Clear();
        points.Clear();
        cells.Clear();
        isDrawing = false;
        firstPairId = -1;
    }
    #endregion

}