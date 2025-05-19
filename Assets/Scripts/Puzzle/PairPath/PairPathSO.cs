using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PairPathSO", menuName = "Scriptable Objects/PairPathSO")]
public class PairPathSO : ScriptableObject
{
    [Range(2, 32)] public int boardSize = 4;
    public Vector2Int GridSize => new(boardSize, boardSize);

    public List<Pair> pairs = new();

    [HideInInspector] public int seed = -1;

    [Serializable]
    public struct Pair
    {
        public int id;
        public Vector2Int start;   // 0‑based, (x = column, y = row)
        public Vector2Int end;
        public Color color;
    }
    [ContextMenu("Generate Random Pairs")]
    public void GenerateRandomPairs()
    {
        int cells = boardSize * boardSize;

        // 1) tạo pool toạ độ
        var pool = new Vector2Int[cells];
        for (int y = 0, i = 0; y < boardSize; y++)
            for (int x = 0; x < boardSize; x++, i++)
                pool[i] = new Vector2Int(x, y);

        // 2) xáo trộn
        System.Random rng = seed >= 0 ? new(seed) : new();
        for (int i = cells - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        // 3) xác định số cặp tối đa
        int pairCount = cells / (boardSize * 2);

        pairs.Clear();
        for (int i = 0; i < pairCount; i++)
        {
            var p = new Pair
            {
                id = i,
                start = pool[i * 2],
                end = pool[i * 2 + 1],
                color = UnityEngine.Random.ColorHSV(0, 1, .6f, 1, .9f, 1, 1, 1) // màu tươi
            };
            pairs.Add(p);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);      // lưu lại asset
#endif
    }
}
