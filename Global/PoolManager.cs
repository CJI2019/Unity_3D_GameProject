using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    private Dictionary<string, object> pools = new Dictionary<string, object>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 풀 등록
    public void RegisterPool<T>(string key, GenericObjectPool<T> pool) where T : Component, IPoolable
    {
        pools[key] = pool;
    }

    // 풀에서 꺼내오기
    public T Get<T>(string key) where T : Component, IPoolable
    {
        return ((GenericObjectPool<T>)pools[key]).Get();
    }

    // 풀에 반환하기
    public void Return<T>(string key, T obj) where T : Component, IPoolable
    {
        ((GenericObjectPool<T>)pools[key]).Return(obj);
    }
}
