using System.Collections.Generic;
using UnityEngine;
public interface IPoolable
{
    void SpawnFromPool();
    void ReturnToPool();
}
public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _pool = new();

    public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
            Return(CreateInstance());
    }

    private T CreateInstance()
    {
        T instance = Object.Instantiate(_prefab, _parent);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public T Spawn()
    {
        T instance = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();
        instance.gameObject.SetActive(true);

        if (instance.TryGetComponent<IPoolable>(out var poolable))
            poolable.SpawnFromPool();

        return instance;
    }

    public void Return(T instance)
    {
        if (instance.TryGetComponent<IPoolable>(out var poolable))
            poolable.ReturnToPool();

        instance.gameObject.SetActive(false);
        _pool.Enqueue(instance);
    }
}
