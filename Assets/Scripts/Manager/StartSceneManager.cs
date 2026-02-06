using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : SceneSingleton<StartSceneManager>
{
    [SerializeField] Button btn_StartGame;
    [SerializeField] Button btn_ExitGame;

    private void Start()
    {
        btn_StartGame.onClick.AddListener(() => OnStartGame());
        btn_ExitGame.onClick.AddListener(() => OnExitGame());
    }

    void OnStartGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

    void OnExitGame()
    {
        Application.Quit();
    }
}
