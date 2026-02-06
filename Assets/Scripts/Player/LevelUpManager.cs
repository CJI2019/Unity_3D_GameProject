using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : SceneSingleton<LevelUpManager>
{
    [SerializeField] UILevelUpSelect levelUpSelectUI;
    PlayerAbility player;

    // 레벨 업 후 능력 선택 하기 전까지 선택창을 갱신하지 않는다.
    // 본래 목적은 레벨 업 후 게임을 정지 시켜서 경험치를 얻는 것을 막아 능력을
    // 선택 후 게임이 재개되도록 해야한다. 또는 선택 갱신 큐를 두어 큐에 삽입한 만큼
    // 선택 횟수를 늘리는 방법도 있음.
    bool abilitySelectWait = false;

    void Start()
    {
        player = FindFirstObjectByType<PlayerAbility>();
        player.OnLevelUp += OnSelectAbilityWindow;
    }

    void OnDisable()
    {
        player.OnLevelUp -= OnSelectAbilityWindow;
    }

    void OnSelectAbilityWindow(int level)
    {
        if(abilitySelectWait) return;
        
        var gam = GameAbilityManager.Instance;
        var randomList = gam.RandomAbilityList(player);
        if(randomList.Count == 0) return;
        levelUpSelectUI.UpdateSelect(randomList);

        abilitySelectWait = true;
        GameManager.Instance.GamePaused(true);
    }

    public void ApplyChoiceAbility(AbilityData abilityData)
    {
        int level = 1;
        var holdAbilitys = player.GetAbilities();

        if (holdAbilitys.ContainsKey(abilityData.abilityType))
        {
            level = holdAbilitys[abilityData.abilityType].level + 1;
        }

        var gaMgr = GameAbilityManager.Instance;
        var newAbilityData = gaMgr.GetAbilityData(abilityData.abilityType,level);
        if (newAbilityData == null) return;
        
        player.SyncAbility(newAbilityData);
        abilitySelectWait = false;
        
        GameManager.Instance.GamePaused(false);
    }
}
