using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Thuật toán Randomized Kruskal để sinh mê cung.
/// 
/// Ý tưởng:
/// 1. Bắt đầu với tất cả các cạnh giữa các ô trong mê cung.
/// 2. Xáo trộn các cạnh và áp dụng thuật toán Kruskal để chọn các cạnh tạo thành mê cung.
/// 3. Các cạnh được thêm vào mê cung chỉ khi chúng không tạo ra chu trình, tức là kết nối các ô chưa thuộc cùng một tập hợp.
/// 
/// Đặc điểm:
/// - Thuật toán này sử dụng phương pháp Union-Find để kiểm tra chu trình.
/// - Tạo ra mê cung với các đường đi không đồng đều và nhiều ngõ cụt.
public class RandomKruskal : IMazeGenerator
{
    private System.Random rand = new System.Random();
    private List<Edge> edges = new List<Edge>();
    private Dictionary<Cell, Cell> parent = new Dictionary<Cell, Cell>();
    private Dictionary<Cell, int> rank = new Dictionary<Cell, int>();
    private List<Cell> carvedOrder = new List<Cell>();
    private HashSet<Cell> carvedSeen = new HashSet<Cell>();

    private Cell[,,] grid;
    private int width, height, depth;
    private Vector3Int boxSize;
    private float scale;

    public RandomKruskal(MazeSO data, float scale)
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
        // Khởi tạo tập hợp cha của mỗi ô
        foreach (var cell in grid)
        {
            parent[cell] = cell;
            rank[cell] = 0;
        }

        // Thêm cạnh hợp lệ theo trục động
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Cell current = grid[x, y, z];
                    foreach (var dir in MazeTools.GetValidDirections())
                    {
                        Vector3Int neighborPos = new Vector3Int(x, y, z) + dir;
                        if (MazeTools.IsInBounds(neighborPos, boxSize))
                        {
                            Cell neighbor = grid[neighborPos.x, neighborPos.y, neighborPos.z];
                            edges.Add(new Edge(current, neighbor));
                        }
                    }
                }
            }
        }

        // Xáo trộn danh sách cạnh
        edges = Shuffle(edges);

        // Áp dụng thuật toán Kruskal
        foreach (var edge in edges)
        {
            if (Find(edge.cellA) != Find(edge.cellB))
            {
                Union(edge.cellA, edge.cellB);
                MazeTools.RemoveWallsBetween(edge.cellA, edge.cellB);
                if (!carvedSeen.Contains(edge.cellA)) { carvedSeen.Add(edge.cellA); carvedOrder.Add(edge.cellA); }
                if (!carvedSeen.Contains(edge.cellB)) { carvedSeen.Add(edge.cellB); carvedOrder.Add(edge.cellB); }
            }
        }

        // Đặt 3 puzzle dựa trên thứ tự khắc đường (carvedOrder)
        if (carvedOrder.Count >= 3)
        {
            var axes = MazeGenerator.Instance.GetDynamicAxes().Value;
            int secondaryAxis = axes.secondary;

            Cell puzzle1 = carvedOrder[Mathf.Min(1, carvedOrder.Count - 1)];
            Cell puzzle2 = carvedOrder[carvedOrder.Count / 2];
            Cell puzzle3 = carvedOrder[0];
            int minSec = MazeTools.GetAxisValue(secondaryAxis, puzzle3);
            foreach (var c in carvedOrder)
            {
                int sec = MazeTools.GetAxisValue(secondaryAxis, c);
                if (sec < minSec)
                {
                    minSec = sec;
                    puzzle3 = c;
                }
            }

            MazeTools.PlacePuzzle(puzzle1, MazeAlgorithmType.RandomKruskal, scale, 0, GameManager.Instance.PoolClone);
            MazeTools.PlacePuzzle(puzzle2, MazeAlgorithmType.RandomKruskal, scale, 1, GameManager.Instance.PoolClone);
            MazeTools.PlacePuzzle(puzzle3, MazeAlgorithmType.RandomKruskal, scale, 2, GameManager.Instance.PoolClone);
        }

        MazeGenerator.Instance.CreateExitPaths(width, height, depth);
    }

    private Cell Find(Cell cell)
    {
        if (parent[cell] != cell)
            parent[cell] = Find(parent[cell]); // Path compression
        return parent[cell];
    }

    private void Union(Cell a, Cell b)
    {
        Cell rootA = Find(a);
        Cell rootB = Find(b);

        if (rootA != rootB)
        {
            if (rank[rootA] > rank[rootB])
                parent[rootB] = rootA;
            else if (rank[rootA] < rank[rootB])
                parent[rootA] = rootB;
            else
            {
                parent[rootB] = rootA;
                rank[rootA]++;
            }
        }
    }

    private List<Edge> Shuffle(List<Edge> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

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
