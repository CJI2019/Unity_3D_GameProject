using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemType
{
    EXP,
    MAGNETIC,
}

public class DropItemManager : SceneSingleton<DropItemManager>
{
    [SerializeField] ExpItem expItemPrefab;
    [SerializeField] MagneticItem magneticItemPrefab;

    Dictionary<Type, string> poolKeyTable;
    const string poolKey_ExpItem = "ExpItem";
    const string poolKey_MagneticItem = "MagneticItem";

    protected override void Awake()
    {
        base.Awake();
        // 아이템의 클래스 타입 - poolKey 매핑
        poolKeyTable = new Dictionary<Type, string> { 
            { typeof(ExpItem), poolKey_ExpItem },
            { typeof(MagneticItem), poolKey_MagneticItem },
        };
    }

    void Start()
    {
        RegisterPickUp<ExpItem>(expItemPrefab, poolKey_ExpItem, 1000);
        RegisterPickUp<MagneticItem>(magneticItemPrefab, poolKey_MagneticItem, 10);
    }

    protected void RegisterPickUp<T>(T prefab,string poolKey,int initialSize) where T : Component, IPoolable
    {
        var pool = new GenericObjectPool<T>(prefab, initialSize, transform);
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

    public IEnumerator MagneticLogic(List<ExpItem> items,Transform player)
    {
        while (true)
        {
            for (int i = items.Count - 1; i >= 0; --i)
            {
                var item = items[i];
                if (!item.gameObject.activeSelf)
                {
                    items.RemoveAt(i);
                    continue;
                }
                var dir = player.position - item.transform.position;
                item.transform.position += dir.normalized * 30f * Time.deltaTime;
            }

            yield return null;

            while(GameManager.Instance.IsGamePaused) {
                yield return null;
            }

            if(items.Count == 0) yield break;
        }
    }
}
