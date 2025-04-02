using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AldousBroder
{
    private System.Random rand = new System.Random();
    private int width, height, depth;
    private Cell[,,] grid;

    private Vector3Int boxSize;

    public AldousBroder(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;

        boxSize = new Vector3Int(width, height, depth);
    }

    public void GenerateMazeInstant()
    {
        int primarySize = rand.Next(MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize));
        int secondarySize = rand.Next(MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize));

        Cell current = MazeTools.GetCell(primarySize, secondarySize, grid);
        current.visited = true;
        int unvisitedCells = width * height * depth - 1;

        while (unvisitedCells > 0)
        {
            Cell next = GetRandomNeighbor(current);
            if (next != null)
            {
                if (!next.visited)
                {
                    MazeTools.RemoveWallsBetween(current, next);
                    next.visited = true;
                    unvisitedCells--;
                }
                current = next;
            }
        }
        MazeGenerator.Instance.CreateExitPaths();
    }

    private Cell GetRandomNeighbor(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        Vector3Int[] directions =
        {
        MazeTools.CreateDirection(-1, 0, MazeGenerator.Instance.GetDynamicAxes()), // Trái
        MazeTools.CreateDirection(1, 0, MazeGenerator.Instance.GetDynamicAxes()),  // Phải
        MazeTools.CreateDirection(0, -1, MazeGenerator.Instance.GetDynamicAxes()), // Dưới
        MazeTools.CreateDirection(0, 1, MazeGenerator.Instance.GetDynamicAxes())   // Trên
        };

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = new Vector3Int(cell.x, cell.y, cell.z) + dir;
            if (MazeTools.IsInBounds(neighborPos,boxSize))
            {
                Cell neighbor = grid[neighborPos.x, neighborPos.y, neighborPos.z];
                neighbors.Add(neighbor);
            }
        }

        return neighbors.Count > 0 ? neighbors[rand.Next(neighbors.Count)] : null;
    }


}