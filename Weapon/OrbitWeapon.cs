using UnityEditor.UI;
using UnityEngine;

public class OrbitWeapon : WeaponBase
{
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float distance = 4f;
    [SerializeField] float attackRange = 2f;    // 공격 범위
    LayerMask monsterLayer;
    Collider[] hitColliders = new Collider[20];

    protected override void Start()
    {
        base.Start();

        monsterLayer = LayerMask.GetMask("Monster");
    }
    protected override void Update()
    {
        base.Update();
        OrbitAroundTarget();
    }

    void OrbitAroundTarget()
    {
        // 플레이어 주위를 공전
        transform.RotateAround(target.position, Vector3.up, rotateSpeed * Time.deltaTime);

        // 플레이어와의 거리 유지
        Vector3 desiredPos = (transform.position - target.position).normalized * distance + target.position;
        transform.position = desiredPos;
    }

    protected override void Attack()
    {
        // 범위 내의 몬스터들을 검사
        // OverlapSphereNonAlloc은 메모리 할당(GC)을 하지 않아 성능에 유리함
        int monsterCount = Physics.OverlapSphereNonAlloc(transform.position, attackRange, hitColliders, monsterLayer);

        // 발견된 몬스터들에게 데미지 전달
        for (int i = 0; i < monsterCount; i++)
        {
            // 이전에 만든 Monster 추상 클래스의 TakeDamage 호출
            if (hitColliders[i].TryGetComponent<Monster>(out var monster))
            {
                monster.TakeDamage(damage);
                
                // 공격 이펙트 생성 등 추가 로직
                PlayHitEffect(hitColliders[i].transform.position);
            }
        }
    }

    // 범위 시각화 (에디터 뷰에서 확인용)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void PlayHitEffect(Vector3 position) 
    {
        /* 이펙트 로직 */ 
    }
}
