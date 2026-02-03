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
    
    // PlayerAbility 는 매니저를 통해 랜덤 로직을 거쳐 데이터를 뽑는 작업이 필요하다.
    // 예) 무기의 종류 여러개 중 1개를 획득 등
    // 매니저에는 게임에서 능력을 어떻게 가져올 지를 정하는 메서드를 둔다.
    // 매니저와 PlayerAbility를 분리하여 코드의 복잡성을 줄이는 이점도 있다.
    GameAbilityManager gameAbilityMgr;
    PlayerController playerController;
    PlayerWeapon playerWeapon;
    ItemSensor itemSensor;

    Dictionary<AbilityType, AbilityData> myAbilities = new Dictionary<AbilityType, AbilityData>();

    int playerLevel = 1;
    int playerEXP = 0;
    int playerLevelMaxEXP = 20;

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

    public Dictionary<AbilityType, AbilityData> GetHoldAbility()
    {
        return myAbilities;
    }

    public override void TakeDamage(long amount)
    {
        base.TakeDamage(amount);
        Debug.Log($"Player HP : {hp}");
    }

    protected override void DeathLogic()
    {
        if (godMode)
        {
            Debug.Log("God모드 활성 중");
            return;
        }

        Debug.Log("Player Dead");
        playerController.GetPlayerAnimation().SetBool("IsDead", true);
    }
}
