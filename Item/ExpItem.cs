using Unity.VisualScripting;
using UnityEngine;

public class ExpItem : PickUp
{
    [SerializeField] int exp = 1;

    protected override void PickUpLogic(Collider player)
    {
        var playerAbility = player.GetComponent<PlayerAbility>();
        playerAbility?.AddExp(exp);

        DropItemManager.Instance.ItemDeSpawn(this);
    }
}
