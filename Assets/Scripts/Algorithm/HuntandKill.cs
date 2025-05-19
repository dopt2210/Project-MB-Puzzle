using UnityEngine;
/// <summary>
/// Thuật toán Hunt and Kill để sinh mê cung.
/// 
/// Ý tưởng: 
/// 1. Chạy theo kiểu "random walk", đi ngẫu nhiên đến các ô chưa thăm.
/// 2. Khi không thể đi tiếp, chuyển sang "hunt mode", tìm một ô đã thăm và chưa kết nối với mê cung, 
/// rồi tiếp tục "random walk" từ đó.
/// 
/// Đặc điểm:
/// - Kết hợp giữa phương pháp tìm kiếm theo chiều sâu (DFS) và tìm kiếm ngẫu nhiên.
/// - Có thể tạo ra mê cung với nhiều ngõ cụt và các vùng không đồng đều.
/// </summary>
public class HuntandKill
{
    private System.Random rand = new System.Random();
    private int width, height, depth;
    private Cell[,,] grid;

    private Vector3Int boxSize;
    private float scale;

    private Cell firstHuntCell = null;
    private Cell farthestCell = null;


    public HuntandKill(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;
        scale = data.cellPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;

        boxSize = new Vector3Int(width, height, depth);
    }

    public void GenerateMazeInstant()
    {
        Cell current = grid[rand.Next(width), rand.Next(height), rand.Next(depth)];
        current.visited = true;
        MazeTools.PlacePuzzle(current, MazeAlgorithmType.HuntandKill, scale, 0);
        while (current != null)
        {
            // Bước 1: Random Walk
            Cell next = MazeTools.GetUnvisitedNeighbor(current, grid, boxSize);
            if (next != null)
            {
                MazeTools.RemoveWallsBetween(current, next);
                current = next;
                current.visited = true;
            }
            else
            {
                // Bước 2: Hunt Mode
                current = HuntForNewStart();
                if (current != null && firstHuntCell == null)
                {
                    firstHuntCell = current;
                    MazeTools.PlacePuzzle(current, MazeAlgorithmType.HuntandKill, scale, 1);
                }
            }
        }
        Vector3Int exit = new Vector3Int(width - 2, height - 1, depth - 2);
        farthestCell = MazeGenerator.grid[exit.x, exit.y, exit.z];
        if (farthestCell != null)
        {
            MazeTools.PlacePuzzle(farthestCell, MazeAlgorithmType.HuntandKill, scale, 2);
        }
        MazeGenerator.Instance.CreateExitPaths();
    }

    private Cell HuntForNewStart()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell cell = grid[x, y, z];
                    if (!cell.visited && MazeTools.HasVisitedNeighbor(cell, grid, boxSize))
                    {
                        cell.visited = true;
                        MazeTools.RemoveWallsBetween(cell, MazeTools.GetVisitedNeighbor(cell, grid, boxSize));
                        return cell;
                    }
                }
            }
        }
        return null;
    }

}