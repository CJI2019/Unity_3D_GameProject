using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWeaponManager : WeaponManager
{
    [SerializeField] BulletWeapon weaponPrefab;
    const string poolKey = "BulletWeapon";
    
    float attackInterval   = 5f;
    float lastAttackTime   = 0f;
    float bulletSpawnDelay = 0.1f;

    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab,poolKey);

        enabled = false;
    }

    void Update()
    {
        if (lastAttackTime > Time.time) return;
        lastAttackTime = Time.time + attackInterval;

        StartCoroutine(SpawnBullet(bulletSpawnDelay));
    }

    IEnumerator SpawnBullet(float delay)
    {
        var wfs = new WaitForSeconds(delay);

        for (int i = 0; i < weaponData.weaponCount; ++i)
        {
            BulletWeapon weapon = AddWeapon<BulletWeapon>();
            weapon.Initialize(this,weaponData.damage);
            yield return wfs;
        }
    }
}
