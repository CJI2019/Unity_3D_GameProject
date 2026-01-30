using System;
using System.Collections;
using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterState
{
    Move,
    Jump, // Jump 준비
    Jumpping, // Jump 중
    Climb,
}

public class MonsterAI : MonoBehaviour
{
    [SerializeField] bool debugThisObjectLog = false;
    [SerializeField] bool autoClimbFlag      = false;
    [SerializeField] float moveSpeed         = 5.0f;
    [SerializeField] float pivotHeight       = 1f;

    MonsterState currentState = MonsterState.Move;

    Transform target;
    NavMeshAgent agent;
    Rigidbody rigidBody;
    Collider goCollider;

    float lastRayCastTime        = 0f;
    float rayCastCooldown        = 1f;
    float startAgentDelay        = 0f;

    bool destinationFlag         = false;
    bool isGravityMode           = false;
    bool rayCastEnvironmentEmpty = false;

    void Awake()
    {
        target      = FindFirstObjectByType<PlayerController>().transform;
        agent       = GetComponent<NavMeshAgent>();
        rigidBody   = GetComponent<Rigidbody>();
        goCollider  = GetComponent<Collider>();

        agent.speed = moveSpeed;
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        agent.enabled = false;
    }

    void InitState()
    {
        rigidBody.isKinematic  = true;

        agent.enabled   = true;
        autoClimbFlag   = false;
        isGravityMode   = false;

        startAgentDelay = Time.time + 1f;
        agent.velocity  = Vector3.zero;
        agent.speed     = moveSpeed;
        currentState    = MonsterState.Move;
    }

    public void InitAgent(Vector3 spawnPos)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        transform.position = spawnPos;

        InitState();
        agent.isStopped = false; // 가끔 이전 상태가 유지되어 멈춰있을 수 있음
        agent.Warp(spawnPos);
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if ((1 << collision.gameObject.layer | 1 << MonsterSpawner.GetRayCastLayer()) != 0)
        {
            if (debugThisObjectLog)
            {
                Debug.Log(collision.gameObject.name + "과 충돌");
            }

            NavMeshHit hit;

            if (NavMesh.SamplePosition(transform.position, out hit, 3.0f, NavMesh.AllAreas))
            {
                InitState();
            }
            else
            {
                var v  = (transform.position - collision.contacts[0].point + Vector3.up).normalized;
                rigidBody.AddForce(v * moveSpeed, ForceMode.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 flattenMoveDir = new Vector3(
                target.position.x - transform.position.x,
                0f,
                target.position.z - transform.position.z);
        Vector3 direction = flattenMoveDir.normalized;

        if (autoClimbFlag)
        {
            // 레이캐스트로 장애물이 있는지 확인
            rayCastEnvironmentEmpty = !Physics.Raycast(transform.position, direction, 5f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore);
        }
        else
        {
            if(currentState != MonsterState.Move) return;
            bool result = true;

            if (!Physics.Raycast(transform.position, (direction + Vector3.down).normalized, 5f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
            {
                agent.enabled             = false;
                rigidBody.isKinematic     = false;
                goCollider.includeLayers ^= MonsterSpawner.GetRayCastLayer();
                currentState              = MonsterState.Jump;
                // 점프의 힘이 가해진 직후 단차로 인해 충돌이 발생하여 낙하하지 않는 현상을 방지하기 위해 딜레이를 준다.
                StartCoroutine(DelaySetCollisionMask(0.5f));
                result = false;
            }

            if (debugThisObjectLog)
            {
                Debug.Log("Raycast Jump Check: " + result);
            }
        }
    }

    IEnumerator DelaySetCollisionMask(float delay)
    {
        yield return new WaitForSeconds(delay);

        goCollider.includeLayers |= MonsterSpawner.GetRayCastLayer();
    }

    void DebugStateLog()
    {
        if(!debugThisObjectLog) return;

        switch(currentState)
        {
            case MonsterState.Move:
                Debug.Log("Monster State: Move");
                break;
            case MonsterState.Jump:
                Debug.Log("Monster State: Jump");
                break;
            case MonsterState.Jumpping:
                Debug.Log("Monster State: Jumpping");
                break;
            case MonsterState.Climb:
                Debug.Log("Monster State: Climb");
                break;
            default:
                break;
        }

    }

    void Update()
    {
        DebugStateLog();
        // 플레이어 방향 벡터
        Vector3 flattenMoveDir = new Vector3(
            target.position.x - transform.position.x,
            0f,
            target.position.z - transform.position.z);
        Vector3 direction = flattenMoveDir.normalized;

        switch (currentState)
        {
            case MonsterState.Move:
                UpdateMoveState(direction);
                break;
            case MonsterState.Jump:
                rigidBody.AddForce((direction + Vector3.up).normalized * moveSpeed*1.5f, ForceMode.Impulse);
                currentState = MonsterState.Jumpping;
                break;
            case MonsterState.Climb:
                UpdateClimbState();
                break;
            default:
                break;
        }
    }

    private void UpdateMoveState(Vector3 direction)
    {
        if (startAgentDelay < Time.time)
        {
            startAgentDelay = Time.time + 1f;
            // 5f 거리부터 등반 준비를 한다. (나중 몬스터 모델이 확정됨에 따라서 조정해야할 수 있음.)
            if (Physics.Raycast(transform.position, direction, 5f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
            {
                agent.enabled      = false;
                autoClimbFlag      = true;

                goCollider.includeLayers ^= MonsterSpawner.GetRayCastLayer();
                currentState              = MonsterState.Climb;
                return;
            }
        }

        // 항상 플레이어 방향으로 이동
        AgentDirectionMove(direction);

        //if (lastRayCastTime > Time.time)
        //{
        //    if (destinationFlag) return;
        //    AgentDirectionMove(direction);
        //    return;
        //}

        // RayCast 프레임 분산
        //lastRayCastTime = Time.time + (rayCastCooldown * Random.value);

        //float distance = Vector3.Distance(target.position, transform.position);

        //// 1. 3m 앞 장애물 확인
        //// 2. 장애물 존재 시 navagent를 끄고 리지드바드

        //// 플레이어 사이에 장애물이 있는지 확인
        //if (!Physics.Raycast(transform.position, direction, distance, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
        //{
        //    AgentDirectionMove(direction);
        //    destinationFlag = false;
        //}
        //else
        //{
        //    agent.SetDestination(target.position);
        //    lastRayCastTime += 2f; // 경로 탐색 호출의 경우 쿨타임 더욱 증가
        //    destinationFlag = true;
        //}
    }

    private void UpdateClimbState()
    {
        if (debugThisObjectLog)
        {
            Debug.Log("isGravityMode : " + isGravityMode);
        }

        if (autoClimbFlag)
        {
            if (isGravityMode) return;
            if (rayCastEnvironmentEmpty)
            {
                isGravityMode = true;
                LinearMoveClosedPoint();
            }
            else
            {
                transform.position += Vector3.up * Time.deltaTime * agent.speed;
            }
        }
    }

    void AgentDirectionMove(Vector3 direction)
    {
        agent.velocity = direction * agent.speed;

        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 30f);
    }

    public void LinearMoveClosedPoint()
    {
        if(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
        {
            StartCoroutine(MoveToTargetLinear(
                hit.position + Vector3.up * pivotHeight,
                moveSpeed,
                () =>
                {
                    agent.speed               = 0f;
                    rigidBody.isKinematic     = false;
                    goCollider.includeLayers |= MonsterSpawner.GetRayCastLayer();
                }
            ));
        }
        else // 감지된 네비메시가 없더라도 리지드바드를 활성화한다.
        {
            agent.speed               = 0f;
            rigidBody.isKinematic     = false;
            goCollider.includeLayers |= MonsterSpawner.GetRayCastLayer();
        }
    }

    public IEnumerator MoveToTargetLinear(Vector3 target, float speed, Action onComplete)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        onComplete?.Invoke();
    }

}
