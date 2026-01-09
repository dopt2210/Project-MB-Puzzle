using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Thuật toán Randomized Prim's để sinh mê cung.
/// 
/// Ý tưởng:
/// 1. Bắt đầu từ một ô ngẫu nhiên và đánh dấu là đã thăm.
/// 2. Thêm các cạnh của ô bắt đầu vào danh sách frontier.
/// 3. Tiến hành mở rộng mê cung bằng cách chọn một cạnh ngẫu nhiên từ frontier và kiểm tra các ô liền kề chưa được thăm.
/// 4. Nếu ô liền kề chưa được thăm, xóa tường giữa hai ô và thêm các cạnh của ô đó vào frontier.
/// 5. Lặp lại quá trình cho đến khi toàn bộ mê cung được tạo ra.
/// 
/// Đặc điểm:
/// - Thuật toán này tạo ra mê cung với đường đi rộng rãi và không có chu trình.
/// - Phù hợp để tạo ra các mê cung với nhiều nhánh và ít ngõ cụt.
public class RandomPrims : IMazeGenerator
{
    private System.Random rand = new System.Random();
    private List<Edge> frontier = new List<Edge>(); // List các cạnh ngẫu nhiên cần kiểm tra
    private List<Cell> visitedOrder = new List<Cell>();
    
    private Cell[,,] grid;
    private int width, height, depth;
    private Vector3Int boxSize;
    private float scale;

    public RandomPrims(MazeSO data, float scale)
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
        // Bước 1: Bắt đầu từ một cell ngẫu nhiên
        Cell startCell = grid[rand.Next(width), rand.Next(height), rand.Next(depth)];
        startCell.visited = true;
        visitedOrder.Add(startCell);

        // Bước 2: Thêm các cạnh của startCell vào danh sách frontier
        AddFrontierCells(startCell);

        // Bước 3: Tiến hành thuật toán Prim's cho đến khi toàn bộ mê cung được tạo ra
        while (frontier.Count > 0)
        {
            // Chọn một cạnh ngẫu nhiên từ frontier
            Edge edge = frontier[rand.Next(frontier.Count)];
            frontier.Remove(edge);

            Cell current = edge.cellA;
            Cell neighbor = edge.cellB;

            // Nếu neighbor chưa được thăm, thì mở rộng mê cung
            if (!neighbor.visited)
            {
                MazeTools.RemoveWallsBetween(current, neighbor);
                neighbor.visited = true;
                visitedOrder.Add(neighbor);

                // Thêm các cell xung quanh neighbor vào frontier
                AddFrontierCells(neighbor);
            }
        }

        // Đặt 3 puzzle dựa trên lộ trình đã mở rộng (visitedOrder)
        if (visitedOrder.Count >= 3)
        {
            var axes = MazeGenerator.Instance.GetDynamicAxes().Value;
            int secondaryAxis = axes.secondary;

            Cell puzzle1 = visitedOrder[Mathf.Min(1, visitedOrder.Count - 1)]; // gần khởi đầu, hướng theo đường chính
            Cell puzzle2 = visitedOrder[visitedOrder.Count / 2]; // giữa nhánh chính
            // chọn ô có tọa độ nhỏ nhất theo trục phụ ("đi xuống dưới cùng")
            Cell puzzle3 = visitedOrder[0];
            int minSec = MazeTools.GetAxisValue(secondaryAxis, puzzle3);
            foreach (var c in visitedOrder)
            {
                int sec = MazeTools.GetAxisValue(secondaryAxis, c);
                if (sec < minSec)
                {
                    minSec = sec;
                    puzzle3 = c;
                }
            }

            MazeTools.PlacePuzzle(puzzle1, MazeAlgorithmType.RandomPrims, scale, 0, GameManager.Instance.PoolClone);
            MazeTools.PlacePuzzle(puzzle2, MazeAlgorithmType.RandomPrims, scale, 1, GameManager.Instance.PoolClone);
            MazeTools.PlacePuzzle(puzzle3, MazeAlgorithmType.RandomPrims, scale, 2, GameManager.Instance.PoolClone);
        }

        // Bước 4: Tạo đường ra cho mê cung
        MazeGenerator.Instance.CreateExitPaths(width, height, depth);
    }

    // Thêm các cạnh từ các cell chưa được thăm vào danh sách frontier
    private void AddFrontierCells(Cell cell)
    {
        foreach (var dir in MazeTools.GetValidDirections())
        {
            Vector3Int neighborPos = new Vector3Int(cell.x, cell.y, cell.z) + dir;
            if (MazeTools.IsInBounds(neighborPos, boxSize))
            {
                Cell neighbor = grid[neighborPos.x, neighborPos.y, neighborPos.z];
                if (!neighbor.visited)
                {
                    // Thêm cạnh vào danh sách frontier nếu neighbor chưa được thăm
                    frontier.Add(new Edge(cell, neighbor));
                }
            }
        }
    }

    // Cấu trúc để lưu các cạnh giữa hai cell
    private class Edge
    {
        public Cell cellA, cellB;
        public Edge(Cell a, Cell b)
        {
            cellA = a;
            cellB = b;
        }
    }
}
