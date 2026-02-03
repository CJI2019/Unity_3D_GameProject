using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageables , IPoolable
{
    [SerializeField] protected long maxHp = 10;
    [SerializeField] protected long hp = 10;
    protected bool isDead = false;

    public void OnDespawn()
    {
    }

    public void OnSpawn()
    {
        isDead = false;
        hp = maxHp;
    }

    public virtual void TakeDamage(long amount)
    {
        if(isDead) return;
    
        hp -= amount;

        if (hp <= 0)
        {
            DeathLogic();
            isDead = true;
        }
    }

    protected abstract void DeathLogic();
}
