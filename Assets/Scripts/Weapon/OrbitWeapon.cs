using UnityEditor.UI;
using UnityEngine;

public class OrbitWeapon : WeaponBase
{
    [SerializeField] float distance = 4f;

    float rotateSpeed = 100f;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize(float angle,float rotateSpeed, long damage)
    {
        // 회전 시작 로컬 위치 및 로컬 각도 조정
        transform.localPosition = new Vector3(0f,0f,distance);        
        transform.RotateAround(owner.position, Vector3.up, angle);

        this.damage = damage;
        this.rotateSpeed = rotateSpeed;
    }

    void Update()
    {
        // 회전하면서 닿는 모든 생명체에게 데미지를 가하므로 추가적인 공격 로직은 필요없음.
        OrbitAroundTarget();
    }

    void OrbitAroundTarget()
    {
        // 플레이어 주위를 공전
        transform.RotateAround(owner.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
