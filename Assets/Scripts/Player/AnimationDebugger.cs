using UnityEngine;

public class AnimationDebugger : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 기본 레이어

        // 상태 이름 직접 비교
        if (stateInfo.IsName("Run"))
        {
            Debug.Log("현재 상태: Run");
        }
        else if (stateInfo.IsName("Jumping"))
        {
            Debug.Log("현재 상태: Jumping");
        }
        else if (stateInfo.IsName("Falling"))
        {
            Debug.Log("현재 상태: Falling");
        }
        else if (stateInfo.IsName("Landing"))
        {
            Debug.Log("현재 상태: Landing");
        }
    }

}
