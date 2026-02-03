using System.Collections;
using UnityEngine;

public class ThunderStrikeWeaponManager : WeaponManager
{
    [SerializeField] ThunderStrikeWeapon weaponPrefab;
    [SerializeField] float weaponScale = 1.0f;
    const string poolKey = "ThunderStrikeWeapon";

    float lastAttackTime = 0f;
    float weaponSpawnDelay = 0.1f;

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

        switch(level)
        {
            case 1:
                attackInterval = 2f;
                weaponScale = 1.0f;
                break;
            case 2:
                attackInterval = 1f;
                weaponScale = 1.5f;
                break;
            case 3:
                attackInterval = 0.5f;
                weaponScale = 2.0f;
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
            ThunderStrikeWeapon weapon = AddWeapon<ThunderStrikeWeapon>();
            weapon.Initialize(this, weaponData.damage, weaponScale);

            yield return wfs;
        }
    }
}
