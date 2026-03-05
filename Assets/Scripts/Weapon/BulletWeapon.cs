using UnityEngine;

public class BulletWeapon : WeaponBase
{
    [SerializeField] float speed = 3f;
    Vector3 dir;
    Vector3 attackStartPos;

    int penetrationCount = 1;

    public void Initialize(BulletWeaponManager myMgr,int penetrationCount, long damage)
    {
        manager = myMgr;

        transform.SetParent(PoolManager.Instance.gameObject.transform);

        this.damage             = damage;
        this.penetrationCount   = penetrationCount;
        owner                   = manager.transform;
        transform.position      = owner.position;
        attackStartPos          = transform.position;

        // 가장 가까운 몬스터 방향 찾기 : 위치 초기화 이후 진행되어야함.
        dir = GetDirectionToClosestMonster();
    }

    public override void Attack(Collider other)
    {
        base.Attack(other);

        --penetrationCount;
        if (penetrationCount <= 0)
        {
            manager.DeActiveWeapon(this);
        }
    }

    protected override void WeaponUpdate()
    {
        if (dir != Vector3.zero)
        {
            transform.position += dir * Time.deltaTime * speed;
        }
        else
        {
            manager.DeActiveWeapon(this);
        }

        // 사정거리 벗어나면 비활성화
        if (Vector3.Distance(attackStartPos, transform.position) > AttackRange)
        {
            manager.DeActiveWeapon(this);
        }
    }

    GameObject FindClosestMonster()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(MONSTER_STRING);
        GameObject closest   = null;
        float minDistance    = Mathf.Infinity;
        Vector3 currentPos   = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPos, enemy.transform.position);
            if (distance < minDistance && AttackRange >= distance)
            {
                minDistance = distance;
                closest     = enemy;
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
    
}
