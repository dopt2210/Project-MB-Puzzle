using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Thuật toán Depth-First Search (DFS) để sinh mê cung.
/// 
/// Ý tưởng: Bắt đầu từ một ô ban đầu, đi sâu liên tục vào ô chưa thăm,
/// mỗi lần đi sẽ phá tường giữa ô hiện tại và ô tiếp theo.
/// Nếu không còn ô chưa thăm lân cận, quay lui (backtrack) để tìm ô khác.
/// 
/// Đặc điểm:
/// - Sinh ra mê cung dạng "ống" nhiều ngõ cụt.
/// - Rất nhanh và đơn giản.
/// </summary>
public class DFS: IMazeGenerator
{
    private List<Cell> stack = new List<Cell>();

    private MazeKeyPoint keyPoint;
    private Cell[,,] grid;
    private int width, height, depth;
    private Vector3Int boxSize;
    private float scale;

    public DFS(MazeSO data, float scale)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        boxSize = data.BoxSize;
        this.scale = scale;

        grid = MazeGenerator.MazeGrid;
        keyPoint = MazeGenerator.KeyPoint;
    }

    public void GenerateMazeInstant()
    {
        Cell startCell = grid[0, 0, 0];
        startCell.visited = true;
        stack.Add(startCell);

        var puzzleCell = grid[1, 0, 1];
        keyPoint.AddKeyPoint(MazeAlgorithmType.DFS, puzzleCell);
        MazeTools.PlacePuzzle(puzzleCell, MazeAlgorithmType.DFS, scale, 0, GameManager.Instance.PoolClone);
        var puzzleCell2 = startCell;
        int maxDistance = 0;

        while (stack.Count > 0)
        {
            Cell current = stack[stack.Count - 1];
            Cell next = MazeTools.GetUnvisitedNeighbor(current, grid, boxSize);

            if (next != null)
            {
                stack.Add(next);
                next.visited = true;
                MazeTools.RemoveWallsBetween(current, next);
            }
            else
            {
                if (stack.Count > 1 && MazeTools.GetUnvisitedNeighbor(current, grid, boxSize) == null)
                {
                    int dist = stack.Count;
                    if (dist > maxDistance)
                    {
                        maxDistance = dist;
                        puzzleCell2 = current;
                    }
                }

                stack.RemoveAt(stack.Count - 1);
            }
        }
        keyPoint.AddKeyPoint(MazeAlgorithmType.DFS, puzzleCell2);
        MazeTools.PlacePuzzle(puzzleCell2, MazeAlgorithmType.DFS, scale, 1, GameManager.Instance.PoolClone);

        MazeGenerator.Instance.CreateExitPaths(width, height, depth);
    }
}
