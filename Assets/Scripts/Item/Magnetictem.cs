using UnityEngine;

public class MagneticItem : PickUp
{
    ItemType itemType = ItemType.MAGNETIC;
    public override void PickUpLogic(Collider player)
    {
        var playerAbility = player.GetComponent<PlayerAbility>();
        playerAbility.GetItem(itemType);

        DropItemManager.Instance.ItemDeSpawn(this);
    }
}
