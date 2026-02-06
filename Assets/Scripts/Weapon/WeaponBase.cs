using UnityEditor.Search;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour , IPoolable , IWeapon
{
    [Header("Common Settings")]
    [SerializeField] private float attackRange  = 10.0f;
    [SerializeField] private long _damage       = 10;

    protected Transform owner;
    protected WeaponManager manager;
    protected const string MONSTER_STRING = "Monster";
    protected LayerMask monsterLayer;

    public float AttackRange => attackRange;
    public long damage { get => _damage; set => _damage = value; }

    public virtual void Attack(Collider other)
    {
        var entity = other.GetComponent<LivingEntity>();
        entity.TakeDamage(owner.transform,damage);
    }

    protected virtual void Awake()
    {
        owner = FindFirstObjectByType<PlayerWeapon>().transform;
        monsterLayer = LayerMask.GetMask(MONSTER_STRING);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void HandleTrigger(Collider other)
    {
        if (other.CompareTag(MONSTER_STRING))
        {
            Attack(other);
        }
    }

    public virtual void OnSpawn()
    {

    }
    public virtual void OnDespawn()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other);
    }

    public void SetAttackRange(float range)
    {
        attackRange = range;
    }
}