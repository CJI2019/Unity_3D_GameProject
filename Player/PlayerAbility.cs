using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerAbility : MonoBehaviour
{
    //Event
    public event EventHandler<OnAddExpArgs> OnAddExp;
    public event Action<int> OnLevelUp;
    Dictionary<AbilityType, AbilityData> holdAbilitys = new Dictionary<AbilityType, AbilityData>();
    PlayerWeapon playerWeapon;
    //PlayerAbility 는 매니저를 통해 랜덤 로직을 거쳐 데이터를 뽑는 작업이 필요하다.
    // 예) 무기의 종류 여러개 중 1개를 획득 등
    // 매니저에는 게임에서 능력을 어떻게 가져올 지를 정하는 메서드를 둔다.
    // 매니저와 PlayerAbility를 분리하여 코드의 복잡성을 줄이는 이점도 있다.
    GameAbilityManager gameAbilityMgr;
    ItemSensor itemSensor;

    int playerLevel = 1;
    int playerEXP = 0;
    int playerLevelMaxEXP = 20;

    void Start()
    {
        itemSensor = GetComponentInChildren<ItemSensor>();
    }
    void OnEnable()
    {
        gameAbilityMgr = GameAbilityManager.Instance;
        gameAbilityMgr.OnInit += GameStartAbility;  
    }

    void OnDisable()
    {
        gameAbilityMgr.OnInit -= GameStartAbility;
    }

    void GameStartAbility()
    {
        playerWeapon = GetComponent<PlayerWeapon>();

        AbilityData weapon = GameAbilityManager.Instance.GetRandomWeapon();
        playerWeapon.LevelUpWeapon(weapon.abilityType, weapon.level);
        SyncAbility(weapon);
        UpgradeItemSensor();
    }

    void SyncAbility(AbilityData data)
    {
        holdAbilitys[data.abilityType] = data;
    }

    void UpgradeItemSensor()
    {
        itemSensor.UpdateLevel(1);
    }

    public void AddExp(int exp)
    {
        playerEXP += exp;
        if (playerLevelMaxEXP <= playerEXP)
        {
            playerEXP = 0;
            playerLevel += 1;
            OnLevelUp?.Invoke(playerLevel);
            gameAbilityMgr.ChanceSelectAbility();
        }

        OnAddExp?.Invoke(this,new OnAddExpArgs(playerEXP,playerLevelMaxEXP));
    }
}
