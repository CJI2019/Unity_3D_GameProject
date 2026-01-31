using UnityEngine;

public class SelectGizmo : MonoBehaviour
{
    [SerializeField] Transform targetObject;
    [SerializeField] public float radius = 5f;
    [SerializeField] public float angle = 90f;

    private void Start()
    {
        if (targetObject == null)
        {
            targetObject = FindFirstObjectByType<PlayerController>().transform;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 중심점
        Vector3 pos = targetObject.position;
        Vector3 forward = targetObject.forward;

        // 부채꼴 시작 방향
        Quaternion leftRot = Quaternion.AngleAxis(-angle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(angle * 0.5f, Vector3.up);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        // 반경 라인
        Gizmos.DrawLine(pos, pos + leftDir * radius);
        Gizmos.DrawLine(pos, pos + rightDir * radius);

        // 원호 그리기
        int segments = 20;
        float step = angle / segments;
        Vector3 prevPoint = pos + leftDir * radius;
        for (int i = 1; i <= segments; i++)
        {
            Quaternion rot = Quaternion.AngleAxis(-angle * 0.5f + step * i, Vector3.up);
            Vector3 nextPoint = pos + (rot * forward) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
