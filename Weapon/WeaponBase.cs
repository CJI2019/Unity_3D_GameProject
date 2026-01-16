using UnityEditor.Search;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] protected Transform target;
    [SerializeField] protected long damage = 10;
    [SerializeField] float attackRate = 2f; // 공격 주기
    protected float timer;


    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer >= attackRate)
        {
            timer = 0f;
            Attack();            
        }
    }

    protected abstract void Attack();
}