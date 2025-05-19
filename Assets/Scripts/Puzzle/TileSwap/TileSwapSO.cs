using UnityEngine;

[CreateAssetMenu(fileName = "TileSwapSO", menuName = "Scriptable Objects/TileSwapSO")]
public class TileSwapSO : ScriptableObject
{
    [Header("Source")]
    public Sprite sourceImage;
    [Range(2, 10)] public int boardSize = 3;    
    public bool shuffleOnStart = true;
    [Tooltip("Seed = -1 => random each play")]
    [HideInInspector] public int seed = -1;
}
