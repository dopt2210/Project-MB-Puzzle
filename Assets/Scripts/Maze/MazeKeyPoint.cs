using System.Collections.Generic;
[System.Serializable]
public class MazeKeyPoint 
{
    private Dictionary<MazeAlgorithmType, List<Cell>> keyPoints = new();

    public void AddKeyPoint(MazeAlgorithmType type, Cell cell)
    {
        if (!keyPoints.ContainsKey(type))
            keyPoints[type] = new List<Cell>();
        keyPoints[type].Add(cell);
    }
    public List<Cell> GetKeyPoints(MazeAlgorithmType type)
    {
        return keyPoints.TryGetValue(type, out var list) ? list : new List<Cell>();
    }
    public void RemoveKeyPoint(MazeAlgorithmType type, Cell cell)
    {
        if (keyPoints.TryGetValue(type, out var list))
        {
            list.Remove(cell);
            //if (list.Count == 0)
            //    keyPoints.Remove(type);
        }
    }
    public void Clear()
    {
        keyPoints.Clear();
    }
}