using System.Collections.Generic;
using UnityEngine;

public struct MonsterData
{
    public Mesh mesh;
    public Material material;
    public int animTexture_Height;
    public float sharedScale;
}

public struct InstancingData
{
    public Matrix4x4[] matrix4X4s;
    public float sharedScale;
    public MaterialPropertyBlock propertyBlock;
    public float[] currentFrame;
}

public class MonsterInstacingManager : SceneSingleton<MonsterInstacingManager>
{
    [SerializeField] float frameSpeed = 1f;

    Dictionary<int,KeyValuePair<string, GameObject>> activeMonsters = new();
    Dictionary<string,List<InstancingData>> instancingDataListByPoolkey = new();
    Dictionary<string,MonsterData> keyByMonsterData = new();
    // poolKey별로 현재 몇 번째 인덱스까지 채웠는지 저장하는 딕셔너리
    Dictionary<string, int> poolCounts = new();
    IdPool idPool = new();

    public void AddMonsterData(string poolKey,Mesh mesh, Material material, float sharedScale = 1.0f)
    {
        var monsterData = new MonsterData();
        monsterData.mesh = mesh;
        monsterData.material = material;
        monsterData.sharedScale = sharedScale;

        var anim_Tex = material.GetTexture("_AnimTex");
        monsterData.animTexture_Height = anim_Tex.height;

        material.enableInstancing = true;

        keyByMonsterData.Add(poolKey,monsterData);
    }

    public int AddMontser(string poolKey, GameObject monster)
    {
        if (!instancingDataListByPoolkey.ContainsKey(poolKey))
        {
            ExtendInstanceData(poolKey);
        }

        int id = idPool.GetId();

        activeMonsters.Add(id,new KeyValuePair<string, GameObject>(poolKey,monster));

        return id;
    }

    public void RemoveMonster(int id)
    {
        activeMonsters.Remove(id);
        idPool.ReturnId(id);
    }

    void ExtendInstanceData(string poolKey)
    {
        instancingDataListByPoolkey[poolKey] = new List<InstancingData>();

        InstancingData instancingData = new InstancingData();
        instancingData.matrix4X4s = new Matrix4x4[1023];
        for (int i = 0; i < instancingData.matrix4X4s.Length; ++i)
        {
            instancingData.matrix4X4s[i] = Matrix4x4.identity;
        }
        instancingData.propertyBlock = new MaterialPropertyBlock();
        instancingData.currentFrame = new float[1023];

        MonsterData monsterData =  keyByMonsterData[poolKey];
        instancingData.sharedScale = monsterData.sharedScale;

        instancingDataListByPoolkey[poolKey].Add(instancingData);
    }

    void Update()
    {
        poolCounts.Clear();

        foreach (var kvp in activeMonsters)
        {
            string poolKey = kvp.Value.Key;
            GameObject go = kvp.Value.Value;

            if (!poolCounts.TryGetValue(poolKey, out int currentCount))
            {
                currentCount = 0;
            }

            int listIndex = currentCount / 1023;
            int arrayIndex = currentCount % 1023;

            if (!instancingDataListByPoolkey.ContainsKey(poolKey))
            {
                Debug.LogError(poolKey + " 가 존재하지 않습니다.");
                return;
            }

            if (instancingDataListByPoolkey[poolKey].Count <= listIndex)
            {
                ExtendInstanceData(poolKey);
            }

            instancingDataListByPoolkey[poolKey][listIndex].matrix4X4s[arrayIndex] = go.transform.localToWorldMatrix;

            poolCounts[poolKey] = currentCount + 1;
        }

        RenderInstances();
    }

    void RenderInstances()
    {
        foreach (var poolEntry in poolCounts)
        {
            string poolKey = poolEntry.Key;
            int totalCount = poolEntry.Value;
            
            if (!instancingDataListByPoolkey.TryGetValue(poolKey, out var instancingDataList)) continue;

            Mesh mesh = keyByMonsterData[poolKey].mesh;
            Material material = keyByMonsterData[poolKey].material;
            int frameCount = keyByMonsterData[poolKey].animTexture_Height;

            int remainingCount = totalCount;
            for (int i = 0; i < instancingDataList.Count; ++i)
            {
                if (remainingCount <= 0) break;

                int drawCount = Mathf.Min(remainingCount, 1023);

                InstancingData instancingData = instancingDataList[i];

                if(!GameManager.Instance.IsGamePaused) // 게임 중지시에 애니메이션 프레임을 고정한다.
                {
                    for(int j = 0; j < drawCount; ++j) 
                    {
                        instancingData.currentFrame[j] = (Time.time * frameSpeed + j) % frameCount;
                    }
                }

                instancingData.propertyBlock.SetFloatArray("_CurrentFrame", instancingData.currentFrame);
                instancingData.propertyBlock.SetFloat("_SharedScale",instancingData.sharedScale);

                Graphics.DrawMeshInstanced(mesh, 0, material, instancingData.matrix4X4s, drawCount,instancingData.propertyBlock);
                remainingCount -= drawCount;
            }
        }
    }
}
