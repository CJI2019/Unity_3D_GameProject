using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> where T : Component, IPoolable
{
    private Queue<T> pool = new Queue<T>();
    private T prefab;
    private Transform parent;

    public GenericObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            obj.OnSpawn();
            return obj;
        }
        else
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.OnSpawn();
            return obj;
        }
    }

    public void Return(T obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
