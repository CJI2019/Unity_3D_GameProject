using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SceneSingleton<GameManager>
{
    [SerializeField] private bool isGamePaused = false;

    public bool IsGamePaused
    {
        get { return isGamePaused; }
    }

    public void GamePaused(bool paused)
    {
        isGamePaused = paused;
    }
}
