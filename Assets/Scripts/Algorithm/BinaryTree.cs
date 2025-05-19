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
    /// <summary>
    /// Thuật toán Binary Tree tạo mê cung bằng cách duyệt từng ô theo một trật tự cố định 
    /// (từ trên xuống dưới, từ trái sang phải). 
    /// Mỗi ô sẽ kết nối với ô bên phải hoặc ô bên dưới, 
    /// đảm bảo rằng toàn bộ mê cung có một đường đi duy nhất
    /// </summary>
    public void GenerateMazeInstant()
    {
        int primarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.primary, boxSize);
        int secondarySize = MazeTools.GetSize(MazeGenerator.Instance.GetDynamicAxes().Value.secondary, boxSize);

        for (int primary = 0; primary < primarySize; primary++)
        {
            for (int secondary = 0; secondary < secondarySize; secondary++)
            {
                Cell current = MazeTools.GetCellByAxes(primary, secondary, grid, boxSize);
                
                if (primary == primarySize - 1 && secondary == secondarySize - 1)
                    continue;
                
                bool goPrimary = (primary < primarySize - 1) &&
                                 ((secondary == secondarySize - 1) || rand.Next(2) == 0);

                Cell next = goPrimary ? MazeTools.GetCellByAxes(primary + 1, secondary, grid, boxSize) : MazeTools.GetCellByAxes(primary, secondary + 1, grid, boxSize);
                MazeTools.RemoveWallsBetween(current, next);
            }
        }

        MazeGenerator.Instance.CreateExitPaths();
    }

}
