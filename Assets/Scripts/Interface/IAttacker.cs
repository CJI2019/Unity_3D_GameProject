using UnityEngine;

public interface IAttacker
{
	void Attack(Collider other);        // 공격 실행
	float AttackRange { get; } // 사거리 (AI 판정용)
	float AttackCoolDown { get; } // 쿨타임
}