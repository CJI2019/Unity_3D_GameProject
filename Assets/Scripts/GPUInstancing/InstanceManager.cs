using Unity.VisualScripting;
using UnityEngine;

public class InstanceManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    [SerializeField] int maxPosX = 100;
    [SerializeField] int maxPosZ = 100;
    [SerializeField] float intencity = 2f;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    Matrix4x4[] matrices;

    void Start()
    {
        mesh = prefab.GetComponent<MeshFilter>().mesh;
        material = prefab.GetComponent<MeshRenderer>().material;
        matrices = new Matrix4x4[maxPosX * maxPosZ];
    }

    void Update()
    {
        for(int x = 0; x < maxPosX ; ++x)
        {
            for(int z = 0; z < maxPosZ ; ++z)
            {
                Vector3 pos = new Vector3(x * intencity,0, z * intencity);
                matrices[x * maxPosZ + z] = Matrix4x4.TRS(pos,Quaternion.identity,Vector3.one);
            }
        }

        Graphics.DrawMeshInstanced(mesh,0,material,matrices);
        SimpleBakeTest(skinnedMeshRenderer);
    }

    // 현재 프레임의 정점 위치를 구워서 로그 찍어보기
    public void SimpleBakeTest(SkinnedMeshRenderer target)
    {
        Mesh bakedMesh = new Mesh();
        target.BakeMesh(bakedMesh); 

        Vector3[] vertices = bakedMesh.vertices;
        
        Debug.Log($"1번 정점 위치: {vertices[0]}");
    }
}
