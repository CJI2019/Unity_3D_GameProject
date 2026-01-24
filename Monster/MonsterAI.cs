using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour {
    Transform target;
    NavMeshAgent agent;
    float destinationInterval = 0.2f;
    float lastSearchTime = 0f;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        target = FindFirstObjectByType<PlayerController>().transform;
    }

    void Update() {
        if(lastSearchTime > Time.time)
        {
            return;
        }

        lastSearchTime = Time.time + destinationInterval;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(target.position, out hit, 30.0f, NavMesh.AllAreas)) 
        {
            agent.SetDestination(hit.position);
        }
    }

    public void MoveStop()
    {
        this.enabled = false;
        agent.ResetPath();
    }

    public void MoveStart()
    {
        this.enabled = true;
    }
}
