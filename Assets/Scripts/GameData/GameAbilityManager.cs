using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameAbilityManager : MonoBehaviour
{
    private static GameAbilityManager _instance;
    public static GameAbilityManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameAbilityManager>();
                if(_instance == null)
                {
                    Debug.Log("객체를 찾을 수 없습니다.");
                }
            }
            return _instance;
        }
    }
    public AbilityDataBaseSO abilityDataBase;

    //Event
    public event Action OnInit;

    public const string ACTIVE_TYPE = "ACTIVE";
    public const string PASSIVE_TYPE = "PASSIVE";

    void Start()
    {
        abilityDataBase.Init();
        StartCoroutine(InitCompleteEvent());
    }

    IEnumerator InitCompleteEvent()
    {
        yield return new WaitForSeconds(1f);
        OnInit.Invoke();
    }

    public AbilityData GetAbilityData(string abilityId)
    {
        return abilityDataBase.GetAbilityData(abilityId);
    }
    
    public AbilityData GetAbilityData(AbilityType abilityType,int level)
    {
        return abilityDataBase.GetAbilityData(abilityType,level);
    }

    public AbilityData GetRandomWeapon()
    {
        var weaponList = abilityDataBase.GetAbilityDataByAbilityType(ACTIVE_TYPE);

        var level1List = weaponList.FindAll(p => p.level == 1);

        var selectWeaponIndex = UnityEngine.Random.Range(0,level1List.Count);

        return level1List[selectWeaponIndex];
    }

    public AbilityData GetRandomPassive(int level)
    {
        var passiveList = abilityDataBase.GetAbilityDataByAbilityType(PASSIVE_TYPE);
        var passiveUniqueList = passiveList.DistinctBy(i => i.abilityType).ToList();
        var selectPassiveIndex = UnityEngine.Random.Range(0,passiveUniqueList.Count);

        AbilityData selectPassive = passiveUniqueList[selectPassiveIndex];
        abilityDataBase.GetAbilityDataByPassiveLevel(selectPassive.abilityType,level);
        return null;
    }

    public AbilityData GetWeaponData(AbilityType weaponType,int level)
    {
        return abilityDataBase.GetAbilityDataByWeaponLevel(weaponType,level);
    }

    public List<AbilityData> RandomAbilityList()
    {
        var db = abilityDataBase;
        var allAbility = db.GetAllAbilityData();
        // 중복 종류 제거
        var uniqueAbilitys = allAbility.DistinctBy(x => x.abilityType).ToList();
        var rand = new System.Random();
        var selected = uniqueAbilitys.OrderBy(x => rand.Next()).Take(3).ToList();

        return selected;
    }
}
