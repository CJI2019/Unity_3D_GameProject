using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityData {
    public string id             = "";
    public string name           = "";
    public string type           = "";
    public string weaponType     = "";
    public int level             = 0;
    public int weaponCount       = 0;
    public int damage            = 0;
    public string passiveType    = "";
    public string strAbilityType = "";
    public AbilityType abilityType => ParseAbilityType(strAbilityType);
    
    public AbilityType ParseAbilityType(string strAbilityType)
    {
        switch (strAbilityType)
        {
            case "PROJECTILE":
                return AbilityType.BULLET;
            case "ORBIT":
                return AbilityType.ORBIT;
            case "ITEMRANGE":
                return AbilityType.ITEMRANGE;
        }

        return 0;
    }
}


public enum AbilityType 
{ 
    BULLET,ORBIT,ITEMRANGE
}

[System.Serializable]
[CreateAssetMenu(fileName = "AbilityDataBaseSO", menuName = "Scriptable Objects/AbilityDataBaseSO")]
public class AbilityDataBaseSO : ScriptableObject
{

    public List<AbilityData> allAbility;

    Dictionary<string, AbilityData> idAccessTable                                     = new();
    // 능력 종류로 접근하는 데이터(무기 또는 패시브)
    Dictionary<string, List<AbilityData>> typeAccessTable                             = new();
    // [공통 종류][레벨] 로 접근하는 데이터
    Dictionary<AbilityType, Dictionary<int, AbilityData>> abilityTypeLevelAccessTable = new();
    // [무기 종류][레벨]로 접근하는 데이터
    Dictionary<AbilityType, Dictionary<int, AbilityData>> weaponTypeLevelAccessTable  = new();
    // [패시브 종류][레벨]로 접근하는 데이터
    Dictionary<AbilityType, Dictionary<int, AbilityData>> passiveTypeLevelAccessTable = new();

    public void Init()
    {
        foreach(var data in allAbility)
        {
            // ID로 접근하는 데이터
            idAccessTable[data.id] = data;
            if (!typeAccessTable.ContainsKey(data.type))
            {
                typeAccessTable[data.type] = new List<AbilityData>();
            }
            
            typeAccessTable[data.type].Add(data);
            
            InitAbilityTable(data);
        }

    }

    void InitAbilityTable(AbilityData data)
    {
        if (data.type == GameAbilityManager.ACTIVE_TYPE)
        {
            if (!weaponTypeLevelAccessTable.ContainsKey(data.abilityType))
            {
                weaponTypeLevelAccessTable[data.abilityType] = new Dictionary<int, AbilityData>();
            }
            weaponTypeLevelAccessTable[data.abilityType][data.level] = data;
        }
        else if (data.type == GameAbilityManager.PASSIVE_TYPE)
        {
            if (!passiveTypeLevelAccessTable.ContainsKey(data.abilityType))
            {
                passiveTypeLevelAccessTable[data.abilityType] = new Dictionary<int, AbilityData>();
            }
            passiveTypeLevelAccessTable[data.abilityType][data.level] = data;
        }

        if (!abilityTypeLevelAccessTable.ContainsKey(data.abilityType))
        {
            abilityTypeLevelAccessTable[data.abilityType] = new Dictionary<int, AbilityData>();
        }
        abilityTypeLevelAccessTable[data.abilityType][data.level] = data;
    }

    public AbilityData GetAbilityData(string abilityId)
    {
        return idAccessTable[abilityId];
    }
    public AbilityData GetAbilityData(AbilityType abilityType,int level)
    {
        if(!abilityTypeLevelAccessTable[abilityType].ContainsKey(level))
            return null;
        return abilityTypeLevelAccessTable[abilityType][level];
    }
    public List<AbilityData> GetAllAbilityData()
    {
        return allAbility;
    }

    // AbilityType 으로 데이터 리스트를 반환한다.(액티브, 패시브 등)
    public List<AbilityData> GetAbilityDataByAbilityType(string abilityType)
    {
        return typeAccessTable[abilityType];
    }
    // 무기의 종류와 레벨에 해당하는 데이터를 반환한다.
    public AbilityData GetAbilityDataByWeaponLevel(AbilityType weaponType, int level)
    {
        if(!weaponTypeLevelAccessTable.ContainsKey(weaponType)) return null;

        return weaponTypeLevelAccessTable[weaponType][level];
    }
    // 패시브의 종류와 레벨에 해당하는 데이터를 반환한다.
    public AbilityData GetAbilityDataByPassiveLevel(AbilityType passiveType, int level)
    {
        if(!passiveTypeLevelAccessTable.ContainsKey(passiveType)) return null;

        return passiveTypeLevelAccessTable[passiveType][level];
    }
}
