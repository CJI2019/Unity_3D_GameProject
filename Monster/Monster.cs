using System.Collections.Generic;
using UnityEngine;

public class Monster : LivingEntity , IAttacker
{
    MonsterAI ai;

    public float AttackRange => throw new System.NotImplementedException();
    public float AttackCoolDown => throw new System.NotImplementedException();

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        ai = GetComponent<MonsterAI>();
    }

    protected override void DeathLogic()
    {
        List<ExpItem> spawnItems = DropItemManager.Instance.ItemSpawn<ExpItem>(1);

        foreach (var item in spawnItems)
        {
            item.transform.position = transform.position;
        }
    }
}