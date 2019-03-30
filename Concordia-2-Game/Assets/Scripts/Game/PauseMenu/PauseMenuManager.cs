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

        [SerializeField] private GameObject _PauseWindow;
        [SerializeField] private Button _ExitButton;
        [SerializeField] private Button _RestartGameButton;
        [SerializeField] private GameObject _ConfirmationWindow;
        [SerializeField] private Button _Yes;
        [SerializeField] private Button _No;
        [SerializeField] private TextMeshProUGUI _ConfirmationText;

        void Start()
        {
            _EventSystem = FindObjectOfType<EventSystem>();
            _EventSystem.SetSelectedGameObject(_RestartGameButton.gameObject);
            Time.timeScale = 0;
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
        }

        void OnDisable()
        {
            Time.timeScale = 1;
        }

        public void OnExitClick()
        {
            ActivateConfirmationWindow(
                "Return to menu?",
                () =>
                {
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
                    Debug.Log("RESTART GAME");
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
            _EventSystem.SetSelectedGameObject(_RestartGameButton.gameObject);
        }

    }
}
