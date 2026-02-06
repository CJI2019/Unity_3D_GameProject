using UnityEngine;
using System.Collections.Generic;

public class MeshManager : MonoBehaviour
{
    public static MeshManager Instance { get; private set; }

    [Header("Pre-load Settings")]
    [Tooltip("캐싱할 메시들을 이곳에 등록하세요.")]
    [SerializeField] public ExpItemDataBase meshDataBase; // 상속구조를 고려해서 Mesh들만 따로 가져올 수 있을 듯하다.

    private Dictionary<string, Mesh> meshCache;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            InitializeCache();
            DontDestroyOnLoad(gameObject); // 씬 변경 시 파괴 방지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 리스트 데이터를 딕셔너리로 변환
    private void InitializeCache()
    {
        meshCache = new Dictionary<string, Mesh>();

        foreach (var entry in meshDataBase.entryList)
        {
            if (entry.GetMesh() == null) continue;

            if (!meshCache.ContainsKey(entry.GetMeshId()))
            {
                meshCache.Add(entry.GetMeshId(), entry.GetMesh());
            }
            else
            {
                Debug.LogWarning($"[MeshManager] 중복된 ID가 존재합니다: {entry.GetMeshId()}");
            }
        }
        
        Debug.Log($"[MeshManager] {meshCache.Count}개의 메시가 캐싱되었습니다.");
    }

    /// <summary>
    /// 대상 오브젝트의 메시를 교체합니다.
    /// </summary>
    /// <param name="targetObj">메시를 교체할 대상 오브젝트</param>
    /// <param name="meshId">캐싱된 메시의 ID</param>
    /// <returns>교체 성공 여부</returns>
    public bool SwapMesh(GameObject targetObj, string meshId)
    {
        if (!meshCache.ContainsKey(meshId))
        {
            Debug.LogError($"[MeshManager] ID '{meshId}'에 해당하는 메시를 찾을 수 없습니다.");
            return false;
        }

        MeshFilter filter = targetObj.GetComponent<MeshFilter>();
        if (filter == null)
        {
            Debug.LogError($"[MeshManager] '{targetObj.name}' 오브젝트에 MeshFilter가 없습니다.");
            return false;
        }

        // 메모리 최적화를 위해 mesh 대신 sharedMesh 사용 권장
        filter.sharedMesh = meshCache[meshId];
        return true;
    }

    /// <summary>
    /// 특정 메시만 가져오고 싶을 때 사용
    /// </summary>
    public Mesh GetMesh(string meshId)
    {
        if (meshCache.TryGetValue(meshId, out Mesh mesh))
        {
            return mesh;
        }
        return null;
    }
}