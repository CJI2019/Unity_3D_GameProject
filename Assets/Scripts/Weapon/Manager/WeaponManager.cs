using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WeaponManager : MonoBehaviour
{
    protected List<IWeapon> spawnedWeapons;
    string poolKey;
    protected AbilityData weaponData;

    //추상 메서드
    public virtual void UpdateWeapons()
    {

    }

    void Awake()
    {
        spawnedWeapons = new List<IWeapon>();
    }

    // 활성화된 무기를 활성화 그룹에 추가한다.
    void ActiveWeapon(IWeapon iWeapon)
    {
        spawnedWeapons.Add(iWeapon);
    }

    // 무기 데이터를 설정한다.
    // 영구 유지 무기와 활성/비활성 무기에 따라 처리가 다를 수 있다.
    // 가상함수로 각 매니저 클래스에서 구현코드를 수정하도록 한다.
    // 영구 유지 무기 비율 < 활성/비활성 무기 비율
    public virtual void SetWeaponData(AbilityData abilityData)
    {
        weaponData = abilityData;

        if(weaponData.level == 1)
        {
            enabled = true;
        }
    }
   
    protected void SetPoolKey(string poolKey)
    {
        this.poolKey = poolKey;
    }

    protected void RegisterWeapon<T>(T prefab,string poolKey) where T : Component, IPoolable, IWeapon
    {
        var weaponPool = new GenericObjectPool<T>(prefab, 10, transform);
        PoolManager.Instance.RegisterPool(poolKey, weaponPool);
    }

    protected T AddWeapon<T>() where T : Component, IPoolable, IWeapon
    {
        T weapon = PoolManager.Instance.Get<T>(poolKey);
        weapon.transform.SetParent(transform);

        return weapon;
    }
    public void DeActiveWeapon<T>(T weapon) where T : Component, IPoolable, IWeapon
    {
        PoolManager.Instance.Return(poolKey, weapon);
    }

    protected void AddWeapon<T>(int count) where T : Component, IPoolable, IWeapon
    {
        int beforeCount = spawnedWeapons.Count;
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

    // 무기 종류를 활성 그룹에서 제거한다.
    protected void DeActiveWeapon<T>() where T : Component, IPoolable, IWeapon
    {
        foreach (IWeapon activeWeapon in spawnedWeapons)
        {
            var weapon = activeWeapon as T;
            if (weapon)
            {
                PoolManager.Instance.Return(poolKey, weapon);
            }
        }

        spawnedWeapons.Clear();
    }

}
