using System.Collections.Generic;
using UnityEngine;

public class Eller
{
    private System.Random rand = new System.Random();
    private int width, height, depth;
    private Cell[,,] grid;
    private Vector3Int boxSize;

    public Eller(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;
        boxSize = new Vector3Int(width, height, depth);
    }

    public void GenerateMazeInstant()
    {
        DynamicAxes? axes = MazeGenerator.Instance.GetDynamicAxes();
        if (!axes.HasValue)
        {
            Debug.LogError("Không thể xác định trục động!");
            return;
        }

        int primaryAxisSize = MazeTools.GetSize(axes.Value.primary, boxSize);
        int secondaryAxisSize = MazeTools.GetSize(axes.Value.secondary, boxSize);

        Dictionary<Vector2Int, int> cellSet = new Dictionary<Vector2Int, int>();
        int nextSetId = 1;

        for (int primary = 0; primary < primaryAxisSize; primary++)
        {
            // Gán set ID cho từng ô nếu chưa có
            for (int secondary = 0; secondary < secondaryAxisSize; secondary++)
            {
                Vector2Int cellKey = new Vector2Int(primary, secondary);
                if (!cellSet.ContainsKey(cellKey))
                {
                    cellSet[cellKey] = nextSetId++;
                }
            }

            // Hợp nhất ngẫu nhiên các ô ngang cùng hàng
            for (int secondary = 0; secondary < secondaryAxisSize - 1; secondary++)
            {
                Vector2Int currentKey = new Vector2Int(primary, secondary);
                Vector2Int nextKey = new Vector2Int(primary, secondary + 1);

                if (rand.Next(2) == 0 && cellSet[currentKey] != cellSet[nextKey])
                {
                    Cell current = MazeTools.GetCell(primary, secondary, grid);
                    Cell next = MazeTools.GetCell(primary, secondary + 1, grid);

                    MazeTools.RemoveWallsBetween(current, next);

                    int oldSet = cellSet[nextKey];
                    int newSet = cellSet[currentKey];

                    foreach (var key in new List<Vector2Int>(cellSet.Keys))
                    {
                        if (cellSet[key] == oldSet)
                        {
                            cellSet[key] = newSet;
                        }
                    }
                }
            }

            // Tạo liên kết dọc
            Dictionary<int, List<Vector2Int>> setsInRow = new Dictionary<int, List<Vector2Int>>();

            for (int secondary = 0; secondary < secondaryAxisSize; secondary++)
            {
                Vector2Int cellKey = new Vector2Int(primary, secondary);
                int setId = cellSet[cellKey];

                if (!setsInRow.ContainsKey(setId))
                {
                    setsInRow[setId] = new List<Vector2Int>();
                }
                setsInRow[setId].Add(cellKey);
            }

            if (primary < primaryAxisSize - 1)
            {
                foreach (var set in setsInRow)
                {
                    bool hasConnected = false;

                    foreach (var cellKey in set.Value)
                    {
                        if (rand.Next(2) == 0 || !hasConnected)
                        {
                            Cell current = MazeTools.GetCell(cellKey.x, cellKey.y, grid);
                            Cell below = MazeTools.GetCell(cellKey.x + 1, cellKey.y, grid);

                            MazeTools.RemoveWallsBetween(current, below);
                            cellSet[new Vector2Int(cellKey.x + 1, cellKey.y)] = set.Key;
                            hasConnected = true;
                        }
                    }
                }
            }
        }

        MazeGenerator.Instance.CreateExitPaths();
    }
}
