using UnityEngine;

public class OrbitWeaponManager : WeaponManager
{
    [SerializeField] OrbitWeapon weaponPrefab;
    const string poolKey = "OrbitWeapon";
    
    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab,poolKey);
        // 레벨과 무기 개수에 따라서 업데이트 되어야함.
        // AddWeapon<OrbitWeapon>(1);
    }

    protected override void AddWeaponBySubClass(int count)
    {
        AddWeapon<OrbitWeapon>(count);
    }

    public override void UpdateWeapons()
    {
        var length = activeWeapons.Count;

        for (int i = 0; i < length; ++i)
        {
            var orbitWeapon = activeWeapons[i] as OrbitWeapon;
            if(!orbitWeapon) continue;

            float angle = 360f / length * i;
            orbitWeapon.Initialize(angle);
        }
    }    
}