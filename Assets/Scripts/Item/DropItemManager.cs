using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    EXP
}

public class DropItemManager : MonoBehaviour
{
    public static DropItemManager Instance;

    [SerializeField] ExpItem prefab;
    
    Dictionary<Type, string> poolKeyTable;
    const string poolKey_ExpItem = "ExpItem";

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        // 아이템의 클래스 타입 - poolKey 매핑
        poolKeyTable = new Dictionary<Type, string> { 
            { typeof(ExpItem), poolKey_ExpItem },
        };
    }

    void Start()
    {
        RegisterPickUp<ExpItem>(prefab,poolKey_ExpItem);
    }
    protected void RegisterPickUp<T>(T prefab,string poolKey) where T : Component, IPoolable
    {
        var pool = new GenericObjectPool<T>(prefab, 1000, transform);
        PoolManager.Instance.RegisterPool(poolKey, pool);
    }


    public List<T> ItemSpawn<T>(int count) where T : Component, IPoolable
    {
        string poolKey = GetTypePoolKey<T>();
        if (poolKey == null) return null;

        List<T> spawnList = new List<T>();
        for (int i = 0; i < count; ++i)
        {
            T item = PoolManager.Instance.Get<T>(poolKey);
            spawnList.Add(item);
        }

        return spawnList;
    }

    private string GetTypePoolKey<T>() where T : Component, IPoolable
    {
        Type type = typeof(T);
        string poolKey = poolKeyTable[type];
        return poolKey;
    }

    public void ItemDeSpawn<T>(T item) where T : Component, IPoolable
    {
        string poolKey = GetTypePoolKey<T>();
        PoolManager.Instance.Return<T>(poolKey,item);
    }
}
