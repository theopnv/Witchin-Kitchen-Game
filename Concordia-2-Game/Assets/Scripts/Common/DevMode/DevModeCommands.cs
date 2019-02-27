using System;
using UnityEngine;
using System.Collections;
using con2.game;
using con2.messages;
using UnityEngine.SceneManagement;
using Event = UnityEngine.Event;
using Random = System.Random;

namespace con2
{
    public class DevModeCommands : MonoBehaviour
    {
        [SerializeField]
        private GameObject _AudienceInteractionManagerPrefab;

        void Start()
        {
            var repo = ConsoleCommandsRepository.Instance;
            repo.RegisterCommand("help", Help);

            // All scenes
            repo.RegisterCommand("reload", Reload);
            repo.RegisterCommand("game1", Game1);
            repo.RegisterCommand("game2", Game2);

            // Game
            repo.RegisterCommand("continuous_events", ContinuousEvents);
            repo.RegisterCommand("random_event", RandomEvent);
            repo.RegisterCommand("ev_fr", EventFreezingRain);
            repo.RegisterCommand("ev_na", EventNetworkAds);
        }

        public string Help(string[] args)
        {
            var commonMessage = string.Join(
                Environment.NewLine,
                "Such hacking skills, much technology, wow (ง ͠° ͟ل͜ ͡°)ง",
                "",
                "List of available commands for all scenes:",
                "- 'reload': Reloads the current scene",
                "- 'game1': Starts Game scene with 1 player (red) and with no communication with the server",
                "- 'game2': Starts Game scene with 2 players (red and blue) and with no communication with the server");
            var help = commonMessage;
            var currentSceneName = GetCurrentSceneName();

            switch (currentSceneName)
            {
                case SceneNames.MainMenu:
                    help = string.Join(
                        Environment.NewLine,
                        commonMessage);
                    break;
                case SceneNames.Lobby:
                    help = string.Join(
                        Environment.NewLine,
                        commonMessage);
                    break;
                case SceneNames.Game:
                    var gameHelp = string.Join(
                        Environment.NewLine,
                        "- 'continuous_events': Randomly simulates events every 60 seconds",
                        "- 'random_event': Simulates a poll and starts an event in 5 seconds",
                        "- 'ev_fr': Simulates the Freezing Rain (fr) event",
                        "- 'ev_dm': Simulates the Disco Mania (dm) event");

                    help = string.Join(
                        Environment.NewLine,
                        commonMessage,
                        gameHelp);
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

        public string Game1(string[] args)
        {
            PlayersInfo.Color[0] = Color.red;
            PlayersInfo.PlayerNumber = 1;

            LoadGameScene();
            return "Started Game scene with 1 player";
        }

        public string Game2(string[] args)
        {
            PlayersInfo.Color[0] = Color.red;
            PlayersInfo.Color[1] = Color.blue;
            PlayersInfo.PlayerNumber = 2;

            LoadGameScene();
            return "Started Game scene with 2 players";
        }

        private void LoadGameScene()
        {
            var audienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            if (audienceInteractionManager == null)
            {
                Instantiate(_AudienceInteractionManagerPrefab);
            }

            SceneManager.LoadSceneAsync(SceneNames.Game);
        }

        #endregion

        #region Main Menu Commands

        #endregion

        #region Game Commands

        private string ContinuousEvents(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            InvokeRepeating("SimulateContinuousEvents", 0, 60);
            return "Will simulate a poll every 60 seconds. Starting the first one in 5 seconds.";
        }

        private void SimulateContinuousEvents()
        {
            StartCoroutine("SimulatePoll");
        }

        private string RandomEvent(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulatePoll");
            return "Poll was simulated. Random event starting in 5 seconds";
        }

        private void BroadcastPoll(Events.EventID id)
        {
            var audienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            var chosenEvent = new messages.Event
            {
                id = (int)id,
            };
            audienceInteractionManager.BroadcastPollResults(chosenEvent);
        }

        private IEnumerator SimulatePoll()
        {
            var eventManager = FindObjectOfType<EventManager>();
            eventManager.StartPoll(Events.EventID.max_id, Events.EventID.max_id, 5);
            yield return new WaitForSeconds(5);
            BroadcastPoll((Events.EventID)UnityEngine.Random.Range(0, (int)Events.EventID.max_id));
        }

        private string EventFreezingRain(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulateEvent", Events.EventID.freezing_rain);
            return "Will start the Freezing Rain event in 2 seconds";
        }

        private string EventNetworkAds(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulateEvent", Events.EventID.network_ads);
            return "Will start the Network Ads event in 2 seconds";
        }

        private IEnumerator SimulateEvent(Events.EventID id)
        {
            yield return new WaitForSeconds(2);
            BroadcastPoll(id);
        }

        #endregion


    }
}
