using UnityEngine;

public class OrbitWeaponManager : WeaponManager
{
    [SerializeField] OrbitWeapon weaponPrefab;
    const string poolKey = "OrbitWeapon";
    
    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab,poolKey);
    }

    public override void SetWeaponData(AbilityData abilityData)
    {
        weaponData = abilityData;

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
            orbitWeapon.Initialize(angle,weaponData.damage);
        }
    }    
}