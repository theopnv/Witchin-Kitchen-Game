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
            StartCoroutine(Transition.Get().SequenceIn(null, _loadTutorial()));
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
        }

        protected IEnumerator _loadTutorial()
        {
            MenuInfo.DoTutorial = true;
            SceneManager.LoadSceneAsync(SceneNames.Lobby);
            yield return null;
        }

        public void LoadGame(string sceneName)
        {
            StartCoroutine(Transition.Get().SequenceIn(null, _loadGame()));
        }

        protected IEnumerator _loadGame()
        {
            MenuInfo.DoTutorial = false;
            SceneManager.LoadSceneAsync(SceneNames.Lobby);
            yield return null;
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
                buttonSel.audioSource = button.gameObject.AddComponent<AudioSource>();
                buttonSel.audioSource.playOnAwake = false;
                buttonSel.audioSource.clip = switchNoise;
            }
        }
    }

}
