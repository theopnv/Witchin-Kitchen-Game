using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.main_menu
{

    public class ButtonFunctions : MonoBehaviour
    {
        public void LoadTutorial()
        {
            MenuInfo.DoTutorial = true;
            SceneManager.LoadSceneAsync(SceneNames.Lobby);
        }

        public void LoadGame(string sceneName)
        {
            MenuInfo.DoTutorial = false;
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


        public AudioClip switchNoise;

        public void Start()
        {
            Button[] b = FindObjectsOfType<Button>();
            AudioSource audio = GetComponent<AudioSource>();
            foreach (var button in b)
            {
                button.onClick.AddListener(delegate () {
                    audio.Play();
                });

                var buttonSel = button.gameObject.AddComponent<OnSelectButton>();
                buttonSel.audio = button.gameObject.AddComponent<AudioSource>();
                buttonSel.audio.playOnAwake = false;
                buttonSel.audio.clip = switchNoise;
            }
        }
    }

}
