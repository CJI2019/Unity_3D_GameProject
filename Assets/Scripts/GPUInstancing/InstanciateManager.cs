using UnityEngine;

public class InstanciateManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int maxPosX = 30;
    [SerializeField] int maxPosZ = 30;
    [SerializeField] float intencity = 2f;

    void Start()
    {

        for(int x = 0; x < maxPosX ; ++x)
        {
            for(int z = 0; z < maxPosZ ; ++z)
            {
                Vector3 pos = new Vector3(x * intencity,0, z * intencity);
                Instantiate(prefab,pos,Quaternion.identity);
            }
        }
    }
}

