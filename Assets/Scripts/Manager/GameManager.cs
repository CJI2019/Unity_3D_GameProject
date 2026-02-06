using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SceneSingleton<GameManager>
{
    [SerializeField] GameObject ui_GameComplete;
    [SerializeField] private bool isGamePaused = false;

    protected override void Awake()
    {
        var btn_Restart = ui_GameComplete.transform.Find("btn_Restart").GetComponent<Button>();
        btn_Restart.onClick.AddListener(() => RestartGame());

        var btn_Exit = ui_GameComplete.transform.Find("btn_Exit").GetComponent<Button>();
        btn_Exit.onClick.AddListener(() => LoadStartScene());

    }

    public bool IsGamePaused
    {
        get { return isGamePaused; }
    }

    public void GamePaused(bool paused)
    {
        isGamePaused = paused;
    }

    public void GameFinished()
    {
        ui_GameComplete?.SetActive(true);

        var playerInput = FindFirstObjectByType<PlayerInputManager>();
        playerInput.SetCursorLockMode(false);
        GamePaused(true);

    }

    void RestartGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

    void LoadStartScene()
    {
        SceneManager.LoadScene("Start");
    }   

    
}
