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
        UIDebug.Instance.UpdateAlgo("Aldous - Broder");
        UIInformation.Instance.UpdateLevel(4);
        int primarySize = rand.Next(MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize));
        int secondarySize = rand.Next(MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize));

        Cell current = MazeTools.GetCellByAxes(primarySize, secondarySize, grid, boxSize);
        current.visited = true;
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
                }
                current = next;
            }
        }
        MazeGenerator.Instance.CreateExitPaths();
    }
}