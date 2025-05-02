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
public class DFS
{
    private List<Cell> stack = new List<Cell>();
    private Cell[,,] grid;
    
    private int width, height, depth;
    private Vector3Int boxSize;

    public DFS(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;

        boxSize = new Vector3Int(width, height, depth);
    }

    public void GenerateMazeInstant()
    {
        Cell startCell = grid[0, 0, 0];
        startCell.visited = true;
        stack.Add(startCell);

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
                stack.RemoveAt(stack.Count - 1);
            }
        }

        MazeGenerator.Instance.CreateExitPaths();
    }
}
