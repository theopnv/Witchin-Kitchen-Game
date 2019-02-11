using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace con2.game
{
    public class DevModeCommands : MonoBehaviour
    {
        void Start()
        {
            var repo = ConsoleCommandsRepository.Instance;
            repo.RegisterCommand("help", Help);

            repo.RegisterCommand("reload", Reload);
        }

        public string Help(string[] args)
        {
            var help = string.Join(
                Environment.NewLine,
                "Such hacking skills, much technology, wow (ง ͠° ͟ل͜ ͡°)ง",
                "",
                "List of available commands for the Game scene:",
                "- 'reload': Reloads the scene");
            return help;
        }

        public string Reload(string[] args)
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadSceneAsync(currentSceneName);
            return "Reloaded the scene";
        }
    }
}
