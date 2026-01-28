using UnityEngine;

public class BulletWeapon : WeaponBase
{
    [SerializeField] float attackRange = 10.0f;
    [SerializeField] GameObject bulletModel;
    [SerializeField] float speed = 3f;
    Collider weaponCollider;
    Vector3 dir;
    Vector3 attackStartPos;
    public override float AttackRange => attackRange;

    protected override void Awake()
    {
        weaponCollider = GetComponent<Collider>();
    }
    public void Initialize(Transform parent,float delay)
    {
        transform.SetParent(PoolManager.Instance.gameObject.transform);
        bulletModel.SetActive(false);
        this.owner = parent;
        timer -= delay;
    }

    protected override void Update()
    {
        ActiveLogic();
        base.Update();
    }

    void ActiveLogic()
    {
        if (bulletModel.activeSelf)
        {
            if (dir != Vector3.zero)
            {
                transform.position += dir * Time.deltaTime * speed;
            }

            if (Vector3.Distance(attackStartPos, transform.position) > AttackRange)
            {
                bulletModel.SetActive(false);
            }
        }
    }

    GameObject FindClosestMonster()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(MONSTER_STRING);
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPos, enemy.transform.position);
            if (distance < minDistance && AttackRange >= distance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }

    Vector3 GetDirectionToClosestMonster()
    {
        GameObject closest = FindClosestMonster();
        if (closest != null)
        {
            Vector3 direction = (closest.transform.position - transform.position).normalized;
            return direction;
        }
        return Vector3.zero; // 없으면 (0,0,0)
    }

    public override void Attack()
    {
        transform.position = owner.position;
        attackStartPos = transform.position;
        bulletModel.SetActive(true);

        dir = GetDirectionToClosestMonster();
        if(dir == Vector3.zero)
        {
            bulletModel.SetActive(false);
        }
        
        weaponCollider.enabled = true;
    }

    protected override void HandleTrigger(Collider other)
    {
        base.HandleTrigger(other);
        bulletModel.SetActive(false);
        weaponCollider.enabled = false;
    }
}
