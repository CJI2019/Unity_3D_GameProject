using System.Data.Common;

public interface IWeapon
{
    public long damage {get;set;}

    public void SetDamage(long damage)
    {
        this.damage = damage;
    }
}
