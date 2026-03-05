using System.Collections;
using UnityEngine;

public class ThunderStrikeWeapon : WeaponBase
{
    [SerializeField] float attackDelay = 0.1f;

    ParticleSystem thunderStrikeVFX;
    Collider[] hits = new Collider[100];
    Vector3 defaultWeaponScale;

    public void Initialize(ThunderStrikeWeaponManager myMgr, long damage,float weaponScale)
    {
        manager = myMgr;
        this.damage = damage;

        transform.SetParent(PoolManager.Instance.transform);
        Vector3 randomPos = owner.position + Random.onUnitSphere * AttackRange;
        RaycastHit hitInfo;
        if (!Physics.Raycast(randomPos, Vector3.down, out hitInfo, 100f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
        {
            if (!Physics.Raycast(randomPos, Vector3.up, out hitInfo, 100f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
            {
                manager.DeActiveWeapon(this);
                return;
            }
        }
        transform.position = hitInfo.point;
        transform.localScale = defaultWeaponScale * weaponScale;

        thunderStrikeVFX.Play();

        StartCoroutine(AttackCoroutine(
            transform.position,
            transform.localScale.x * 2f
        ));
    }

    protected override void Awake()
    {
        base.Awake();
        thunderStrikeVFX = GetComponent<ParticleSystem>();
        defaultWeaponScale = transform.localScale;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void WeaponUpdate() 
    {
        if (!thunderStrikeVFX.IsAlive(true))
        {
            manager.DeActiveWeapon(this);
        }
    }

    IEnumerator AttackCoroutine(Vector3 center, float radius)
    {
        yield return new WaitForSeconds(attackDelay);

        int hitCount = Physics.OverlapSphereNonAlloc(center, radius, hits, monsterLayer, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; ++i)
        {
            var hit = hits[i];

            if (hit.transform.position.y > center.y)
            {
                Debug.DrawLine(center, hit.transform.position, Color.green, 1.0f);
                var damageble = hit.GetComponent<IDamageables>();
                damageble?.TakeDamage(owner.transform, damage);
            }
        }
    }
}
