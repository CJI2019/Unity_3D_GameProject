using System.Collections;
using UnityEngine;

public class SwordWeapon : WeaponBase
{
    [SerializeField] float speed       = 3f;
    [SerializeField] float attackDelay = 0.1f;

    Transform playerModelTransform;
    ParticleSystem slashVFX;
    Collider[] hits = new Collider[100]; 

    Vector3 defaultWeaponScale;

    public void Initialize(SwordWeaponManager myMgr,float weaponScale, long damage)
    {
        manager = myMgr;

        this.damage = damage;

        if(playerModelTransform == null)
        {
            playerModelTransform = owner.GetComponent<PlayerController>().GetPlayerModelTransform();
        }

        var offsetVector = (playerModelTransform.right + playerModelTransform.up).normalized
            * (Random.value - 0.5f) * weaponScale;
        
        transform.position = owner.position + offsetVector;
        transform.rotation = Quaternion.LookRotation(playerModelTransform.forward);
        transform.localScale = defaultWeaponScale * weaponScale;

        slashVFX.Play();

        StartCoroutine(AttackCoroutine(
            owner.position,
            playerModelTransform.forward,
            transform.localScale.x * 2f,
            220f
        ));
    }

    protected override void Awake()
    {
        base.Awake();
        slashVFX = GetComponent<ParticleSystem>();
        defaultWeaponScale = transform.localScale;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void WeaponUpdate() 
    {
        if (!slashVFX.IsAlive(true))
        {
            manager.DeActiveWeapon(this);
        }
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

            float dot = Vector3.Dot(forward, dir);

            if (dot >= cosHalfAngle)
            {
                //Debug.DrawLine(center, hit.transform.position, Color.green, 1.0f);
                var damageble = hit.GetComponent<IDamageables>();
                damageble?.TakeDamage(owner.transform,damage);
            }
        }
    }
}
