using UnityEngine;
using UnityEditor; 

#if UNITY_EDITOR
public class TextureBaker : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer targetRenderer;
    [SerializeField] AnimationClip clip;
    [SerializeField] int fps = 30;
    

    [ContextMenu("텍스처로 애니메이션 굽기")]
    public void Bake()
    {
        if (targetRenderer == null || clip == null) return;

        Mesh mesh = targetRenderer.sharedMesh;
        int vertexCount = mesh.vertexCount;
        int frameCount = Mathf.RoundToInt(clip.length * fps);

        // 가로: 정점 인덱스, 세로: 프레임 인덱스
        Texture2D vATex = new Texture2D(vertexCount, frameCount, TextureFormat.RGBAFloat, false);
        vATex.filterMode = FilterMode.Point; // 데이터 오염 방지
        vATex.wrapMode = TextureWrapMode.Clamp;

        Mesh tempMesh = new Mesh();
        float timeStep = 1f / frameCount;
        Animator animator = GetAnimator();
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        for (int frame = 0; frame < frameCount; frame++)
        {
            float currentTime = frame * timeStep;
            // clip.name 과 애니메이션 컨트롤러의 state 이름이 같아야 함에 주의.
            animator.Play(clip.name, 0, currentTime / clip.length);
            animator.Update(0f);

            targetRenderer.BakeMesh(tempMesh);
            Vector3[] vertices = tempMesh.vertices;

            for (int v = 0; v < vertexCount; v++)
            {
                Color posColor = new Color(vertices[v].x, vertices[v].y, vertices[v].z, 1f);
                vATex.SetPixel(v, frame, posColor);
            }
            if (frame == 0) Debug.Log("0프레임 첫 정점: " + tempMesh.vertices[0]);
            if (frame == 10) Debug.Log("10프레임 첫 정점: " + tempMesh.vertices[0]);
        }

        vATex.Apply();

        SaveTexture(vATex);

        Debug.Log($"베이킹 완료 : {vertexCount} 개의 정점 {frameCount} 개의 프레임.");
    }

    Animator GetAnimator()
    {
        Animator animator = targetRenderer.GetComponent<Animator>();
        if (animator == null)
            animator = targetRenderer.GetComponentInParent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator가 없어 임시로 추가합니다.");
            animator = targetRenderer.gameObject.AddComponent<Animator>();
        }

        return animator;
    }

    void SaveTexture(Texture2D tex)
    {
        string path = "Assets/MonsterAnim_VAT.asset";

        AssetDatabase.CreateAsset(tex, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"파일 저장 완료: {path}");
    }
}

#endif