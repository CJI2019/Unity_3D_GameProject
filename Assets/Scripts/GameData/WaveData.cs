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
    [Header("웨이브 설정")]
    public float duration = 60f; // 웨이브 지속 시간
    public float ability_Weight = 1.0f; // 능력치 가중치 : 체력, 데미지 등 포함.

    [Header("웨이브 몬스터 설정")]
    public List<SpawnEntry> spawnEntries; // 이 웨이브에 나올 몬스터 목록
    public ExpItemDataBase expItemDB;

    // 선택된 엔트리 : 인스펙터에는 표시 안함.(커스텀 에디터로 표시될 것임.WaveDataCustomEditor)
    // 경험치를 선택하면 DB에 경험치에 맞는 메시가 선택될 것임.
    [SerializeField] ExpItemEntry selectedExpItemEntry; 

    public ExpItemEntry GetSelectedExpItemEntry() => selectedExpItemEntry;

    public void SetSelectedExpItemEntry(ExpItemEntry expItemEntry)
    {
        selectedExpItemEntry = expItemEntry;
    }
}

[System.Serializable]
public class SpawnEntry
{
    public string monsterPoolKey;  // 풀링 키
    public int count;              // 스폰 할 마릿수
    public float spawnStartTime; // 첫 스폰 전 대기할 시간
    public SpawnPatternType pattern; // 스폰 패턴
    public float spawnInterval;    // 스폰 간격 (0이면 한 번에)
    public float hp_Weight = 1.0f; // 체력 가중치
    public float damage_Weight = 1.0f; // 데미지 가중치
    
    [Header("보스 설정 : 마지막 웨이브 보스는 게임 최종 보스")]
    public bool isWaveBoss = false; // 웨이브 최종 보스 여부 : 웨이브 당 1마리만 있어야한다.



    [Header("원형 스폰 패턴 설정")]
    public float circleRadius = 15.0f; // 플레이어로부터의 거리 (반지름)
}