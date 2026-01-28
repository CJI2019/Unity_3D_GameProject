public interface IPoolable
{
    void OnSpawn();   // 풀에서 꺼낼 때 호출
    void OnDespawn(); // 풀에 반환할 때 호출
}
