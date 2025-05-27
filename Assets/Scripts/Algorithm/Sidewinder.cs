using System.Collections.Generic;
using UnityEngine;

public class Sidewinder : IMazeGenerator
{
    private System.Random rand = new System.Random();
    private Cell[,,] grid;
    private int width, height, depth;
    private Vector3Int boxSize;
    private float scale;

    public Sidewinder(MazeSO data, float scale)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        boxSize = data.BoxSize;
        this.scale = scale;

        grid = MazeGenerator.MazeGrid;
    }

    public void GenerateMazeInstant()
    {
        int primarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize);
        int secondarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize);

        List<Cell> lastRunSet = null;

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
            // Lưu lại runSet của hàng cuối để sau này chọn ô đặt _puzzleObj
            if (secondary == secondarySize - 1)
            {
                lastRunSet = runSet;
            }
        }
        // Đặt _puzzleObj tại ô đầu của hàng cuối cùng
        if (secondarySize > 0)
        {
            Cell puzzleCellA = MazeTools.GetCellByAxes(0, secondarySize - 1, grid, boxSize);
            MazeTools.PlacePuzzle(puzzleCellA, MazeAlgorithmType.Sidewinder, scale, 0, GameManager.Instance.PoolClone);
        }

        if (lastRunSet != null && lastRunSet.Count > 0)
        {
            Cell puzzleCellB = lastRunSet[rand.Next(lastRunSet.Count)];
            MazeTools.PlacePuzzle(puzzleCellB, MazeAlgorithmType.Sidewinder, scale, 1, GameManager.Instance.PoolClone);

        }

        MazeGenerator.Instance.CreateExitPaths(width, height, depth);
    }

    private Cell GetCell(Cell current, int axis, int offset)
    {
        Vector3Int nextPos = new Vector3Int(current.x, current.y, current.z);
        nextPos[axis] += offset;

        return MazeTools.IsInBounds(nextPos, boxSize) ? grid[nextPos.x, nextPos.y, nextPos.z] : null;
    }
}
