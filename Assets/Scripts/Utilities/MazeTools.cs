using System.Collections.Generic;
using UnityEngine;

public static class MazeTools
{
    private static System.Random rand = new System.Random();
    /// <summary>
    /// Xóa hai bức tường kề nhau giữa hai ô liền kề.
    /// </summary>
    /// <remarks>
    /// Nếu một trong hai ô là null, phương thức sẽ log lỗi và không thực hiện thao tác.
    /// </remarks>
    public static void RemoveWallsBetween(Cell current, Cell next)
    {
        if (current == null || next == null)
        {
            Debug.LogError("Lỗi: Một trong hai ô Cell bị null khi xóa tường!");
            return;
        }

        Vector3Int direction = new Vector3Int(next.x - current.x, next.y - current.y, next.z - current.z);

        current.RemoveWall(direction);
        next.RemoveWall(-direction);
    }
    /// <summary>
    /// Kiểm tra xem có tồn tại tường giữa hai ô kề nhau hay không.
    /// </summary>
    /// <returns>Trả về `true` nếu có tường giữa hai ô, `false` nếu không có tường.</returns>
    public static bool HasWallBetween(Cell currentCell, Cell nextCell)
    {
        Vector3Int direction = new Vector3Int(nextCell.x - currentCell.x, nextCell.y - currentCell.y, nextCell.z - currentCell.z);

        if (direction == Vector3Int.right) return currentCell.rightWall.activeSelf;
        if (direction == Vector3Int.left) return currentCell.leftWall.activeSelf;
        if (direction == Vector3Int.up) return currentCell.topWall.activeSelf;
        if (direction == Vector3Int.down) return currentCell.bottomWall.activeSelf;
        if (direction == Vector3Int.forward) return currentCell.frontWall.activeSelf;
        if (direction == Vector3Int.back) return currentCell.backWall.activeSelf;

        return true; 
    }
    /// <summary>
    /// Xác định và trả về hai trục thay đổi trong không gian ba chiều (X, Y, Z) dựa trên các tham số chiều dài của mê cung.
    /// Nếu chiều rộng bằng 0, trả về trục Y-Z.
    /// Nếu chiều cao bằng 0, trả về trục X-Z.
    /// Nếu chiều sâu bằng 0, trả về trục X-Y.
    /// </summary>
    /// <returns>Hai trục thay đổi dưới dạng một đối tượng DynamicAxes hoặc null nếu không có hai trục thay đổi.</returns>
    public static DynamicAxes? IdentifyDynamicAxes(int width, int height, int depth)
    {
        List<int> axes = new List<int>();

        if (width > 1) axes.Add(0);  // Trục X
        if (height > 1) axes.Add(1); // Trục Y
        if (depth > 1) axes.Add(2);  // Trục Z

        if (axes.Count != 2)
        {
            Debug.LogError("Lỗi: Mê cung phải có đúng 2 trục thay đổi!");
            return null;
        }

        return new DynamicAxes(axes[0], axes[1]);
    }
    /// <summary>
    /// Trả về hướng của trục cố định trong không gian ba chiều (X, Y, Z), dựa trên các kích thước chiều rộng, chiều cao và chiều sâu của mê cung.
    /// - Nếu chiều rộng bằng 1, trục X sẽ cố định và trả về hướng Vector3Int.right.
    /// - Nếu chiều cao bằng 1, trục Y sẽ cố định và trả về hướng Vector3Int.up.
    /// - Nếu chiều sâu bằng 1, trục Z sẽ cố định và trả về hướng Vector3Int.forward.
    /// </summary>
    /// <returns>Hướng của trục cố định.</returns>
    public static Vector3Int GetFixedAxis(int width, int height, int depth)
    {
        if (width == 1) return Vector3Int.right;    // X cố định
        if (height == 1) return Vector3Int.up;      // Y cố định
        if (depth == 1) return Vector3Int.forward;  // Z cố định

        Debug.LogError("Lỗi: Không xác định được trục cố định!");
        return Vector3Int.zero; 
    }
    /// <summary>
    /// Lấy Renderer của bức tường nằm ở hướng đối diện với trục `oppositeAxis` từ ô `cell`.
    /// </summary>
    /// <returns>Renderer của bức tường trong ô nếu tồn tại, hoặc null nếu không tìm thấy.</returns>
    public static GameObject GetWallRenderer(Cell cell, Vector3Int oppositeAxis)
    {
        if (oppositeAxis == Vector3Int.right && cell.leftWall != null)
            return cell.leftWall;

        if (oppositeAxis == Vector3Int.up && cell.bottomWall != null)
            return cell.bottomWall;

        if (oppositeAxis == Vector3Int.forward && cell.backWall != null)
            return cell.backWall;

        return null;
    }
    /// <summary>
    /// Tạo và trả về một hướng (Vector3Int) dựa trên các offset và các trục động.
    /// </summary>
    /// <param name="primaryOffset">Offset cho trục chính.</param>
    /// <param name="secondaryOffset">Offset cho trục phụ.</param>
    /// <param name="dynamicAxes">Trục động xác định các trục có thể thay đổi.</param>
    /// <returns>Vector3Int đại diện cho hướng được tạo ra từ các offset và trục động.</returns>
    /// <exception cref="System.ArgumentException">Thrown khi dynamicAxes có giá trị null.</exception>
    /// <remarks>
    /// Phương thức này sử dụng dynamicAxes để xác định các trục có thể thay đổi và sử dụng các offset
    /// để tạo ra một hướng trong không gian 3D.
    /// </remarks>
    public static Vector3Int CreateDirection(int primaryOffset, int secondaryOffset, DynamicAxes? dynamicAxes)
    {
        if (!dynamicAxes.HasValue)
        {
            throw new System.ArgumentException("Lỗi: dynamicAxes không được null!");
        }

        int[] offsets = new int[3];
        offsets[dynamicAxes.Value.primary] = primaryOffset;
        offsets[dynamicAxes.Value.secondary] = secondaryOffset;
        return new Vector3Int(offsets[0], offsets[1], offsets[2]);
    }
    /// <summary>
    /// Lấy danh sách các ô láng giềng của ô hiện tại trong lưới thỏa mãn điều kiện xác định.
    /// </summary>
    /// <param name="cell">Ô hiện tại trong lưới.</param>
    /// <param name="grid">Lưới các ô (Cell[,,]).</param>
    /// <param name="boxSize">Kích thước của lưới trong không gian (chiều dài, chiều rộng, chiều cao).</param>
    /// <param name="condition">Điều kiện được áp dụng cho mỗi ô láng giềng. Nếu null, tất cả các ô láng giềng sẽ được thêm vào.</param>
    /// <returns>Danh sách các ô láng giềng thỏa mãn điều kiện.</returns>
    /// <remarks>
    /// Phương thức này duyệt qua tất cả các ô láng giềng hợp lệ của ô hiện tại và kiểm tra xem chúng có thỏa mãn
    /// điều kiện đã được cung cấp hay không (nếu điều kiện không phải là null). Các ô thỏa mãn điều kiện sẽ được thêm vào danh sách.
    /// </remarks>
    public static List<Cell> GetNeighborsByCondition(Cell cell, Cell[,,] grid, Vector3Int boxSize, System.Func<Cell, bool> condition)
    {
        List<Cell> neighbors = new List<Cell>();
        foreach (var dir in GetValidDirections())
        {
            Vector3Int neighborPos = new Vector3Int(cell.x, cell.y, cell.z) + dir;
            if (IsInBounds(neighborPos, boxSize))
            {
                Cell neighbor = grid[neighborPos.x, neighborPos.y, neighborPos.z];
                if (condition == null || condition(neighbor))
                    neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }
    public static Cell GetRandomNeighbor(Cell cell, Cell[,,] grid, Vector3Int boxSize)
    {
        List<Cell> neighbors = GetNeighborsByCondition(cell, grid, boxSize, null);
        return neighbors.Count > 0 ? neighbors[rand.Next(neighbors.Count)] : null;
    }
    public static Cell GetUnvisitedNeighbor(Cell cell, Cell[,,] grid, Vector3Int boxSize)
    {
        List<Cell> neighbors = GetNeighborsByCondition(cell, grid, boxSize, v => !v.visited);
        return neighbors.Count > 0 ? neighbors[rand.Next(neighbors.Count)] : null;
    }
    public static Cell GetVisitedNeighbor(Cell cell, Cell[,,] grid, Vector3Int boxSize)
    {
        List<Cell> neighbors = GetNeighborsByCondition(cell, grid, boxSize, v => v.visited);
        return neighbors.Count > 0 ? neighbors[rand.Next(neighbors.Count)] : null;
    }
    public static bool HasVisitedNeighbor(Cell cell, Cell[,,] grid, Vector3Int boxSize)
    {
        return GetVisitedNeighbor(cell, grid, boxSize) != null;
    }
    /// <summary>
    /// Lấy danh sách các hướng di chuyển hợp lệ trong mê cung, được xác định bởi các trục động của mê cung.
    /// </summary>
    /// <returns>Danh sách các hướng di chuyển hợp lệ dưới dạng mảng các Vector3Int.</returns>
    /// <remarks>
    /// Phương thức này sử dụng các trục động (dynamic axes) của mê cung để tạo ra các hướng di chuyển hợp lệ
    /// (trái, phải, trên, dưới) dựa trên cách thức tổ chức của mê cung. Các hướng di chuyển được trả về dưới 
    /// dạng mảng các vector, mỗi vector biểu thị một hướng trong không gian 3D (X, Y, Z).
    /// </remarks>
    public static Vector3Int[] GetValidDirections()
    {
        var axes = MazeGenerator.Instance.GetDynamicAxes();
        Vector3Int[] directions =
        {
        CreateDirection(-1, 0, axes), // Trái
        CreateDirection(1, 0, axes),  // Phải
        CreateDirection(0, -1, axes), // Dưới
        CreateDirection(0, 1, axes)   // Trên
        };
        return directions;
    }
    /// <summary>
    /// Kiểm tra xem một vị trí có nằm trong phạm vi hợp lệ của mê cung hay không.
    /// </summary>
    /// <param name="pos">Vị trí cần kiểm tra.</param>
    /// <param name="boxSize">Kích thước giới hạn của mê cung.</param>
    /// <returns>Trả về `true` nếu vị trí nằm trong phạm vi hợp lệ, ngược lại trả về `false`.</returns>
    public static bool IsInBounds(Vector3Int pos, Vector3Int boxSize)
    {
        return pos.x >= 0 && pos.x < boxSize.x &&
               pos.y >= 0 && pos.y < boxSize.y &&
               pos.z >= 0 && pos.z < boxSize.z;
    }
    /// <summary>
    /// Lấy kích thước của một trục trong không gian 3D dựa trên chỉ số trục.
    /// </summary>
    /// <param name="axis">Chỉ số của trục (0: X, 1: Y, 2: Z).</param>
    /// <param name="boxSize">Kích thước của không gian (bao gồm x, y, z).</param>
    /// <returns>Kích thước của trục tương ứng (X, Y, hoặc Z) trong không gian 3D.</returns>
    /// <remarks>
    /// Phương thức này trả về kích thước của trục tương ứng từ một đối tượng `Vector3Int`, tùy thuộc vào 
    /// giá trị của chỉ số trục được cung cấp (0 cho X, 1 cho Y, 2 cho Z). Nếu chỉ số trục không hợp lệ 
    /// (khác 0, 1, 2), phương thức sẽ trả về kích thước của trục Z.
    /// </remarks>
    public static int GetSize(int axis, Vector3Int boxSize)
    {
        return axis == 0 ? boxSize.x : axis == 1 ? boxSize.y : boxSize.z;
    }
    /// <summary>
    /// Lấy giá trị của một tọa độ thuọc một trục trong không gian 3D từ đối tượng Cell.
    /// </summary>
    /// <param name="axis">Chỉ số của trục (0: X, 1: Y, 2: Z).</param>
    /// <param name="cell">Đối tượng Cell chứa giá trị của các trục (x, y, z).</param>
    /// <returns>Giá trị của trục tương ứng từ đối tượng Cell (X, Y, hoặc Z).</returns>
    /// <remarks>
    /// Phương thức này trả về giá trị của trục tương ứng từ đối tượng `Cell` dựa trên chỉ số trục (0 cho X, 1 cho Y, 2 cho Z).
    /// Nếu chỉ số trục không hợp lệ (khác 0, 1, 2), phương thức sẽ trả về giá trị của trục Z.
    /// </remarks>
    public static int GetAxisValue(int axis, Cell cell)
    {
        return axis == 0 ? cell.x : axis == 1 ? cell.y : cell.z;
    }
    /// <summary>
    /// Lấy đối tượng Cell từ lưới 3D dựa trên tọa độ theo trục chính và phụ.
    /// </summary>
    /// <param name="primary">Giá trị độ dài cho trục chính (X, Y hoặc Z).</param>
    /// <param name="secondary">Giá trị độ dài cho trục phụ (X, Y hoặc Z).</param>
    /// <param name="grid">Lưới 3D chứa các đối tượng Cell.</param>
    /// <param name="boxSize">Kích thước của lưới 3D (boxSize.x, boxSize.y, boxSize.z).</param>
    /// <returns>Đối tượng Cell tại tọa độ xác định trong lưới 3D, hoặc null nếu tọa độ vượt phạm vi.</returns>
    /// <remarks>
    /// Phương thức này tính toán tọa độ 3D của ô trong lưới `grid` dựa trên trục chính và phụ,
    /// sau đó trả về đối tượng `Cell` tại tọa độ đó. Nếu tọa độ không hợp lệ (vượt phạm vi),
    /// phương thức sẽ ghi log lỗi và trả về `null`.
    /// </remarks>
    public static Cell GetCellByAxes(int primary, int secondary, Cell[,,] grid, Vector3Int boxSize)
    {
        int primaryProperty = MazeGenerator.Instance.GetDynamicAxes().Value.primary;
        int secondaryProperty = MazeGenerator.Instance.GetDynamicAxes().Value.secondary;


        int x = primaryProperty == 0 ? primary : secondaryProperty == 0 ? secondary : 0;
        int y = primaryProperty == 1 ? primary : secondaryProperty == 1 ? secondary : 0;
        int z = primaryProperty == 2 ? primary : secondaryProperty == 2 ? secondary : 0;
        Vector3Int pos = new Vector3Int(x, y, z);

        if (!IsInBounds(pos, boxSize))
        {
            Debug.LogError($"GetCellByAxes: Tọa độ ({x}, {y}, {z}) vượt phạm vi! Kích thước hợp lệ: ({boxSize.x}, {boxSize.y}, {boxSize.z})");
            return null;
        }

        return grid[x, y, z];
    }
    public static Cell GetCellByAxes(int x, int y, int z, Cell[,,] grid, Vector3Int boxSize)
    {
        Vector3Int pos = new Vector3Int(x, y, z);

        if (!IsInBounds(pos, boxSize))
        {
            Debug.LogError($"GetCellByAxes: Tọa độ ({x}, {y}, {z}) vượt phạm vi! Kích thước hợp lệ: ({boxSize.x}, {boxSize.y}, {boxSize.z})");
            return null;
        }

        return grid[x, y, z];
    }
    /// <summary>
    /// Trả về ô (Cell) chứa GameObject được truyền vào, nếu GameObject nằm trong grid.
    /// </summary>
    /// <param name="obj">GameObject cần xác định ô chứa nó.</param>
    /// <param name="grid">Lưới 3D chứa các đối tượng Cell.</param>
    /// <param name="boxSize">Kích thước của lưới 3D (boxSize.x, boxSize.y, boxSize.z).</param>
    /// <returns>Cell tương ứng hoặc null nếu ngoài phạm vi mê cung.</returns>
    public static Cell GetCellFromGameObject(GameObject obj, Cell[,,] grid, Vector3Int boxSize, float cellSize)
    {
        Vector3 pos = obj.transform.position;

        int x = Mathf.FloorToInt(pos.x / cellSize);
        int y = Mathf.FloorToInt(pos.y / cellSize);
        int z = Mathf.FloorToInt(pos.z / cellSize);

        return MazeTools.GetCellByAxes(x, y, z, grid, boxSize);
    }


    public static void ColorPath(Color pathColor, Cell startCell, Cell endCell, MazeSO data)
    {

        List<Cell> path = new PathFinding_Astar(data).FindPath(startCell, endCell);
        if (path == null) { Debug.Log("Không tồn tại đường đi khả dụng"); return; }

        Vector3Int fixedAxis = GetFixedAxis(data.Width, data.Height, data.Depth);

        foreach (Cell cell in path)
        {
            Renderer renderer = GetWallRenderer(cell, fixedAxis).GetComponent<Renderer>();
            
            if (renderer != null)
            {
                renderer.material.color = pathColor;
            }
        }

    }
}
public struct DynamicAxes
{
    public int primary;
    public int secondary;

    public DynamicAxes(int primary, int secondary)
    {
        this.primary = primary;
        this.secondary = secondary;
    }

    public bool IsXY() => (primary == 0 && secondary == 1) || (primary == 1 && secondary == 0);
    public bool IsXZ() => (primary == 0 && secondary == 2) || (primary == 2 && secondary == 0);
    public bool IsYZ() => (primary == 1 && secondary == 2) || (primary == 2 && secondary == 1);
}

