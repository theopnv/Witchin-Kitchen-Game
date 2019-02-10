using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour {

    public const string LOBBY_SCENE_NAME = "Lobby";

    public void LoadMainScene(bool isHost)
    {
        SceneManager.LoadSceneAsync(LOBBY_SCENE_NAME);
    }

	public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
