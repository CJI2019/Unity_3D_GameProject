using System;
using System.Collections.Generic;
using UnityEngine;

public class Monster : LivingEntity , IAttacker
{
    [SerializeField] float itemSpawnHeight = 1.0f;
    [SerializeField] long damage           = 1;

    public event Action<Monster> OnMonsterDead;
    string poolKey = "";

    public float AttackRange {get;}
    public float AttackCoolDown {get;}

    public void Attack(Collider other)
    {
        other.GetComponent<LivingEntity>()?.TakeDamage(damage);
    }

    public void SetPoolKey(string poolKey)
    {
        this.poolKey = poolKey;
    }

    protected override void DeathLogic()
    {
        List<ExpItem> spawnItems = DropItemManager.Instance.ItemSpawn<ExpItem>(1);

        foreach (var item in spawnItems)
        {
            item.transform.position = transform.position + Vector3.up * itemSpawnHeight;
        }
        
        OnMonsterDead?.Invoke(this);
        OnMonsterDead = null;
        
        PoolManager.Instance.Return<Monster>(poolKey,this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Attack(other);
        }
    }
}