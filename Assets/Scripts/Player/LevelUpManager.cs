using UnityEngine;

public class LevelUpManager : SceneSingleton<LevelUpManager>
{
    [SerializeField] UILevelUpSelect levelUpSelectUI;
    
    PlayerAbility player;
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
