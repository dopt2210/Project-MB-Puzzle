using UnityEngine;

public class HuntandKill
{
    private System.Random rand = new System.Random();
    private int width, height, depth;
    private Cell[,,] grid;

    private Vector3Int boxSize;

    public HuntandKill(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;

        boxSize = new Vector3Int(width, height, depth);
    }

    public void GenerateMazeInstant()
    {
        UIDebug.Instance.UpdateAlgo("Hunt And Kill");
        UIInformation.Instance.UpdateLevel(5);
        Cell current = grid[rand.Next(width), rand.Next(height), rand.Next(depth)];
        current.visited = true;

        while (true)
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
                if (current == null) break;
            }
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