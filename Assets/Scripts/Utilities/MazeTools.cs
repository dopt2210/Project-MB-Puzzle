using System.Collections.Generic;
using UnityEngine;

public static class MazeTools
{
    private static System.Random rand = new System.Random();
    /// <summary>
    /// Xóa hai bức tường kề nhau của hai cell.
    /// </summary>
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
    /// Trả về true nếu có tồn tại tường giữa hai cell kề nhau.
    /// </summary>
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
    /// Trả về hai trục được thay đổi trong ba trục X, Y, Z.
    /// Y-Z Nếu width bằng 0.
    /// X-Z Nếu height bằng 0.
    /// X-Y Nếu depth bằng 0.
    /// </summary>
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
    /// Trả về hướng của tường thuộc trục cố định.
    /// </summary>
    public static Vector3Int GetFixedAxis(int width, int height, int depth)
    {
        if (width == 1) return Vector3Int.right;    // X cố định
        if (height == 1) return Vector3Int.up;      // Y cố định
        if (depth == 1) return Vector3Int.forward;  // Z cố định

        Debug.LogError("Lỗi: Không xác định được trục cố định!");
        return Vector3Int.zero; 
    }
    /// <summary>
    /// Lấy Renderer của bức tường nằm ở hướng `oppositeAxis`.
    /// </summary>
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
    /// Trả về hai trục đường có thể.
    /// </summary>
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
    /// Trả về cell hàng xoám chưa thăm.
    /// </summary>
    public static Cell GetUnvisitedNeighbor(Cell cell, Cell[,,] grid, Vector3Int boxSize)
    {
        List<Cell> neighbors = new List<Cell>();

        foreach (var dir in GetValidDirections())
        {
            Vector3Int neighborPos = new Vector3Int(cell.x, cell.y, cell.z) + dir;

            if (IsInBounds(neighborPos, boxSize))
            {
                Cell neighbor = grid[neighborPos.x, neighborPos.y, neighborPos.z];
                if (!neighbor.visited)
                    neighbors.Add(neighbor);
            }

        }
        return neighbors.Count > 0 ? neighbors[rand.Next(neighbors.Count)] : null;
    }
    /// <summary>
    /// Trả về cell hàng xóm đã thăm.
    /// </summary>
    public static Cell GetVisitedNeighbor(Cell cell, Cell[,,] grid, Vector3Int boxSize)
    {
        foreach (var dir in GetValidDirections())
        {
            Vector3Int neighborPos = new Vector3Int(cell.x, cell.y, cell.z) + dir;
            if (IsInBounds(neighborPos, boxSize))
            {
                Cell neighbor = grid[neighborPos.x, neighborPos.y, neighborPos.z];
                if (neighbor.visited)
                    return neighbor;
            }
        }
        return null;
    }
    /// <summary>
    /// Trả về true nếu cell đã thăm.
    /// </summary>
    public static bool HasVisitedNeighbor(Cell cell, Cell[,,] grid, Vector3Int boxSize)
    {
        return MazeTools.GetVisitedNeighbor(cell, grid, boxSize) != null;
    }
    /// <summary>
    /// Trả về hướng đi có thể đi
    /// </summary>
    public static Vector3Int[] GetValidDirections()
    {
        Vector3Int[] directions =
        {
        CreateDirection(-1, 0, MazeGenerator.Instance.GetDynamicAxes()), // Trái
        CreateDirection(1, 0, MazeGenerator.Instance.GetDynamicAxes()),  // Phải
        CreateDirection(0, -1, MazeGenerator.Instance.GetDynamicAxes()), // Dưới
        CreateDirection(0, 1, MazeGenerator.Instance.GetDynamicAxes())   // Trên
        };
        return directions;
    }
    /// <summary>
    /// Trả về true nếu chưa chạm phải biên của mê cung.
    /// </summary>
    public static bool IsInBounds(Vector3Int pos, Vector3Int boxSize)
    {
        return pos.x >= 0 && pos.x < boxSize.x &&
               pos.y >= 0 && pos.y < boxSize.y &&
               pos.z >= 0 && pos.z < boxSize.z;
    }
    /// <summary>
    /// Trả về kích thước của mê cung theo trục được chỉ định.
    /// </summary>
    public static int GetSize(int axis, Vector3Int boxSize)
    {
        return axis == 0 ? boxSize.x : axis == 1 ? boxSize.y : boxSize.z;
    }
    /// <summary>
    /// Trả về Cell cần lấy dựa trên trục được chỉ định
    /// </summary>
    public static Cell GetCell(int primary, int secondary, Cell[,,] grid)
    {
        int primaryProperty = MazeGenerator.Instance.GetDynamicAxes().Value.primary;
        int secondaryProperty = MazeGenerator.Instance.GetDynamicAxes().Value.secondary;

        int x = primaryProperty == 0 ? primary : secondaryProperty == 0 ? secondary : 0;
        int y = primaryProperty == 1 ? primary : secondaryProperty == 1 ? secondary : 0;
        int z = primaryProperty == 2 ? primary : secondaryProperty == 2 ? secondary : 0;

        return grid[x, y, z];
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

