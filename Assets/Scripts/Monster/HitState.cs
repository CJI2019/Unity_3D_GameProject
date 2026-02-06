using UnityEngine;

public class HitState : IMonsterState
{
    private MonsterController _monster;
    private Transform attacker;
    public HitState(MonsterController monster,Transform attacker)
    {
        _monster = monster;
        this.attacker = attacker;
    }

    public void Enter() 
    {
        if (_monster.DebugOn) Debug.Log("몬스터: 피격!");
        _monster.SetHitState();
    }

    public void Execute()
    {
        Vector3 direction = _monster.GetTargetDirection();

        _monster.rigidBody.AddForce((-direction+Vector3.up).normalized * 7f, ForceMode.Impulse);

        _monster.ChangeState(new JumpingState(_monster));
    }

    public void FixedExecute() { }

    public void Exit()
    {
    }

    public bool CanExit(IMonsterState nextState){ return true; }
}