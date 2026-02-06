using UnityEngine;

public class ChaseState : IMonsterState
{
    private MonsterController _monster;

    public ChaseState(MonsterController monster) => _monster = monster;

    public void Enter() 
    {
        if (_monster.DebugOn)
        {
            Debug.Log("몬스터: 타겟 추적 중...");
        }
    }

    public void Execute()
    {
        Vector3 direction = _monster.GetTargetDirection();

        // 플레이어가 감지 범위 안에 들어오면 추격 상태로 변경
        if (_monster.NeadClimb(direction))
        {
            _monster.ChangeState(new ClimbState(_monster));
            return;
        }

        _monster.AgentDirectionMove(direction);
    }

    public void FixedExecute()
    {
        Vector3 direction = _monster.GetTargetDirection();

        if (_monster.RayJumpCheck(direction))
        {
            _monster.ChangeState(new JumpState(_monster));
        }
    }

    public void Exit() { }
}
