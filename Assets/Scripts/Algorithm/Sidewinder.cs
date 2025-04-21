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
                Cell current = MazeTools.GetCellByAxes(primary, secondary, grid, boxSize);
                if (current == null) continue;

                runSet.Add(current);

                bool atPrimaryEdge = (primary == primarySize - 1);
                bool atSecondaryEdge = (secondary == 0);

                if (atPrimaryEdge || (!atSecondaryEdge && rand.Next(2) == 0))
                {
                    // Chọn một ô ngẫu nhiên trong tập hợp và mở đường lên trên (hoặc trục tương ứng)
                    Cell chosenCell = runSet[rand.Next(runSet.Count)];
                    Cell next = GetCell(chosenCell, MazeGenerator.Instance.GetDynamicAxes().Value.secondary, -1);

                    if (next != null)
                    {
                        MazeTools.RemoveWallsBetween(chosenCell, next);
                    }

                    runSet.Clear(); // Bắt đầu một nhóm mới
                }
                else
                {
                    // Nếu không, mở đường theo trục chính (qua phải hoặc xuống)
                    Cell next = GetCell(current, MazeGenerator.Instance.GetDynamicAxes().Value.primary, +1);
                    if (next != null)
                    {
                        MazeTools.RemoveWallsBetween(current, next);
                    }
                }
            }
        }

        MazeGenerator.Instance.CreateExitPaths();
    }

    private Cell GetCell(Cell current, int axis, int offset)
    {
        Vector3Int nextPos = new Vector3Int(current.x, current.y, current.z);
        nextPos[axis] += offset;

        return MazeTools.IsInBounds(nextPos, boxSize) ? grid[nextPos.x, nextPos.y, nextPos.z] : null;
    }
}
