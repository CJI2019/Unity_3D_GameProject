using UnityEngine;

public class ExpItem : PickUp
{
    [SerializeField] int exp = 1;

    public override void PickUpLogic(Collider player)
    {
        var playerAbility = player.GetComponent<PlayerAbility>();
        playerAbility?.AddExp(exp);

        DropItemManager.Instance.ItemDeSpawn(this);
    }

    public void SetExp(int exp)
    {
        this.exp = exp;
    }
}
