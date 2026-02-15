using UnityEngine;

public class ItemSensor : MonoBehaviour
{
    AbilityDataBaseSO abilityDB;
    AbilityData abilityData;
    Collider ownerCollider;
    CapsuleCollider thisCollider;

    const string STRING_PICKUP = "PickUp";
    public int Level {get => level;}
    int level = 0;
    float originRadius;
    float originHeight;

    void Awake()
    {
        var ownerPlayer = GetComponentInParent<CharacterController>();
        ownerCollider = ownerPlayer.GetComponent<Collider>();
        thisCollider = GetComponent<CapsuleCollider>();

        originRadius = thisCollider.radius;
        originHeight = thisCollider.height;
    }

    void OnEnable()
    {
        var gam = GameAbilityManager.Instance;
        gam.OnInit += GameStartAbility;  
    }

    void OnDisable()
    {
        var gam = GameAbilityManager.Instance;
        if (gam != null)
        {
            gam.OnInit -= GameStartAbility;
        }
    }

    void GameStartAbility()
    {
        abilityDB = GameAbilityManager.Instance.abilityDataBase;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(STRING_PICKUP))
        {
            var pickUp = other.GetComponent<PickUp>();
            pickUp?.PickUpLogic(ownerCollider);
        }
    }

    public void UpdateSensor()
    {
        thisCollider.radius = originRadius;
        thisCollider.height = originHeight;

        thisCollider.radius *= 1.0f + this.abilityData.damage / 100;
        thisCollider.height *= 1.0f + this.abilityData.damage / 100;
    }

    public void SetAbilityData(AbilityData abilityData)
    {
        this.abilityData = abilityData;
        this.level = abilityData.level;

        UpdateSensor();
        // Debug.Log(abilityData.name + "를 " + abilityData.level + "로 설정");
    }

    public AbilityData GetAbilityData()
    {
        return this.abilityData;
    }
}
