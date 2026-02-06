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

    public const string ACTIVE_TYPE  = "ACTIVE";
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
        var weaponList        = abilityDataBase.GetAbilityDataByAbilityType(ACTIVE_TYPE);
        var level1List        = weaponList.FindAll(p => p.level == 1);
        var selectWeaponIndex = UnityEngine.Random.Range(0,level1List.Count);

        return level1List[selectWeaponIndex];
    }

    public AbilityData GetRandomPassive(int level)
    {
        var passiveList        = abilityDataBase.GetAbilityDataByAbilityType(PASSIVE_TYPE);
        var passiveUniqueList  = passiveList.DistinctBy(i => i.abilityType).ToList();
        var selectPassiveIndex = UnityEngine.Random.Range(0,passiveUniqueList.Count);

        AbilityData selectPassive = passiveUniqueList[selectPassiveIndex];
        abilityDataBase.GetAbilityDataByPassiveLevel(selectPassive.abilityType,level);
        return null;
    }

    public AbilityData GetWeaponData(AbilityType weaponType,int level)
    {
        return abilityDataBase.GetAbilityDataByWeaponLevel(weaponType,level);
    }

    public List<AbilityData> RandomAbilityList(PlayerAbility playerAbility)
    {
        Dictionary<AbilityType, AbilityData> playerAbilities = playerAbility.GetAbilities();
        var db = abilityDataBase;

        // 1. 모든 가능한 능력 데이터를 가져옵니다. (ScriptableObject의 원본 데이터를 수정하지 않도록 방어적 복사)
        var allPossibleAbilities = new List<AbilityData>(db.GetAllAbilityData());

        // 2. 플레이어에게 유효한 선택지가 될 수 있는 능력들만 필터링합니다.
        //    유효한 선택지는 다음 중 하나입니다.
        //    a) 플레이어가 아직 가지고 있지 않은 능력의 1레벨.
        //    b) 플레이어가 이미 가지고 있는 능력의 다음 레벨 업그레이드.
        var candidateAbilities = new List<AbilityData>();

        foreach (var abilityFromDB in allPossibleAbilities)
        {
            if (playerAbilities.TryGetValue(abilityFromDB.abilityType, out var playerCurrentAbility))
            {
                // 플레이어가 이미 해당 능력 타입을 가지고 있습니다. 다음 레벨 업그레이드인지 확인합니다.
                if (abilityFromDB.level == playerCurrentAbility.level + 1)
                {
                    candidateAbilities.Add(abilityFromDB);
                }
            }
            else
            {
                // 플레이어가 아직 해당 능력 타입을 가지고 있지 않습니다. 1레벨만 제공합니다.
                if (abilityFromDB.level == 1)
                {
                    candidateAbilities.Add(abilityFromDB);
                }
            }
        }

        // 고유 선택지 중에서 무작위로 최대 3개를 선택합니다.
        var selected = candidateAbilities.OrderBy(x => UnityEngine.Random.Range(0, int.MaxValue)).Take(3).ToList();

        return selected;
    }
}
