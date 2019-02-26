using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace con2.main_menu
{

    public class ButtonFunctions : MonoBehaviour
    {
        public void LoadLobby(string sceneName)
        {
            SceneManager.LoadSceneAsync(SceneNames.Lobby);
        }

        public void OnSettingsClick()
        {
            SceneManager.LoadSceneAsync(SceneNames.Settings);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }

}
