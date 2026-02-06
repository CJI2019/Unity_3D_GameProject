public interface IMonsterState
{
    void Enter();    // 상태에 진입할 때 (초기화)
    void Execute();  // 상태 중일 때 (Update에서 호출)
    void FixedExecute();  // 상태 중일 때 (FixedUpdate에서 호출)
    void Exit();     // 상태에서 벗어날 때 (정리)
    bool CanExit(IMonsterState nextState); // 현재 상태에서 nextState로 변경 가능한지 여부
}
