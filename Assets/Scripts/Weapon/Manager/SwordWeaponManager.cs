using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SwordWeaponManager : WeaponManager
{
    [SerializeField] SwordWeapon weaponPrefab;
    const string poolKey = "SwordWeapon";

    float lastAttackTime = 0f;
    float weaponSpawnDelay = 0.3f;

    float weaponScale = 1.0f;

    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab, poolKey);

        enabled = false;
    }
    public override void SetWeaponData(AbilityData abilityData)
    {
        base.SetWeaponData(abilityData);
        int level = abilityData.level;

        switch (level)
        {
            case 1:
                attackInterval = 3f;
                weaponScale = 1.0f;
                break;
            case 2:
                attackInterval = 2.5f;
                weaponScale = 2.0f;
                break;
            case 3:
                attackInterval = 2f;
                weaponScale = 3.0f;
                break;
            default:
                Debug.LogError("존재하지 않는 레벨입니다 : " + level);
                break;
        }
    }

    void Update()
    {
        if (lastAttackTime > Time.time) return;
        lastAttackTime = Time.time + attackInterval;

        StartCoroutine(SpawnWeapon(weaponSpawnDelay));
    }

    IEnumerator SpawnWeapon(float delay)
    {
        var wfs = new WaitForSeconds(delay);

        for (int i = 0; i < weaponData.weaponCount; ++i)
        {
            SwordWeapon weapon = AddWeapon<SwordWeapon>();
            weapon.Initialize(this, weaponScale,weaponData.damage);
            yield return wfs;
        }
    }
}
