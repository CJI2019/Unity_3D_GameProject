using System.Collections;
using System.Data;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour {
    Transform target;
    NavMeshAgent agent;
    float lastRayCastTime = 0f;
    float rayCastCooldown = 1f;
    bool destinationFlag = false;

    void Awake() 
    {
        target = FindFirstObjectByType<PlayerController>().transform;
    }

    void Update()
    {
        // 플레이어 방향 벡터
        Vector3 flattenMoveDir = new Vector3(
            target.position.x - transform.position.x,
            0f,
            target.position.z - transform.position.z);
        Vector3 direction = flattenMoveDir.normalized;
        if (lastRayCastTime > Time.time)
        {
            if(destinationFlag) return;
            AgentDirectionMove(direction);
            return;
        }

        // RayCast 프레임 분산
        lastRayCastTime = Time.time + (rayCastCooldown * Random.value);

        float distance = Vector3.Distance(target.position, transform.position);

        // 플레이어 사이에 장애물이 있는지 확인
        if (!Physics.Raycast(transform.position, direction, distance, MonsterSpawner.GetRayCastLayer(),QueryTriggerInteraction.Ignore))
        {
            AgentDirectionMove(direction);
            destinationFlag = false;
        }
        else 
        {
            agent.SetDestination(target.position);
            lastRayCastTime += 2f; // 경로 탐색 호출의 경우 쿨타임 더욱 증가
            destinationFlag = true;
        }
    }

    void AgentDirectionMove(Vector3 direction)
    {
        agent.velocity = direction * agent.speed;

        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 30f);
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        agent.enabled = false;
    }

    public void InitAgent(Vector3 spawnPos)
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        transform.position = spawnPos;

        agent.enabled = true;
        agent.velocity = Vector3.zero;
        agent.isStopped = false; // 가끔 이전 상태가 유지되어 멈춰있을 수 있음
        agent.Warp(spawnPos);
    }
}
