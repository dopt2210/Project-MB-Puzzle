using System.Collections.Generic;
using UnityEngine;

public class Sidewinder
{
    private System.Random rand = new System.Random();
    private Cell[,,] grid;
    private int width, height, depth;
    private Vector3Int boxSize;

    public Sidewinder(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;

        boxSize = new Vector3Int(width, height, depth);
    }

    public void GenerateMazeInstant()
    {
        int primarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize);
        int secondarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize);

        for (int secondary = 0; secondary < secondarySize; secondary++)
        {
            List<Cell> runSet = new List<Cell>();

            for (int primary = 0; primary < primarySize; primary++)
            {
                Cell current = MazeTools.GetCell(primary, secondary, grid);
                if (current == null) continue;

                runSet.Add(current);

                bool atPrimaryEdge = (primary == primarySize - 1);
                bool atSecondaryEdge = (secondary == 0);

                if (atPrimaryEdge || (!atSecondaryEdge && rand.Next(2) == 0))
                {
                    // Chọn một ô ngẫu nhiên trong tập hợp và mở đường lên trên (hoặc trục tương ứng)
                    Cell chosenCell = runSet[rand.Next(runSet.Count)];
                    Cell next = GetCell(chosenCell.x, chosenCell.y, chosenCell.z, true, primarySize);

                    if (next != null)
                    {
                        MazeTools.RemoveWallsBetween(chosenCell, next);
                    }

                    runSet.Clear(); // Bắt đầu một nhóm mới
                }
                else
                {
                    // Nếu không, mở đường theo trục chính (qua phải hoặc xuống)
                    Cell next = GetCell(current.x, current.y, current.z, false, primarySize);
                    if (next != null)
                    {
                        MazeTools.RemoveWallsBetween(current, next);
                    }
                }
            }
        }

        MazeGenerator.Instance.CreateExitPaths();
    }

    private Cell GetCell(int x, int y, int z, bool moveUp, int primarySize)
    {
        int primaryProperty = MazeGenerator.Instance.GetDynamicAxes().Value.primary;
        int secondaryProperty = MazeGenerator.Instance.GetDynamicAxes().Value.secondary;

        if (moveUp)
        {
            if (secondaryProperty == 0 && x > 0) return grid[x - 1, y, z];
            if (secondaryProperty == 1 && y > 0) return grid[x, y - 1, z];
            if (secondaryProperty == 2 && z > 0) return grid[x, y, z - 1];
        }
        else
        {
            if (primaryProperty == 0 && x < primarySize - 1) return grid[x + 1, y, z];
            if (primaryProperty == 1 && y < primarySize - 1) return grid[x, y + 1, z];
            if (primaryProperty == 2 && z < primarySize - 1) return grid[x, y, z + 1];
        }

        return null;
    }
}
