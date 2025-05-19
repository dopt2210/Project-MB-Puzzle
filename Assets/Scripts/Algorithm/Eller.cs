using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Thuật toán Eller để sinh mê cung.
/// 
/// Ý tưởng:
/// 1. Sử dụng thuật toán Eller để tạo ra mê cung với các ô liên kết theo chiều ngang và dọc.
/// 2. Thuật toán sẽ chia mê cung thành các tập hợp và kết nối các ô trong các tập hợp đó.
/// 3. Kết nối các ô theo chiều ngang và chiều dọc, đồng thời quản lý các tập hợp thông qua một từ điển.
/// 4. Sử dụng xác suất ngẫu nhiên để quyết định việc kết nối các ô.
/// 5. Thuật toán tiếp tục cho đến khi toàn bộ mê cung được tạo ra.
public class Eller
{
    private int width, height, depth;
    private Cell[,,] grid;
    private Dictionary<int, List<Cell>> sets;
    private int nextSetID = 1;
    private float randomSeed = 0.5f;
    private Vector3Int boxSize;

    public Eller(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;

        boxSize = new Vector3Int(width, height, depth);
        sets = new Dictionary<int, List<Cell>>();
    }

    public void GenerateMazeInstant()
    {
        int primarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize);
        int secondarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize);

        for (int secondary = 0; secondary < secondarySize; secondary++)
        {
            AssignSets(secondary, primarySize);
            ConnectHorizontal(secondary, primarySize);
            if (secondary < secondarySize - 1)
            {
                CreateVertical(secondary, primarySize, secondarySize);
            }
        }

        ConnectLast(primarySize, secondarySize);
        MazeGenerator.Instance.CreateExitPaths();
    }

    private void AssignSets(int secondary, int primarySize)
    {
        Debug.Log($"Hàng {secondary}: Gán tập hợp cho các ô");

        for (int primary = 0; primary < primarySize; primary++)
        {
            Cell cell = MazeTools.GetCellByAxes(primary, secondary, grid, boxSize);
            if (cell.setID == 0)
            {
                cell.setID = nextSetID++;
            }

            if (!sets.ContainsKey(cell.setID))
            {
                sets[cell.setID] = new List<Cell>();
            }
            sets[cell.setID].Add(cell);
            Debug.Log($"Cell ({cell.x}, {cell.y}, {cell.z}) - SetID: {cell.setID}");
        }
    }


    private void ConnectHorizontal(int secondary, int primarySize)
    {
        for (int primary = 0; primary < primarySize - 1; primary++)
        {
            Cell current = MazeTools.GetCellByAxes(primary, secondary, grid, boxSize);
            Cell next = MazeTools.GetCellByAxes(primary + 1, secondary, grid, boxSize);

            if (current.setID != next.setID && Random.value > randomSeed)
            {
                MergeSets(current, next);
            }
        }

        Debug.Log($"Sau khi kết nối ngang, số set còn lại: {sets.Count}");
    }


    private void CreateVertical(int secondary, int primarySize, int secondarySize)
    {
        Debug.Log($"Hàng {secondary}: Tạo kết nối dọc xuống dưới");
        HashSet<int> processedSets = new HashSet<int>();

        int secondaryAxis = MazeGenerator.Instance.GetDynamicAxes().Value.secondary;

        for (int primary = 0; primary < primarySize; primary++)
        {
            Cell cell = MazeTools.GetCellByAxes(primary, secondary, grid, boxSize);

            if (!processedSets.Contains(cell.setID))
            {
                processedSets.Add(cell.setID);
                List<Cell> setCells = sets[cell.setID].FindAll(c => MazeTools.GetAxisValue(secondaryAxis, c) == secondary);

                int count = 0;
                
                foreach (var current in setCells)
                {
                    if (count == 0 || Random.value > randomSeed)
                    {
                        if (secondary + 1 < secondarySize)
                        {
                            int primaryCoord = MazeTools.GetAxisValue(MazeGenerator.Instance.GetDynamicAxes().Value.primary, current);
                            Cell below = MazeTools.GetCellByAxes(primaryCoord, secondary + 1, grid, boxSize);

                            if (below != null)
                            {
                                MazeTools.RemoveWallsBetween(current, below);
                                Debug.Log($"Xóa tường xuống Current({current.x},{current.y},{current.z}) và Below({below.x},{below.y},{below.z})");
                                below.setID = current.setID;
                                count++;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"=== Danh sách sets sau khi kết nối dọc ở hàng {secondary} ===");
        foreach (var set in sets)
        {
            string cellList = string.Join(", ", set.Value.ConvertAll(cell => $"({cell.x}, {cell.y}, {cell.z})"));
            Debug.Log($"Set {set.Key}: {cellList}");
        }
    }


    private void ConnectLast(int primarySize, int secondarySize)
    {
        Debug.Log("Kết nối các ô còn lại");

        for (int primary = 0; primary < primarySize - 1; primary++)
        {
            Cell current = MazeTools.GetCellByAxes(primary, secondarySize - 1, grid, boxSize);
            Cell next = MazeTools.GetCellByAxes(primary + 1, secondarySize - 1, grid, boxSize);

            if (current.setID != next.setID)
            {
                MazeTools.RemoveWallsBetween(current, next);
                MergeSets(current, next);
            }

        }

        Debug.Log($"Số tập hợp sau khi hoàn thành: {sets.Count}");
        foreach (var set in sets)
        {
            Debug.Log($"Set {set.Key} chứa {set.Value.Count} ô.");
        }
    }

    private void MergeSets(Cell current, Cell next)
    {
        MazeTools.RemoveWallsBetween(current, next);
        int oldSet = next.setID;

        if (!sets.ContainsKey(oldSet)) return;

        if (!sets.ContainsKey(current.setID))
        {
            sets[current.setID] = new List<Cell>();
        }

        List<Cell> oldCells = sets[oldSet];

        foreach (var cell in oldCells)
        {
            cell.setID = current.setID;
            sets[current.setID].Add(cell);
        }

        sets.Remove(oldSet); 
    }
}
