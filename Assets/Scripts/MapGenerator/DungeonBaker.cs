using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation; 
using UnityEngine;
using UnityEngine.AI; 

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

        var navMeshData               = new NavMeshData();
        surface.navMeshData           = navMeshData;
        NavMeshBuildSettings settings = surface.GetBuildSettings();
        
        List<NavMeshBuildSource> sources = new();
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
        var operation = NavMeshBuilder.UpdateNavMeshDataAsync(
            navMeshData,
            settings,
            sources,
            bounds
        );

        // 완료 대기
        while (!operation.isDone)
        {
            Debug.Log($"베이킹 진행률: {operation.progress * 100:F1}%");
            yield return null;
        }

        // 씬에 있는 모든 NavMeshLink를 찾아서 껐다 킵니다.
        var links = FindObjectsByType<NavMeshLink>(FindObjectsSortMode.None);
        foreach (var link in links)
        {
            if (link.gameObject.activeInHierarchy)
            {
                link.enabled = false;
                link.enabled = true;
            }
        }

        // 완료 후 적용
        surface.AddData();
        Debug.Log("NavMesh Async Baking Complete!");

        onBakeComplete?.Invoke();
    }
}