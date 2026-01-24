using UnityEditor.Search;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour ,IAttacker , IPoolable , IWeapon
{
    [Header("Common Settings")]
    [SerializeField] protected float attackRate = 2f; // 공격 주기

    protected Transform owner;
    protected float timer;
    protected const string MONSTER_STRING = "Monster";

    public virtual float AttackRange => throw new System.NotImplementedException();
    public float AttackCoolDown => attackRate;
    [SerializeField] private long _damage = 10;
    public long damage { get => _damage; set => _damage = value; }

    public abstract void Attack();
    protected virtual void Awake()
    {
        owner = FindFirstObjectByType<PlayerWeapon>().transform;
    }
    protected virtual void Start()
    {
        
    }
    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer >= attackRate)
        {
            timer = 0f;
            Attack();
        }
    }
    protected virtual void HandleTrigger(Collider other)
    {
        if (other.CompareTag(MONSTER_STRING))
        {
            var entity = other.GetComponent<LivingEntity>();
            entity.TakeDamage(damage);
        }
    }
    public virtual void OnSpawn()
    {
        // throw new System.NotImplementedException();
    }
    public virtual void OnDespawn()
    {
        // throw new System.NotImplementedException();
    }

    void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other);
    }

}