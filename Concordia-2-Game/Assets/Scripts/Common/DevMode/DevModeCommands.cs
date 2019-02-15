using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace con2
{
    public class DevModeCommands : MonoBehaviour
    {
        private const string _MAIN_MENU_SCENE_NAME = "Main Menu";
        private const string _LOBBY_SCENE_NAME = "Lobby";
        private const string _GAME_SCENE = "Game";

        void Start()
        {
            var repo = ConsoleCommandsRepository.Instance;
            repo.RegisterCommand("help", Help);

            // All scenes
            repo.RegisterCommand("reload", Reload);

            // Main Menu
            repo.RegisterCommand("game1", Game1);
            repo.RegisterCommand("game2", Game2);
        }

        public string Help(string[] args)
        {
            var commonMessage = string.Join(
                Environment.NewLine,
                "Such hacking skills, much technology, wow (ง ͠° ͟ل͜ ͡°)ง",
                "",
                "List of available commands for all scenes:",
                "- 'reload': Reloads the current scene",
                "- 'game1': Starts Game scene with 1 player (red)",
                "- 'game2': Starts Game scene with 2 players (red and blue)");
            var help = commonMessage;
            var currentSceneName = GetCurrentSceneName();

            switch (currentSceneName)
            {
                case _MAIN_MENU_SCENE_NAME:
                    help = string.Join(
                        Environment.NewLine,
                        commonMessage);
                    break;
                case _LOBBY_SCENE_NAME:
                    help = string.Join(
                        Environment.NewLine,
                        commonMessage);
                    break;
                case _GAME_SCENE:
                    help = string.Join(
                        Environment.NewLine,
                        commonMessage);
                    break;
            }
            return help;
        }

        private string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        #region All Scenes' methods

        public string Reload(string[] args)
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadSceneAsync(currentSceneName);
            return "Reloaded the scene";
        }

        #endregion 

        #region Main Menu Commands

        public string Game1(string[] args)
        {
            PlayersInfo.Color[0] = Color.red;
            PlayersInfo.PlayerNumber = 1;
            SceneManager.LoadSceneAsync("Game");
            return "Started Game scene with 1 player";
        }

        public string Game2(string[] args)
        {
            PlayersInfo.Color[0] = Color.red;
            PlayersInfo.Color[1] = Color.blue;
            PlayersInfo.PlayerNumber = 2;
            SceneManager.LoadSceneAsync("Game");
            return "Started Game scene with 2 players";
        }

        #endregion
    }
}
