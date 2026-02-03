using UnityEngine;

public class OrbitWeaponManager : WeaponManager
{
    [SerializeField] OrbitWeapon weaponPrefab;
    [SerializeField] float rotateSpeed = 100f;
    const string poolKey = "OrbitWeapon";

    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab,poolKey);
    }

    public override void SetWeaponData(AbilityData abilityData)
    {
        weaponData = abilityData;

        switch (weaponData.level)
        {
            case 1:
                rotateSpeed = 50f;
                break;
            case 2:
                rotateSpeed = 100f;
                break;
            case 3:
                rotateSpeed = 150f;
                break;
            case 4:
                rotateSpeed = 200f;
                break;
            default:
                Debug.LogError("존재하지 않는 레벨입니다 : " + weaponData.level);
                break;
        }

        var addWeaponCount = weaponData.weaponCount - spawnedWeapons.Count;
        if (addWeaponCount > 0)
        {
            AddWeapon<OrbitWeapon>(addWeaponCount);
        }
    }

    public override void UpdateWeapons()
    {
        var length = spawnedWeapons.Count;

        for (int i = 0; i < length; ++i)
        {
            var orbitWeapon = spawnedWeapons[i] as OrbitWeapon;
            if(!orbitWeapon) continue;

            float angle = 360f / length * i;
            orbitWeapon.Initialize(angle, rotateSpeed,weaponData.damage);
        }
    }    
}