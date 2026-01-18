using UnityEngine;

public class BulletWeaponManager : WeaponManager
{
    [SerializeField] BulletWeapon weaponPrefab;
    const string poolKey = "BulletWeapon";
    
    void Start()
    {
        SetPoolKey(poolKey);
        RegisterWeapon(weaponPrefab,poolKey);
        // 레벨과 무기 개수에 따라서 업데이트 되어야함.
        // AddWeapon<BulletWeapon>(10);
    }
    public override void UpdateWeapons()
    {
        var length = activeWeapons.Count;

        for (int i = 0; i < length; ++i)
        {
            var bulletWeapon = activeWeapons[i] as BulletWeapon;
            if(!bulletWeapon) continue;

            bulletWeapon.Initialize(transform,i * 0.1f);
        }
    }
}
