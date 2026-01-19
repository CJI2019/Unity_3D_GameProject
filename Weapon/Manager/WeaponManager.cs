using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WeaponManager : MonoBehaviour
{
    protected List<IWeapon> activeWeapons;
    protected int weaponLevel { get; set; } = 1;
    string poolKey;
    //추상 메서드
    public abstract void UpdateWeapons();
    //
    void Awake()
    {
        activeWeapons = new List<IWeapon>();
    }
    protected void SetPoolKey(string poolKey)
    {
        this.poolKey = poolKey;
    }
    protected void RegisterWeapon<T>(T prefab,string poolKey) where T : Component, IPoolable, IWeapon
    {
        var weaponPool = new GenericObjectPool<T>(prefab, 20, transform);
        PoolManager.Instance.RegisterPool(poolKey, weaponPool);
    }
    public void AddWeapon<T>(int count) where T : Component, IPoolable, IWeapon
    {
        int beforeCount = activeWeapons.Count;
        // 기존 무기 반환
        DeActiveWeapon<T>();

        // 무기 꺼내기
        for (int i = 0; i < beforeCount + count; ++i)
        {
            T weapon = PoolManager.Instance.Get<T>(poolKey);
            ActiveWeapon(weapon);
            weapon.transform.SetParent(transform);
        }

        UpdateWeapons();
    }
    // 활성화된 무기를 활성화 그룹에 추가한다.
    void ActiveWeapon(IWeapon iWeapon)
    {
        activeWeapons.Add(iWeapon);
    }
    // 무기 종류를 활성 그룹에서 제거한다.
    protected void DeActiveWeapon<T>() where T : Component, IPoolable, IWeapon
    {
        foreach (IWeapon activeWeapon in activeWeapons)
        {
            var weapon = activeWeapon as T;
            if (weapon)
            {
                PoolManager.Instance.Return(poolKey, weapon);
            }
        }

        activeWeapons.Clear();
    }
}
