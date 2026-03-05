using UnityEngine;


public abstract class PickUp : MonoBehaviour , IPoolable
{
    const string PLAYER_STRING = "Player";

    public void OnDespawn() {}
    public void OnSpawn() {}   

    public abstract void PickUpLogic(Collider player);

    
}
