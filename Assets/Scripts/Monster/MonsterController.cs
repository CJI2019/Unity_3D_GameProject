using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField] public bool debugThisObjectLog = false;
    public bool DebugOn {get => debugThisObjectLog;}
    [SerializeField] float moveSpeed         = 5.0f;
    [SerializeField] float pivotHeight       = 1f;

    public Rigidbody rigidBody {get;set;}

    IMonsterState currentState;
    Transform target;
    NavMeshAgent agent;
    Collider goCollider;

    float startAgentDelay        = 0f;
    bool isGamePaused            = false;

    // 클래스 전용 구조체
    public struct PauseSaveData
    {
        public bool useGravity;
        public RigidbodyConstraints constraints;
        public Vector3 agentVelocity;
        public Vector3 rbVelocity;
        public Vector3 rbAngularVelocity;
    }
    PauseSaveData beforeState;

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

        startAgentDelay = Time.time + 1f;
        agent.velocity  = Vector3.zero;
        agent.speed     = moveSpeed;
        goCollider.includeLayers |= MonsterSpawner.GetRayCastLayer();
        ChangeState(new ChaseState(this));
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
    
    // 초기 상태 설정
    private void Start()
    {
        ChangeState(new ChaseState(this));
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused)
        {
            // 움직임 멈춤
            if (isGamePaused) { return; }
            GamePauseSave();
            return;
        }
        else if (isGamePaused)
        {
            GamePauseFinshLoad();
            isGamePaused = false;
        }

        currentState?.Execute();
    }

    public void ChangeState(IMonsterState newState)
    {
        if(currentState != null)
        {
            if(!currentState.CanExit(newState)) return;
        }

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePaused) return;

        currentState.FixedExecute();
    }

    public bool RaycastHitsObstacle(Vector3 direction)
    {
        return !Physics.Raycast(transform.position, direction, 5f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore);
    }

    public bool RayJumpCheck(Vector3 direction)
    {
        bool result = false;

        // 피봇이 하단에 있을 경우 미세한 오차로 인해 점프가 발동하지 않는 경우가 있어 Vector3.up 올린 위치에서 레이캐스트를 쏜다.
        if (!Physics.Raycast(transform.position + Vector3.up, (direction + Vector3.down).normalized, 5f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
        {
            result = true;
        }

        if (debugThisObjectLog)
        {
            Debug.Log("Raycast Jump Check: " + result);
        }

        return result;
    }

    public void SetJumpState()
    {
        agent.enabled = false;
        rigidBody.isKinematic = false;
        goCollider.includeLayers ^= MonsterSpawner.GetRayCastLayer();

        if (gameObject.activeSelf)
        {
            // 점프의 힘이 가해진 직후 단차로 인해 충돌이 발생하여 점프하지 않는 현상을 방지하기 위해 딜레이를 준다.
            StartCoroutine(DelaySetCollisionMask(0.5f));
        }
    }

    public void SetHitState()
    {
        agent.enabled = false;
        rigidBody.isKinematic = false;
    }

    IEnumerator DelaySetCollisionMask(float delay)
    {
        yield return new WaitForSeconds(delay);

        goCollider.includeLayers |= MonsterSpawner.GetRayCastLayer();
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

    public Vector3 GetTargetDirection()
    {
        Vector3 flattenMoveDir = new Vector3(
            target.position.x - transform.position.x,
            0f,
            target.position.z - transform.position.z);
        return flattenMoveDir.normalized;
    }

    public bool NeadClimb(Vector3 direction)
    {
        if (startAgentDelay < Time.time)
        {
            startAgentDelay = Time.time + 1f;
            // 5f 거리부터 등반 준비를 한다. (나중 몬스터 모델이 확정됨에 따라서 조정해야할 수 있음.)
            if (Physics.Raycast(transform.position, direction, 5f, MonsterSpawner.GetRayCastLayer(), QueryTriggerInteraction.Ignore))
            {
                InitClimbState();
                return true;
            }
        }

        return false;
    }

    public void InitClimbState()
    {
        agent.enabled = false;

        goCollider.includeLayers ^= MonsterSpawner.GetRayCastLayer();
    }

    public void MoveToUp()
    {
        transform.position += Vector3.up * Time.deltaTime * agent.speed;
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

    // direction 방향벡터로 이동 & direction으로 lookvector 회전
    public void AgentDirectionMove(Vector3 direction)
    {
        agent.velocity = direction * agent.speed;

        if(direction == Vector3.zero) return;
        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 30f);
    }

    public void AddForceToDirUp(Vector3 direction)
    {
        rigidBody.AddForce((direction + Vector3.up).normalized * moveSpeed*1.5f, ForceMode.Impulse);
    }

    private void GamePauseFinshLoad()
    {
        // 직전 상태 로드
        rigidBody.useGravity = beforeState.useGravity;
        rigidBody.constraints = beforeState.constraints;
        agent.velocity = beforeState.agentVelocity;

        if (!rigidBody.isKinematic)
        {
            rigidBody.angularVelocity = beforeState.rbAngularVelocity;
            rigidBody.linearVelocity = beforeState.rbVelocity;
        }
    }

    private void GamePauseSave()
    {
        // 직전 상태 저장
        beforeState.useGravity = rigidBody.useGravity;
        beforeState.constraints = rigidBody.constraints;
        beforeState.agentVelocity = agent.velocity;
        beforeState.rbVelocity = rigidBody.linearVelocity;
        beforeState.rbAngularVelocity = rigidBody.angularVelocity;

        // 물리력을 0으로 만들어 즉시 정지
        isGamePaused = true;
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        agent.velocity = Vector3.zero;

        if (!rigidBody.isKinematic)
        {
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

}