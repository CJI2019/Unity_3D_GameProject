using UnityEngine;

public class JumpState : IMonsterState
{
    private MonsterController _monster;

    public JumpState(MonsterController monster) => _monster = monster;

    public void Enter() 
    {
        if (_monster.DebugOn) Debug.Log("몬스터: 점프...");
        _monster.SetJumpState();
    }

    public void Execute()
    {
        Vector3 direction = _monster.GetTargetDirection();

        _monster.AddForceToDirUp(direction);
        _monster.ChangeState(new JumpingState(_monster));
    }

    public void FixedExecute() { }

    public void Exit() { }
    public bool CanExit(IMonsterState nextState)
    { 
        if(nextState is HitState) return false;
        return true; 
    }

}
