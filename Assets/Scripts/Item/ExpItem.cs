using UnityEngine;

public class ExpItem : PickUp
{
    [SerializeField] int exp = 1;
    MeshRenderer meshRenderer;

    public override void PickUpLogic(Collider player)
    {
        var playerAbility = player.GetComponent<PlayerAbility>();
        playerAbility?.AddExp(exp);

        DropItemManager.Instance.ItemDeSpawn(this);
    }

    public void SetExpItemEntry(ExpItemEntry entry)
    {
        this.exp = entry.exp;
        SetColor(entry.color);
    }

    void SetColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
