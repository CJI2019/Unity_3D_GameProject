using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation; // AI Navigation 패키지 네임스페이스
using UnityEngine;
using UnityEngine.AI; // NavMeshBuilder용

public class DungeonBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    public System.Action onBakeComplete;

    public void BakeMapAsync()
    {
        StartCoroutine(BakeRoutine());
    }

    IEnumerator BakeRoutine()
    {
        surface.RemoveData();

        var navMeshData = new NavMeshData();
        surface.navMeshData = navMeshData;

        NavMeshBuildSettings settings = surface.GetBuildSettings();
        
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        Bounds bounds = surface.GetComponent<Collider>().bounds; // 맵 전체 영역

        // NavMesh를 빌드할 때 필요한 소스 데이터를 수집하는 함수
        // NavMeshSurface의 설정을 그대로 가져와서 소스를 수집합니다.
        NavMeshBuilder.CollectSources(
            bounds,
            surface.layerMask,      // 포함할 레이어
            surface.useGeometry,    // RenderMeshes(지오메트리) or PhysicsColliders(충돌체)
            surface.defaultArea,    // 기본 Area 타입 (Walkable 등)
            new List<NavMeshBuildMarkup>(), // 마크업 (보통 비워둠)
            sources
        );

        // 비동기 연산 시작
        // bounds를 설정할 때, surface가 감싸는 영역(Bounds)을 정확히 넣어야 합니다.
        var operation = NavMeshBuilder.UpdateNavMeshDataAsync(
            navMeshData,
            settings,
            sources,
            bounds
        );

        // 완료 대기 (로딩처리시 이부분에서 하면 될듯?)
        while (!operation.isDone)
        {
            Debug.Log($"베이킹 진행률: {operation.progress * 100:F1}%");
            yield return null;
        }

        // 완료 후 적용
        surface.AddData();
        Debug.Log("NavMesh Async Baking Complete!");

        onBakeComplete?.Invoke();
    }
}