using UnityEngine;

public enum ItemType 
{ 
    BulletWeapon,OrbitWeapon
}

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] OrbitWeaponManager orbitWeaponManager;
    [SerializeField] BulletWeaponManager bulletWeaponManager;

    public void ItemPickUp(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.OrbitWeapon:
                orbitWeaponManager.AddWeapon<OrbitWeapon>(1);
            break;
            case ItemType.BulletWeapon:
                bulletWeaponManager.AddWeapon<BulletWeapon>(1);
            break;
            default:
            break;
        }
    }
}