using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    Dictionary<int,GameObject> monsters = new();
    [SerializeField] Mesh monsterMesh;
    [SerializeField] Material monsterMat;


    void Start()
    {
        
    }

    void Update()
    {
        var monsterList = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        foreach (var monster in monsterList)
        {
            monster.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }
        
        Matrix4x4[] matrices = new Matrix4x4[monsterList.Length];

        for (int i = 0; i < monsterList.Length; i++) {
            Vector3 pos = monsterList[i].transform.position; 
            Quaternion rot = monsterList[i].transform.rotation;

            matrices[i] = Matrix4x4.TRS(pos, rot, Vector3.one);
        }

        Graphics.DrawMeshInstanced(monsterMesh, 0, monsterMat, matrices, monsterList.Length);
    }

    void AddMontser(int id, GameObject monster)
    {
        monsters.Add(id,monster);
    }

    void RemoveMonster(int id)
    {
        monsters.Remove(id);
    }

    public GameObject GetMonster(int id)
    {
        return monsters[id];
    }

}
