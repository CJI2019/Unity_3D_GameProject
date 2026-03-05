using System.Collections;
using UnityEngine;

public class BulletWeaponManager : WeaponManager
{
    [SerializeField] BulletWeapon weaponPrefab;
    const string poolKey = "BulletWeapon";
    
    float lastAttackTime   = 0f;
    float bulletSpawnDelay = 0.1f;

    int penetrationCount = 1;

    public override void SetWeaponData(AbilityData abilityData)
    {
        base.SetWeaponData(abilityData);
        int level = abilityData.level;

        switch (level)
        {
            case 1:
                attackInterval = 2f;
                penetrationCount = 1;
                break;
            case 2:
                attackInterval = 1f;
                penetrationCount = 3;
                break;
            case 3:
                attackInterval = 0.5f;
                penetrationCount = 5;
                break;
            default:
                Debug.LogError("존재하지 않는 레벨입니다 : " + level);
                break;
        }
    }

    protected override void UpdateManager()
    {
        if (lastAttackTime > Time.time) return;
        lastAttackTime = Time.time + attackInterval;

        StartCoroutine(SpawnBullet(bulletSpawnDelay));
    }

    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab,poolKey);

        enabled = false;
    }

    IEnumerator SpawnBullet(float delay)
    {
        var wfs = new WaitForSeconds(delay);

        for (int i = 0; i < weaponData.weaponCount; ++i)
        {
            BulletWeapon weapon = AddWeapon<BulletWeapon>();
            weapon.Initialize(this, penetrationCount,weaponData.damage);
            yield return wfs;
        }
    }
}
