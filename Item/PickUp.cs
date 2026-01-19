using UnityEngine;


public abstract class PickUp : MonoBehaviour , IPoolable
{
    const string PLAYER_STRING = "Player";

    public void OnDespawn() {}
    public void OnSpawn() {}   

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_STRING))
        {
            PickUpLogic(other);
        }
    }

    protected abstract void PickUpLogic(Collider player);
}
