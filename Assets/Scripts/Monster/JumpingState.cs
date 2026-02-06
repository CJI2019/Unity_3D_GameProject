using UnityEngine;

public class JumpingState : IMonsterState
{
     private MonsterController _monster;

    public JumpingState(MonsterController monster) => _monster = monster;

    public void Enter() 
    {
        if (_monster.DebugOn) Debug.Log("몬스터: 점프중...");
    }

    public void Execute() { }

    public void FixedExecute() { }

    public void Exit() { }
}
