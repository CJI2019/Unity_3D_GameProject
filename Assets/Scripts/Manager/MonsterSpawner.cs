using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MonsterObject
{
    public Monster prefab;
    public string poolKey;
}

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] List<MonsterObject> monsterList;
    [SerializeField] int monsterCountLimit = 500;
    [Tooltip("몬스터 제한수가 스폰되었을 때 활성화 몬스터와 총 몬스터의 로그를 출력")]
    [SerializeField] bool isDebugActiveLog = false;

    public event Action OnInit;

    static int rayCastingLayerMask = 0;
    int angleStep                  = 1;
    int currentMonsterCount        = 0;
    int activeMosnterListIndex     = 0;
    float radius                   = 25f;
    float spawnCooldown            = 3.0f;
    float lastSpawnTime            = 0;

    Dictionary<int,NavMeshAgent> activeMonsterList = new Dictionary<int, NavMeshAgent>();

    void Awake()
    {
        NavMesh.pathfindingIterationsPerFrame = monsterCountLimit;

        rayCastingLayerMask = 1 << LayerMask.NameToLayer("Default");
    }

    void Start()
    {
        for(int i = 0 ; i < monsterList.Count;++i)
        {
            var monster     = monsterList[i];
            var monsterPool = new GenericObjectPool<Monster>(monster.prefab, 1, transform);
            PoolManager.Instance.RegisterPool(monster.poolKey, monsterPool);
        }

        OnInit.Invoke();
        // float delay = 0.2f;
        // for(int i = 0; i < monsterCountLimit; i += monsterCountLimit / 5)
        // {
        //     StartCoroutine(UpdateAgent(i,i + monsterCountLimit / 5,delay));
        //     delay += 0.2f;
        // }
    }

    void RayCheckSpawn()
    {
        Vector3 center = player.position;
        RaycastHit hit;

        //플레이어를 중심으로 원을 그리며 스폰
        for (int angle = 0; angle < 360; angle += angleStep)
        {
            if (currentMonsterCount >= monsterCountLimit)
            {
                TestActiveMonsterLog();
                return;
            }

            float radian = angle * Mathf.Deg2Rad;
            float x      = center.x + radius * Mathf.Cos(radian);
            float z      = center.z + radius * Mathf.Sin(radian);

            Vector3 pointPos  = new Vector3(x, center.y, z);
            Vector3 direction = Vector3.down;

            float distance   = 20.0f;
            var rayOriginPos = pointPos + Vector3.up * (distance - 5.0f);

            if (Physics.Raycast(rayOriginPos, Vector3.down, out hit, distance, rayCastingLayerMask, QueryTriggerInteraction.Ignore))
            {
                //Monster spawnMonster = PoolManager.Instance.Get<Monster>(poolKey + '0');

                //var ai = spawnMonster.GetComponent<MonsterAI>();
                //ai.InitAgent(hit.point + Vector3.up * 2.0f);
                //FindEmptyKey();
                //// 키를 검색했지만, 남아있는 자리가 없을 경우
                //// 몬스터가 사망해서 풀에 반환한 상태이다. 하지만 UpdateAgent 코루틴 함수에서 아직 이 키가 남아 있는 경우
                //// 키의 반환은 코루틴 함수가 호출되면 자동으로 반환됨
                //if (activeMonsterList.ContainsKey(activeMosnterListIndex))
                //{
                //    PoolManager.Instance.Return<Monster>(poolKey + '0', spawnMonster);
                //    continue;
                //}

                //activeMonsterList[activeMosnterListIndex] = spawnMonster.GetComponent<NavMeshAgent>();
                //spawnMonster.OnMonsterDead += OnMonsterDead;
                //++currentMonsterCount;
                //lastSpawnTime = Time.time + spawnCooldown;

                //Debug.DrawRay(rayOriginPos, direction * hit.distance, Color.green, spawnCooldown);
            }
            else
            {
                Debug.DrawRay(rayOriginPos, direction * distance, Color.red, spawnCooldown);
            }
        }

        Debug.Log($"현재 {currentMonsterCount} 마리 스폰됨.");
    }

    void FindEmptyKey()
    {
        int findCount = 0;
        while (findCount < monsterCountLimit)
        {
            if (!activeMonsterList.ContainsKey(activeMosnterListIndex))
            {
                break;
            }
            activeMosnterListIndex = (activeMosnterListIndex + 1) % monsterCountLimit;
            ++findCount;
        }
    }

    IEnumerator UpdateAgent(int start,int end,float delay)
    {
        yield return new WaitForSeconds(delay);

        // 미세한 딜레이를 줘서 한 프레임에 다른 코루틴 객체와 함께 몰려 처리하는 것을 방지
        WaitForSeconds wfs = new WaitForSeconds(1.0f + delay / 10);

        NavMeshHit hit;

        while (true)
        {
            NavMesh.SamplePosition(player.position, out hit, 30.0f, NavMesh.AllAreas);

            for(int i = start; i < end; ++i)
            {
                if(!activeMonsterList.ContainsKey(i)) continue;

                NavMeshAgent agent = activeMonsterList[i];
                if( !agent.enabled) {
                    activeMonsterList.Remove(i);
                    continue;
                }
                agent.velocity = (hit.position - agent.transform.position).normalized * agent.speed;

                // agent.SetDestination(hit.position);
            }

            // yield return wfs;   
            yield return null;  
        }
    }

    void TestActiveMonsterLog()
    {
        if(!isDebugActiveLog) return;

        int activeCount = 0, allCount = 0;
        foreach (Transform child in transform)
        {
            ++allCount;
            if (child.gameObject.activeSelf) ++activeCount;
        }

        Debug.Log($"몬스터 마리수 제한 도달 : {allCount} 중 {activeCount} 가 활성되어 있음.");
    }

    public void OnMonsterDead(Monster monster)
    {
        --currentMonsterCount;
        monster.OnMonsterDead -= OnMonsterDead;
    }
    
    public static Vector3 GetClumpedPosition(Vector3 anchor)
    {
        // 기준점에서 반경 3f 이내로 모이게 함
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 3f; 
        return anchor + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public static Vector3 GetScatteredPosition(Vector3 playerPos, float minR, float maxR)
    {
        // 각 몬스터마다 랜덤한 각도와 거리를 계산
        float angle    = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = UnityEngine.Random.Range(minR, maxR);

        float x = playerPos.x + Mathf.Cos(angle) * distance;
        float z = playerPos.z + Mathf.Sin(angle) * distance;
        
        Vector3 pointPos = new Vector3(x, playerPos.y, z);

        RaycastHit hit;
        float rayDistance = 20.0f;
        var rayOriginPos  = pointPos + Vector3.up * (rayDistance - 5.0f);
        if (Physics.Raycast(rayOriginPos, Vector3.down, out hit, rayDistance, rayCastingLayerMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return new Vector3(x, 0, z);
    }
    public static Vector3 GetCirclePos(Vector3 center, float radius, int index, float angleStep)
    {
        float angle = index * angleStep * Mathf.Deg2Rad;
        return center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
    }

    public Monster SpawnMonster(string poolKey,Vector3 spawnPos)
    {
        Monster spawnMonster = PoolManager.Instance.Get<Monster>(poolKey);
        spawnMonster.SetPoolKey(poolKey);

        var ai = spawnMonster.GetComponent<MonsterController>();
        ai.InitAgent(spawnPos);
        
        return spawnMonster;
    }

    public static int GetRayCastLayer()
    {
        return rayCastingLayerMask;
    }
}
