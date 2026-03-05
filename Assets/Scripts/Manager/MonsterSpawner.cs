using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterObject
{
    public Monster prefab;
    public string poolKey;
    public Mesh mesh;
    public Material material;
    public float sharedScale;
}

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] List<MonsterObject> monsterList;
    [Tooltip("몬스터 제한수가 스폰되었을 때 활성화 몬스터와 총 몬스터의 로그를 출력")]
    [SerializeField] bool isDebugActiveLog = false;


    public event Action OnInit;

    static int rayCastingLayerMask = 0;

    void Awake()
    {
        rayCastingLayerMask = 1 << LayerMask.NameToLayer("Default");
    }

    void Start()
    {
        for(int i = 0 ; i < monsterList.Count;++i)
        {
            var monster     = monsterList[i];
            var monsterPool = new GenericObjectPool<Monster>(monster.prefab, 1, transform);
            PoolManager.Instance.RegisterPool(monster.poolKey, monsterPool);

            MonsterInstacingManager.Instance.AddMonsterData(monster.poolKey,monster.mesh,monster.material,monster.sharedScale);
        }

        OnInit.Invoke();
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
    public static Vector3 GetCirclePos(Vector3 center, float radius,float angleStep, int stepCount)
    {
        float angle = stepCount * angleStep * Mathf.Deg2Rad;
        return center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
    }

    public Monster SpawnMonster(string poolKey,Vector3 spawnPos)
    {
        Monster spawnMonster = PoolManager.Instance.Get<Monster>(poolKey);
        spawnMonster.SetPoolKey(poolKey);

        var ai = spawnMonster.GetComponent<MonsterController>();
        ai.InitAgent(spawnPos);

        int id = MonsterInstacingManager.Instance.AddMontser(poolKey,spawnMonster.gameObject);
        spawnMonster.SetMonsterId(id);
        
        return spawnMonster;
    }

    public static int GetRayCastLayer()
    {
        return rayCastingLayerMask;
    }
}
