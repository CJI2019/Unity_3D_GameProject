using UnityEditor;
using UnityEngine;

public class SelectGizmo : MonoBehaviour
{
    [SerializeField] Transform targetObject;

    [SerializeField] bool draw2DArcSector = false;
    [SerializeField] bool draw3DArcSector = false;

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
        if (draw2DArcSector) DrawArcSector2D();
        if (draw3DArcSector) DrawArcSector3D();
    }

    void DrawArcSector2D()
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

    void DrawArcSector3D()
    {
        // 공격의 중심점 (플레이어 위치나 무기 위치)
        Vector3 drawCenter = targetObject.position;

        // 전체 탐색 범위 (원) 그리기 - 흰색
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(drawCenter, radius);

        // 실제 타격 범위 (부채꼴) 그리기 - 반투명 빨강
        Handles.color = new Color(1f, 0f, 0f, 0.2f); // 붉은색, 투명도 20%

        // DrawSolidArc(중심, 회전축, 시작방향, 각도, 반지름)
        // 시야각의 왼쪽 절반
        Handles.DrawSolidArc(drawCenter, Vector3.up, targetObject.forward, angle * 0.5f, radius);
        // 시야각의 오른쪽 절반
        Handles.DrawSolidArc(drawCenter, Vector3.up, targetObject.forward, -angle * 0.5f, radius);
    }
}
