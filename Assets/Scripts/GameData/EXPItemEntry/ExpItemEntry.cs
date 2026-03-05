using UnityEngine;

[System.Serializable]
public class MeshEntry
{
    public string id;       // 메시를 식별할 고유 ID 
    public Mesh meshAsset;  // 실제 메시 에셋
}

[CreateAssetMenu(fileName = "ExpItemEntry", menuName = "Scriptable Objects/ExpItemEntry")]
public class ExpItemEntry : ScriptableObject
{
    public int exp; // 경험치 량
    public MeshEntry meshEntry;
    public Color color;

    public string GetMeshId()
    {
        return meshEntry.id;
    }

    public Mesh GetMesh()
    {
        return meshEntry.meshAsset;
    }
}