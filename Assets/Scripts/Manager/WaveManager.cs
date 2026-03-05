using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    [SerializeField] MonsterSpawner spawner;
    [SerializeField] DungeonBaker dungeonBaker;
    [SerializeField] List<WaveData> waves; // 웨이브 데이터

    Transform player;
    int currentWaveIndex = 0;
    int initCount        = 0;
    bool waveBossDead = true;

    void Awake()
    {
        spawner.OnInit += Initalize; // 풀 확보 대기
        dungeonBaker.onBakeComplete += Initalize; // 던전 생성 대기
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Initalize()
    {
        ++initCount;
        if(initCount == 2)
        {
            StartCoroutine(ProcessWave());
        }
    }

    void WaveBossDeadFlag(Monster monster)
    {
        waveBossDead = true;
    }

    IEnumerator ProcessWave()
    {
        while (currentWaveIndex < waves.Count)
        {
            WaveData currentWave = waves[currentWaveIndex];

            // 현재 웨이브의 모든 스폰 항목 실행
            foreach (var entry in currentWave.spawnEntries)
            {
                if (entry.isWaveBoss) waveBossDead = false;
                StartCoroutine(SpawnRoutine(currentWave,entry));
            }

            // 웨이브 지속 시간만큼 대기
            yield return new WaitForSeconds(currentWave.duration);

            while (GameManager.Instance.IsGamePaused || !waveBossDead)
            {
                yield return null;
            }

            currentWaveIndex++;
        }

        var playerLiving = player.GetComponent<LivingEntity>();
        
        if (playerLiving != null && !playerLiving.IsDead)
        {
            GameManager.Instance.GameFinished();
        }

        Debug.Log("모든 웨이브가 끝났습니다.");
    }

    IEnumerator SpawnRoutine(WaveData waveData,SpawnEntry entry)
    {
        // 스폰 시간 지연
        if (entry.spawnStartTime != 0)
        {
            yield return new WaitForSeconds(entry.spawnStartTime);
        }

        Vector3 playerPos           = GetPlayerUnderPos(player);
        float lastPlayerPosSyncTime = Time.time;

        // 군집 패턴일 경우, 이번 그룹이 사용할 공통 기준점(Anchor) 생성
        Vector3 clumpAnchor = Vector3.zero;
        if (entry.pattern == SpawnPatternType.Clumped)
        {
            clumpAnchor = MonsterSpawner.GetScatteredPosition(playerPos, 20f, 30f);
        }

        float angleStep = 360f / entry.count;
        int retryCount  = 0;

        for (int i = 0; i < entry.count; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            // 플레이어 좌표 갱신
            if (lastPlayerPosSyncTime < Time.time - 3f)
            {
                playerPos = GetPlayerUnderPos(player);
                lastPlayerPosSyncTime = Time.time;
            }

            // 패턴에 따른 위치 결정
            spawnPos = PatternSpawnPos(entry, playerPos, clumpAnchor, angleStep, i, spawnPos);

            // NavMesh 위에 유효한 위치인지 확인
            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
                SpawnMonsterSetting(waveData, entry, spawnPos);
            }
            else
            {
                if (retryCount == 10)
                {
                    retryCount = 0;
                }
                else
                {
                    --i;
                    ++retryCount;
                    continue;
                }
            }

            if (entry.spawnInterval > 0) yield return new WaitForSeconds(entry.spawnInterval);
            else yield return null; //프레임 분산

            while (GameManager.Instance.IsGamePaused)
            {
                yield return null;
            }
        }
    }

    void SpawnMonsterSetting(WaveData waveData, SpawnEntry entry, Vector3 spawnPos)
    {
        Monster spawnMonster = spawner.SpawnMonster(entry.monsterPoolKey, spawnPos);
        if (entry.isWaveBoss)
        {
            spawnMonster.OnMonsterDead += WaveBossDeadFlag;
        }

        spawnMonster.MultiplyDamage(entry.damage_Weight * waveData.ability_Weight);
        spawnMonster.MultiplyMaxHp(entry.hp_Weight * waveData.ability_Weight);
        spawnMonster.SetExpItemEntry(waveData.GetSelectedExpItemEntry());
    }

    Vector3 PatternSpawnPos(SpawnEntry entry, Vector3 playerPos, Vector3 clumpAnchor, float angleStep, int angleStepCount, Vector3 spawnPos)
    {
        switch (entry.pattern)
        {
            case SpawnPatternType.Scattered:
                spawnPos = MonsterSpawner.GetScatteredPosition(playerPos, 25f, 35f);
                break;
            case SpawnPatternType.Clumped:
                spawnPos = MonsterSpawner.GetClumpedPosition(clumpAnchor);
                break;
            case SpawnPatternType.Circle:
                spawnPos = MonsterSpawner.GetCirclePos(playerPos, entry.circleRadius, angleStep,angleStepCount);
                break;
        }

        return spawnPos;
    }

    Vector3 GetPlayerUnderPos(Transform player)
    {
        Vector3 playerPos;
        RaycastHit hit;

        float rayDistance = 20.0f;
        var rayOriginPos  = player.position + Vector3.up;

        if (Physics.Raycast(rayOriginPos, Vector3.down, out hit, rayDistance, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
        {
            playerPos = hit.point;
        }
        else
        {
            playerPos = player.position;
        }

        return playerPos;
    }
}