using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;

public class SwordWeapon : WeaponBase
{
    [SerializeField] float speed       = 3f;
    [SerializeField] float attackDelay = 0.1f;

    Transform playerModelTransform;
    ParticleSystem slashVFX;
    Collider[] hits = new Collider[100]; // 몬스터 충돌체 저장용
    Vector3 dir;
    Vector3 attackStartPos;

    protected override void Awake()
    {
        base.Awake();
        slashVFX = GetComponent<ParticleSystem>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void Initialize(SwordWeaponManager myMgr, long damage)
    {
        manager = myMgr;

        this.damage = damage;
        transform.position = owner.position + Random.onUnitSphere; 
        attackStartPos = transform.position;

        if(playerModelTransform == null)
        {
            playerModelTransform = owner.GetComponent<PlayerController>().GetPlayerModelTransform();
        }
        transform.rotation = Quaternion.LookRotation(playerModelTransform.forward);
         
        slashVFX.Play();

        StartCoroutine(AttackCoroutine(
            owner.position,
            playerModelTransform.forward,
            transform.localScale.x * 2f,
            220f
        ));
    }

    private void Update()
    {
        if (!slashVFX.IsAlive(true))
        {
            manager.DeActiveWeapon(this);
        }
    }

    public override void Attack(Collider other)
    {
        base.Attack(other);
        manager.DeActiveWeapon(this);
        //Hit Effect 발동 등 추가
    }

    IEnumerator AttackCoroutine(Vector3 center, Vector3 forward, float radius, float angle)
    {
        yield return new WaitForSeconds(attackDelay);

        int hitCount = Physics.OverlapSphereNonAlloc(center, radius, hits, monsterLayer, QueryTriggerInteraction.Ignore);

        float cosHalfAngle = Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);

        for(int i = 0; i < hitCount; ++i)
        {
            var hit = hits[i];
            Vector3 dir = (hit.transform.position - center).normalized;

            // forward와 dir 사이의 코사인 값
            float dot = Vector3.Dot(forward.normalized, dir);

            if (dot >= cosHalfAngle)
            {
                var damageble = hit.GetComponent<IDamageables>();
                damageble?.TakeDamage(damage);
            }
        }
    }
}
