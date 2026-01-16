using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour {
    Transform target;
    NavMeshAgent agent;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        target = FindFirstObjectByType<PlayerController>().transform;
    }

    void Update() {
        agent.SetDestination(target.position);
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
