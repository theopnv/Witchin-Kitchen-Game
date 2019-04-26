using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.main_menu
{
    public class ButtonFunctions : MonoBehaviour
    {
        // Credits and attribution
        public static string CREDITS_DOC_URL = "https://docs.google.com/document/d/1PH4Si2lHkjPLwk_vJrEr5LwYgHm-KrcigfE7ZMkKtX4/edit?usp=sharing";

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

        public void LoadSettingsScene()
        {
            SceneManager.LoadSceneAsync(SceneNames.Settings);
        }

        public void LoadControlsScene()
        {
            SceneManager.LoadSceneAsync(SceneNames.Controls);
        }

        public void LoadConceptArtScene()
        {
            SceneManager.LoadSceneAsync(SceneNames.ConceptArt);
        }

        public void LoadCreditsScene()
        {
            SceneManager.LoadSceneAsync(SceneNames.Credits);
        }

        public void ShowCredits()
        {
            Application.OpenURL(CREDITS_DOC_URL);
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
