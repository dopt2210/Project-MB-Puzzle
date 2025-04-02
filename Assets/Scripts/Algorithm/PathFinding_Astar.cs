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

    public List<Cell> FindPath(Cell start, Cell end)
    {
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

        return null; // Không tìm thấy đường đi
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

    public void ColorPath(Color pathColor)
    {
        Cell startCell = grid[0, 0, 0];
        Cell endCell = grid[width - 1, height - 1, depth - 1];
        List<Cell> path = FindPath(startCell, endCell);
        if (path == null) { Debug.Log("Không tồn tại đường đi khả dụng"); return; }

        Vector3Int fixedAxis = MazeTools.GetFixedAxis(width, height, depth);

        foreach (Cell cell in path)
        {
            Renderer renderer = MazeTools.GetWallRenderer(cell, fixedAxis).GetComponent<Renderer>();
            Debug.Log(MazeTools.GetWallRenderer(cell, fixedAxis));
            if (renderer != null)
            {
                renderer.material.color = pathColor;
            }
        }
        
    }

}