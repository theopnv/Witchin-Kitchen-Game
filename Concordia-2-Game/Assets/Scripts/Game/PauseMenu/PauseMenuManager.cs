using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{
    public class PauseMenuManager : MonoBehaviour
    {
        private AudienceInteractionManager _AudienceInteractionManager;
        public Button _ExitButton;

        void Start()
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            eventSystem.SetSelectedGameObject(_ExitButton.gameObject);
            Time.timeScale = 0;
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
        }

        void OnDisable()
        {
            Time.timeScale = 1;
        }

        public void OnExitClick()
        {
            _AudienceInteractionManager.SendEndGame(false);

            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
        }

    }
}
