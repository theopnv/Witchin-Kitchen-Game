using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{
    public class PauseMenuManager : MonoBehaviour
    {
        private AudienceInteractionManager _AudienceInteractionManager;
        private EventSystem _EventSystem;
        private AudioSource _ArenaMusic;

        [SerializeField] private GameObject _PauseWindow;
        [SerializeField] private Button _ExitButton;
        [SerializeField] private Button _RestartGameButton;

        [SerializeField] private GameObject _ConfirmationWindow;
        [SerializeField] private Button _Yes;
        [SerializeField] private Button _No;
        [SerializeField] private TextMeshProUGUI _ConfirmationText;

        void Start()
        {
            _ArenaMusic = GameObject.FindGameObjectWithTag(Tags.ARENA_MUSIC).GetComponent<AudioSource>();
            _EventSystem = FindObjectOfType<EventSystem>();
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();

            _ArenaMusic.Pause();
            Time.timeScale = 0;

            if (SceneManager.GetActiveScene().name == SceneNames.Lobby)
            {
                _RestartGameButton.gameObject.SetActive(false);
                _EventSystem.SetSelectedGameObject(_ExitButton.gameObject);
            }
            else
            {
                _EventSystem.SetSelectedGameObject(_RestartGameButton.gameObject);
            }
        }

        void OnDisable()
        {
            Time.timeScale = 1;
            if (_ArenaMusic)
            {
                _ArenaMusic?.UnPause();
            }
        }

        public void OnExitClick()
        {
            ActivateConfirmationWindow(
                "Return to menu?",
                () =>
                {
                    Debug.Log("Returning to menu");
                    _AudienceInteractionManager.SendEndGame(false);
                    SceneManager.LoadSceneAsync(SceneNames.MainMenu);
                },
                BackToPause);
        }

        public void OnRestartGameClick()
        {
            ActivateConfirmationWindow(
                "Restart the game?",
                () =>
                {
                    Debug.Log("Restarting game");
                    _AudienceInteractionManager.SendEndGame(true);
                    SceneManager.LoadSceneAsync(SceneNames.Game);
                },
                BackToPause);
        }

        private void ActivateConfirmationWindow(string text, Action onYes, Action onNo)
        {
            _PauseWindow.SetActive(false);
            _ConfirmationWindow.SetActive(true);

            _EventSystem.SetSelectedGameObject(_No.gameObject);
            _ConfirmationText.text = text;
            _Yes.onClick.AddListener(onYes.Invoke);
            _No.onClick.AddListener(onNo.Invoke);
        }

        private void BackToPause()
        {
            _Yes.onClick.RemoveAllListeners();
            _No.onClick.RemoveAllListeners();
            _PauseWindow.SetActive(true);
            _ConfirmationWindow.SetActive(false);

            if (SceneManager.GetActiveScene().name == SceneNames.Lobby)
            {
                _EventSystem.SetSelectedGameObject(_ExitButton.gameObject);
            }
            else
            {
                _EventSystem.SetSelectedGameObject(_RestartGameButton.gameObject);
            }
        }

    }
}
