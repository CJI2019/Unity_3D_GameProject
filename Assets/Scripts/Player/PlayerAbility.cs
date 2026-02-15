using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbility : LivingEntity
{
    [SerializeField] bool godMode = false;
    //Event
    public event EventHandler<OnAddExpArgs> OnAddExp;
    public event Action<int> OnLevelUp;
    
    GameAbilityManager gameAbilityMgr;
    PlayerController playerController;
    PlayerWeapon playerWeapon;
    ItemSensor itemSensor;

    Dictionary<AbilityType, AbilityData> myAbilities = new Dictionary<AbilityType, AbilityData>();

    int playerLevel = 1;
    int playerEXP = 0;
    int playerLevelMaxEXP = 10;

    void Start()
    {
        itemSensor = GetComponentInChildren<ItemSensor>();
        playerController = GetComponent<PlayerController>();
        hp = maxHp;
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
        SyncAbility(weapon);
    }

    public void SyncAbility(AbilityData data)
    {
        myAbilities[data.abilityType] = data;

        switch (data.abilityType)
        {
            // Active 무기
            case AbilityType.BULLET:
            case AbilityType.ORBIT:
            case AbilityType.SWORD:
            case AbilityType.THUNDERSTRIKE:
                playerWeapon.LevelUpWeapon(data.abilityType, data.level);
            break;
            // Passive 버프
            case AbilityType.ITEMRANGE:
                itemSensor.SetAbilityData(data);
            break;
            default:
                Debug.LogError("Unknown Ability Type");
            break;
        }
    }

    public void AddExp(int exp)
    {
        playerEXP += exp;
        if (playerLevelMaxEXP <= playerEXP)
        {
            playerEXP = 0;
            playerLevel += 1;
            OnLevelUp?.Invoke(playerLevel);
            playerLevelMaxEXP = (int)(playerLevelMaxEXP * 1.5);
        }

        OnAddExp?.Invoke(this,new OnAddExpArgs(playerEXP,playerLevelMaxEXP));
    }

    public void GetItem(ItemType itemType)
    {
        switch(itemType)
        {
            case ItemType.MAGNETIC:
                // 기본적으로 비활성 객체는 가져오지 않음.
                var expItems = FindObjectsByType<ExpItem>(FindObjectsSortMode.None);
                
                StartCoroutine(DropItemManager.Instance.MagneticLogic(expItems.ToList(),transform));
                break;
            default:
                Debug.LogError("Unknown Item Type");
                break;
        }
    }

    public Dictionary<AbilityType, AbilityData> GetAbilities()
    {
        return myAbilities;
    }

    public override void TakeDamage(Transform transform, long amount)
    {
        base.TakeDamage(transform,amount);
        // Debug.Log($"Player HP : {hp}");
    }

    protected override void DeathLogic()
    {
        if (godMode)
        {
            Debug.Log("God모드 활성 중");
            return;
        }

        // Debug.Log("Player Dead");
        playerController.GetPlayerAnimation().SetBool("IsDead", true);
    }
}
