using System;
using System.Collections.Generic;
using UnityEngine;

public class Monster : LivingEntity , IAttacker
{
    [SerializeField] float itemSpawnHeight = 1.0f;
    [SerializeField] long originDamage     = 1;

    public event Action<Monster> OnMonsterDead;
    string poolKey = "";

    public float AttackRange {get;}
    public float AttackCoolDown {get;}

    long damage;
    ExpItemEntry expItemEntry;

    protected void Awake()
    {
        damage = originDamage;
    }

    public void Attack(Collider other)
    {
        other.GetComponent<LivingEntity>()?.TakeDamage(damage);
    }

    public void MultiplyDamage(float damage_Weight)
    {
        damage = (long)(originDamage * damage_Weight);
    }

    public void SetExpItemEntry(ExpItemEntry expItemEntry)
    {
        this.expItemEntry = expItemEntry;
    }

    public void SetPoolKey(string poolKey)
    {
        this.poolKey = poolKey;
    }

    public override void TakeDamage(long amount)
    {
        if(isDead) return;

        base.TakeDamage(amount);
        
        
    }

    protected override void DeathLogic()
    {
        List<ExpItem> spawnExpItems = DropItemManager.Instance.ItemSpawn<ExpItem>(1);

        foreach (var exp in spawnExpItems)
        {
            exp.transform.position = transform.position + Vector3.up * itemSpawnHeight;
            MeshManager.Instance.SwapMesh(exp.gameObject,expItemEntry.GetMeshId());
            exp.SetExp(expItemEntry.exp);
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