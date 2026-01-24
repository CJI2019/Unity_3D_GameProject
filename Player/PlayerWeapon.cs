using UnityEngine;


public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] OrbitWeaponManager orbitWeaponManager;
    [SerializeField] BulletWeaponManager bulletWeaponManager;

    public void LevelUpWeapon(AbilityType weaponType, int level)
    {
        AbilityData weapon = GameAbilityManager.Instance.GetWeaponData(weaponType,level);

        Debug.Log($"{weapon.level} 레벨의 {weapon.name} 을 가져왔습니다.");

        switch(weaponType)
        {
            case AbilityType.BULLET:
                bulletWeaponManager.SetWeaponData(weapon);
            break;
            case AbilityType.ORBIT:
                orbitWeaponManager.SetWeaponData(weapon);
            break;
        }
    }
}