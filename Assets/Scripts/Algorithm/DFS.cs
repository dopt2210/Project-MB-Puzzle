using System.Collections.Generic;
using UnityEngine;

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
