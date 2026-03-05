using UnityEngine;

public class ClimbState : IMonsterState
{
    private MonsterController _monster;

    public ClimbState(MonsterController monster) => _monster = monster;

    bool isPendingExit;
    bool rayCastHitObstacle;

    public void Enter() 
    {
        isPendingExit = false;
        rayCastHitObstacle = false;

        _monster.InitClimbState();
        
        if (_monster.DebugOn) Debug.Log("몬스터: 등반중...");
    }

    public void Execute()
    {
        // 잠정적으로 상태가 종료됨을 의미
        // LinearMoveClosedPoint 위치 이동 후 충돌 발생 시 자동으로 상태 초기화
        if(isPendingExit) return;

        if (rayCastHitObstacle)
        {
            _monster.LinearMoveClosedPoint();
            isPendingExit = true;
            return;
        }

        _monster.MoveToUp();
    }

    public void FixedExecute()
    {
        Vector3 direction = _monster.GetTargetDirection();
        rayCastHitObstacle = _monster.RaycastHitsObstacle(direction);
    }

    public void Exit() { }
    public bool CanExit(IMonsterState nextState)
    { 
        if(nextState is HitState) return false;
        return true; 
    }

}
