using System;
using System.Collections;
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
    // 게임 시작 시 무기 하나를 고르는 시스템이 필요하다.
    // [테스트] 게임 시작 시 무기 하나를 랜덤으로 골라 반환한다.
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
        Debug.Log(passiveUniqueList.Count + "개의 패시브가 존재");
        Debug.Log(selectPassive.name + "을 골랐습니다.");
        abilityDataBase.GetAbilityDataByPassiveLevel(selectPassive.abilityType,level);
        return null;
    }

    public AbilityData GetWeaponData(AbilityType weaponType,int level)
    {
        return abilityDataBase.GetAbilityDataByWeaponLevel(weaponType,level);
    }

    public void ChanceSelectAbility()
    {
        var db = abilityDataBase;
        // 레벨업 하거나 보물상자를 획득했을 경우
        // 랜덤하게 3개의 능력을 가져와서 UI에 출력하고, 1개를 고를 수 있도록 한다.
        // 3개의 능력은 이 함수에서 뽑도록 하고, UI 로 해당 능력 데이터 리스트를 넘겨주도록 한다.
        // [테스트] 능력치 3개를 랜덤하게 뽑아와서 1종을 랜덤하게 골라 적용한다.
        var activeList = db.GetAbilityDataByAbilityType(ACTIVE_TYPE);
        var passiveList = db.GetAbilityDataByAbilityType(PASSIVE_TYPE);
        activeList.DistinctBy(i => i.abilityType).ToList();
        passiveList.DistinctBy(i => i.abilityType).ToList();
    }
}
