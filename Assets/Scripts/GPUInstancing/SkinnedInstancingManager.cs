using UnityEngine;

public class SkinnedInstancingManager : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int instanceCount = 1000;
    public float animationSpeed = 1f; // 애니메이션 속도

    private Matrix4x4[] matrices;
    private float[] frameIndices;
    private MaterialPropertyBlock propertyBlock;

    void Start()
    {
        matrices = new Matrix4x4[instanceCount];
        frameIndices = new float[instanceCount];
        propertyBlock = new MaterialPropertyBlock();

        // 초기 위치 설정
        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
        }

        material.enableInstancing = true;
    }

    void Update()
    {
        float totalFrames = 10f;
        for (int i = 0; i < instanceCount; i++)
        {
            frameIndices[i] = (Time.time * animationSpeed + i) % totalFrames;
        }

        // PropertyBlock에 배열 데이터 채우기 (DX12의 Constant Buffer 업데이트)
        propertyBlock.SetFloatArray("_CurrentFrame", frameIndices);

        // 유니티는 한 번의 호출당 최대 1,023개까지만 지원합니다.
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, instanceCount, propertyBlock);
    }
}