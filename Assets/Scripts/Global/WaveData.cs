using System.Collections.Generic;
using UnityEngine;

// 스폰 패턴 종류
public enum SpawnPatternType
{
    Scattered, // 개별적으로 위치 선정
    Clumped,   // 한 지점에 뭉쳐서
    Circle,    // 플레이어를 둘러쌈
}

[CreateAssetMenu(fileName = "NewWave", menuName = "Game/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Settings")]
    public float duration = 60f; // 웨이브 지속 시간

    [Header("Spawn Info")]
    public List<SpawnEntry> spawnEntries; // 이 웨이브에 나올 몬스터 목록
}

[System.Serializable]
public class SpawnEntry
{
    public string monsterPoolKey;  // 풀링 키
    public int count;              // 스폰 할 마릿수
    public float spawnStartTime; // 첫 스폰 전 대기할 시간
    public SpawnPatternType pattern; // 스폰 패턴
    public float spawnInterval;    // 스폰 간격 (0이면 한 번에)
    
    [Header("Circle Pattern Settings")]
    public float circleRadius = 15.0f; // 플레이어로부터의 거리 (반지름)
}