using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageables
{
    [SerializeField] protected long hp = 10;

    public void TakeDamage(long amount)
    {
        hp -= amount;
        Debug.Log($"{amount} 만큼의 데미지를 받았습니다. 현재 HP : {hp}");
        if (hp <= 0)
        {
            DeathLogic();
            Destroy(this.gameObject);
        }
    }

    protected abstract void DeathLogic();
}
