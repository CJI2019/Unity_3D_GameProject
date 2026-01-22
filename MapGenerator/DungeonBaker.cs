using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonBaker : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    public void BakeMapAsync()
    {
        StartCoroutine(BakeRoutine());
    }

    IEnumerator BakeRoutine()
    {
        // 비동기 빌드 시작
        var op = navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        
        // 완료될 때까지 대기 (게임이 멈추지 않음)
        yield return op;

        Debug.Log("Async NavMesh Baking Complete!");
    }
}