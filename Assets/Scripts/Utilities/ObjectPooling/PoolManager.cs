using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<string, object> _pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public ObjectPool<T> CreatePool<T>(string key, T prefab, int size = 10, Transform parent = null) where T : Component
    {
        var pool = new ObjectPool<T>(prefab, size, parent);
        _pools[key] = pool;
        return pool;
    }

    public ObjectPool<T> GetPool<T>(string key) where T : Component
    {
        if (_pools.TryGetValue(key, out var pool))
            return pool as ObjectPool<T>;

        Debug.LogError($"No pool with key '{key}' found.");
        return null;
    }
}
