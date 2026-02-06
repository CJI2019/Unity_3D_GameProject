using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] UIAbility uiAbility;

    void OnEnable()
    {
        uiAbility.Init(player);
    }

    void OnDisable()
    {
        uiAbility.DeInit();
        
    }
}
