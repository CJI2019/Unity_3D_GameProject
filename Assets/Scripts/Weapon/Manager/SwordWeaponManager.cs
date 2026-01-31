using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SwordWeaponManager : WeaponManager
{
    [SerializeField] SwordWeapon weaponPrefab;
    const string poolKey = "SwordWeapon";

    float attackInterval = 2f;
    float lastAttackTime = 0f;
    float weaponSpawnDelay = 0.1f;

    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab, poolKey);

        enabled = false;
    }

    void Update()
    {
        if (lastAttackTime > Time.time) return;
        lastAttackTime = Time.time + attackInterval;

        StartCoroutine(SpawnBullet(weaponSpawnDelay));
    }

    IEnumerator SpawnBullet(float delay)
    {
        var wfs = new WaitForSeconds(delay);

        for (int i = 0; i < weaponData.weaponCount; ++i)
        {
            SwordWeapon weapon = AddWeapon<SwordWeapon>();
            weapon.Initialize(this, weaponData.damage);
            yield return wfs;
        }
    }
}
