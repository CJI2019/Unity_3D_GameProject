using System;
using System.Collections.Generic;
using UnityEngine;

public class Monster : LivingEntity , IAttacker
{
    public event Action<Monster> OnMonsterDead;
    string poolKey = "";

    public float AttackRange {get;}
    public float AttackCoolDown {get;}

    public void Attack()
    {
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
            item.transform.position = transform.position;
        }
        
        OnMonsterDead?.Invoke(this);
        OnMonsterDead = null;
        
        PoolManager.Instance.Return<Monster>(poolKey,this);
    }
}