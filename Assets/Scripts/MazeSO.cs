using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeSO", menuName = "Scriptable Objects/MazeSO")]
public class MazeSO : ScriptableObject
{
    public event Action OnDataChanged;

    [Header("Maze")]
    [SerializeField] private int width = 15;
    [SerializeField] private int height = 15;
    [SerializeField] private int depth = 1;

    public GameObject cellPrefab;
    public GameObject playerPrefab;

    public int Width
    {
        get => width;
        set
        {
            if (width != value)
            {
                width = value;
                OnDataChanged?.Invoke(); 
            }
        }
    }

    public int Height
    {
        get => height;
        set
        {
            if (height != value)
            {
                height = value;
                OnDataChanged?.Invoke(); 
            }
        }
    }
    public int Depth
    {
        get => depth;
        set
        {
            if (depth != value)
            {
                depth = value;
                OnDataChanged?.Invoke();
            }
        }
    }
}
