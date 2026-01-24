using UnityEngine;

public class ItemSensor : MonoBehaviour
{
    AbilityDataBaseSO abilityDB;
    AbilityData abilityData;
    Collider ownerCollider;
    CapsuleCollider thisCollider;
    const string STRING_PICKUP = "PickUp";
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
        if(gam == null)
        {
            Debug.Log("gam is null");
        }
        gam.OnInit += GameStartAbility;  
    }

    void OnDisable()
    {
        var gam = GameAbilityManager.Instance;
        gam.OnInit -= GameStartAbility;
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

    public void UpdateLevel(int level)
    {
        this.level = level;

        thisCollider.radius = originRadius;
        thisCollider.height = originHeight;

        AbilityData ad = abilityDB.GetAbilityDataByPassiveLevel(AbilityType.ITEMRANGE,level);
        Debug.Log(ad.name + "를 " + ad.level + "로 설정");
        this.abilityData = ad;

        thisCollider.radius *= 1.0f + this.abilityData.damage / 100;
        thisCollider.height *= 1.0f + this.abilityData.damage / 100;
    }
}
