using UnityEngine;

public class Monster : MonoBehaviour
{
    long hp = 100;
    MonsterAI ai;
    void Start()
    {
        ai = GetComponent<MonsterAI>();
    }

    public void TakeDamage(long damage)
    {
        hp -= damage;
        Debug.Log(hp);
    }
}
