using UnityEngine;

public class BinaryTree
{
    private System.Random rand = new System.Random();
    private Cell[,,] grid;
    private int width, height, depth;

    Vector3Int boxSize;

    public BinaryTree(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;

        boxSize = new Vector3Int(width, height, depth);
    }

    public void GenerateMazeInstant()
    {
        int primarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize);
        int secondarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize);

        for (int primary = 0; primary < primarySize; primary++)
        {
            for (int secondary = 0; secondary < secondarySize; secondary++)
            {
                Cell current = MazeTools.GetCell(primary, secondary, grid);

                if (primary == primarySize - 1 && secondary == secondarySize - 1)
                    continue;

                bool goPrimary = (primary < primarySize - 1) &&
                                 ((secondary == secondarySize - 1) || rand.Next(2) == 0);

                Cell next = goPrimary ? MazeTools.GetCell(primary + 1, secondary, grid) : MazeTools.GetCell(primary, secondary + 1, grid);
                MazeTools.RemoveWallsBetween(current, next);
            }
        }

        MazeGenerator.Instance.CreateExitPaths();
    }

}
