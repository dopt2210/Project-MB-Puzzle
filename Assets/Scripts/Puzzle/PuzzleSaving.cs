using System.Linq;
using UnityEngine;

public static class PuzzleSaving
{
    public static TileSwapSO FindTileSwap(string uniqueId)
    {
        var allPuzzles = Resources.LoadAll<TileSwapSO>("Scriptable/TileSwap");
        return allPuzzles.FirstOrDefault(p => p.uniqueId == uniqueId);
    }
    public static PairPathSO FindPairPath(string uniqueId)
    {
        var allPuzzles = Resources.LoadAll<PairPathSO>("Scriptable/PairPath");
        return allPuzzles.FirstOrDefault(p => p.uniqueId == uniqueId);
    }
    public static WordleSO FindWordle(string uniqueId)
    {
        var allPuzzles = Resources.LoadAll<WordleSO>("Scriptable/Wordle");
        return allPuzzles.FirstOrDefault(p => p.uniqueId == uniqueId);
    }
}
