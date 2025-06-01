using UnityEngine;

[CreateAssetMenu(fileName = "TileSwapSO", menuName = "Scriptable Objects/TileSwapSO")]
public class TileSwapSO : ScriptableObject
{
    [HideInInspector] public string uniqueId;

    [Tooltip("Seed = -1 => random each play")]
    [HideInInspector] public int seed = -1;

    [Header("Source")]
    public Sprite sourceImage;
    [Range(2, 10)] public int boardSize = 3;    
    public bool shuffleOnStart = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueId))
        {
            uniqueId = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
