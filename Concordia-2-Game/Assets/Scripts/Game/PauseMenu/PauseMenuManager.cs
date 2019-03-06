using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace con2.game
{
    public class PauseMenuManager : MonoBehaviour
    {
        private AudienceInteractionManager _AudienceInteractionManager;

        // TODO: Block timer and game flow
        // TODO: Allow gamepad input in pause menu

        void Start()
        {
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
        }

        public void OnExitClick()
        {
            _AudienceInteractionManager.ExitRoom(false);

            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
        }
    }
}
