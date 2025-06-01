using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Thuật toán Aldous-Broder để sinh mê cung.
/// 
/// Ý tưởng: Bắt đầu từ một ô ngẫu nhiên, liên tục đi ngẫu nhiên đến một ô lân cận.
/// Nếu ô đó chưa được thăm, phá vỡ tường giữa hai ô.
/// Tiếp tục quá trình cho đến khi tất cả các ô đã được thăm.
/// 
/// Đặc điểm:
/// - Đảm bảo toàn bộ mê cung kết nối.
/// - Không thiên vị, nhưng rất kém hiệu quả (rất nhiều bước thừa).
/// </summary>
public class AldousBroder : IMazeGenerator
{
    private System.Random rand = new System.Random();
    private Cell farCorner, centerish;
    private List<Cell> visitedCells = new List<Cell>();

    private Cell[,,] grid;
    private int width, height, depth;
    private Vector3Int boxSize;
    private float scale;

    public AldousBroder(MazeSO data, float scale)
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
        int primarySize = rand.Next(MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize));
        int secondarySize = rand.Next(MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize));

        Cell current = MazeTools.GetCellByAxes(primarySize, secondarySize, grid, boxSize);
        current.visited = true;
        visitedCells.Add(current);
        int unvisitedCells = width * height * depth - 1;

        while (unvisitedCells > 0)
        {
            Cell next = MazeTools.GetRandomNeighbor(current, grid, boxSize);
            if (next != null)
            {
                if (!next.visited)
                {
                    MazeTools.RemoveWallsBetween(current, next);
                    next.visited = true;
                    unvisitedCells--;
                    visitedCells.Add(next);
                }
                current = next;
            }
        }
        if (visitedCells.Count >= 3)
        {
            farCorner = visitedCells[1]; // Vị trí xa đầu tiên
            centerish = visitedCells[visitedCells.Count / 2]; // Gần trung tâm lộ trình
            MazeTools.PlacePuzzle(farCorner, MazeAlgorithmType.AldousBroder, scale, 0, GameManager.Instance.PoolClone);
            MazeTools.PlacePuzzle(centerish, MazeAlgorithmType.AldousBroder, scale, 1, GameManager.Instance.PoolClone);
        }
        MazeGenerator.Instance.CreateExitPaths(width, height, depth);
    }
}