using System.Collections.Generic;
using UnityEngine;

public class PathFinding_Astar
{
    private int width, height, depth;
    private Cell[,,] grid;

    private Vector3Int boxSize;

    public PathFinding_Astar(MazeSO data)
    {
        width = data.Width;
        height = data.Height;
        depth = data.Depth;
        grid = MazeGenerator.grid;

        boxSize = new Vector3Int(width, height, depth);
    }
    public GameObject GetGameObjectAtCell(Cell cell)
    {
        Vector3 targetPos = new Vector3(cell.x, cell.y, cell.z);

        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(obj.transform.position, targetPos) < 0.1f)
            {
                return obj;
            }
        }
        return null;
    }

    public List<Cell> FindPath(Cell start, Cell end)
    {
        if (start == null || end == null)
        {
            Debug.Log("Không thể xác định được cell từ gameObject.");
            return null;
        }

        List<Cell> openSet = new List<Cell> { start };
        HashSet<Cell> closedSet = new HashSet<Cell>();

        Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();
        Dictionary<Cell, float> gScore = new Dictionary<Cell, float>();
        Dictionary<Cell, float> fScore = new Dictionary<Cell, float>();

        foreach (Cell cell in grid)
        {
            gScore[cell] = float.MaxValue;
            fScore[cell] = float.MaxValue;
        }

        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => fScore[a].CompareTo(fScore[b]));
            Cell current = openSet[0];
            openSet.RemoveAt(0);

            if (current == end)
                return ReconstructPath(cameFrom, current);

            closedSet.Add(current);

            foreach (Cell neighbor in GetWalkableNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeGScore = gScore[current] + 1;
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);
            }
        }

        return null; 
    }

    private List<Cell> GetWalkableNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        foreach (var dir in MazeTools.GetValidDirections())
        {
            Vector3Int neighborPos = new Vector3Int(cell.x, cell.y, cell.z) + dir;
            if (MazeTools.IsInBounds(neighborPos, boxSize))
            {
                Cell neighbor = grid[neighborPos.x, neighborPos.y, neighborPos.z];
                if (!MazeTools.HasWallBetween(cell, neighbor))
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
    /// <summary>
    /// Tính khoảng cách dựa trên công thức manhattan
    /// </summary>
    /// <param name="a">Vị trí cell a</param>
    /// <param name="b">Vị trí cell b</param>
    /// <returns>Khoảng cách giữa 2 điểm trong không gian</returns>
    private float Heuristic(Cell a, Cell b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
    {
        List<Cell> path = new List<Cell>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }


}