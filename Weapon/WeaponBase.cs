using UnityEditor.Search;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour ,IAttacker , IPoolable , IWeapon
{
    [Header("Common Settings")]
    [SerializeField] protected long damage = 10;
    [SerializeField] protected float attackRate = 2f; // 공격 주기

    protected Transform owner;
    protected float timer;
    protected const string MONSTER_STRING = "Monster";

    public virtual float AttackRange => throw new System.NotImplementedException();
    public float AttackCoolDown => attackRate;
    public int level { get; set; } = 1;

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

    protected virtual void HandleTrigger(Collider other)
    {
        if (other.CompareTag(MONSTER_STRING))
        {
            var entity = other.GetComponent<LivingEntity>();
            entity.TakeDamage(damage);
        }
    }
}