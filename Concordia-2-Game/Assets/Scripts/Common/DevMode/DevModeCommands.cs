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

        private AudienceInteractionManager _AudienceInteractionManager;

        void Start()
        {
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            if (_AudienceInteractionManager == null)
            {
                var instance = Instantiate(_AudienceInteractionManagerPrefab);
                _AudienceInteractionManager = instance.GetComponent<AudienceInteractionManager>();
            }

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
            repo.RegisterCommand("ev_mf", EventMeteoritesFalling);
            repo.RegisterCommand("ev_im", EventIngredientMorph);

            repo.RegisterCommand("spell_dm", SpellDiscoMania);
            repo.RegisterCommand("spell_mmp", SpellMegaMagePunch);
            repo.RegisterCommand("spell_fb", SpellFireballForAll);

            repo.RegisterCommand("game_over", GameOver);
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
                        "- 'ev_na': Simulates the Network Ads (na) event",
                        "- 'ev_mf': Simulates the Meteorites Falling (mf) event",
                        "- 'ev_im': Simulates the Ingredient Morph (im) event",
                        "- 'spell_dm': Simulates the Disco Mania (dm) spell on player 1",
                        "- 'spell_mmp': Simulates the Mega Mage Punch (mmp) spell on player 1",
                        "- 'spell_fb': Simulates the Fireball For All (fb) spell on player 1");

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
            var chosenEvent = new messages.Event
            {
                id = (int)id,
            };
            _AudienceInteractionManager?.BroadcastPollResults(chosenEvent);
        }

        private IEnumerator SimulatePoll()
        {
            var eventManager = FindObjectOfType<EventManager>();
            eventManager.StartPoll(Events.EventID.max_id, Events.EventID.max_id, 5);
            yield return new WaitForSeconds(5);
            BroadcastPoll((Events.EventID)UnityEngine.Random.Range(0, (int)Events.EventID.max_id));
        }

        private IEnumerator SimulateEvent(Events.EventID id)
        {
            yield return new WaitForSeconds(2);
            BroadcastPoll(id);
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

        private string EventMeteoritesFalling(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulateEvent", Events.EventID.meteorites);
            return "Will start the Meteorites Falling event in 2 seconds";
        }

        private string EventIngredientMorph(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulateEvent", Events.EventID.ingredient_morph);
            return "Will start the Ingredient Morph event in 2 seconds";
        }

        private string SpellDiscoMania(string[] args)
        {
            StartCoroutine("SimulateSpell", Spells.SpellID.disco_mania);
            return "Will cast the Disco Mania spell on player 1 in 2 seconds";
        }

        private string SpellMegaMagePunch(string[] args)
        {
            StartCoroutine("SimulateSpell", Spells.SpellID.mega_mage_punch);
            return "Will cast the Mega Mage Punch spell on player 1 in 2 seconds";
        }

        private string SpellFireballForAll(string[] args)
        {
            StartCoroutine("SimulateSpell", Spells.SpellID.fireball_for_all);
            return "Will cast the Fireball For All spell on player 1 in 2 seconds";
        }

        private IEnumerator SimulateSpell(Spells.SpellID id)
        {
            yield return new WaitForSeconds(2);

            var spell = new messages.Spell()
            {
                spellId = (int)id,
                targetedPlayer = new messages.Player() { id = 0 }
            };
            _AudienceInteractionManager?.BroadcastSpellRequest(spell);
        }

        private string GameOver(string[] args)
        {
            StartCoroutine(SimulateGameOver());
            return "Will end the game in 2 seconds.";
        }

        private IEnumerator SimulateGameOver()
        {
            yield return new WaitForSeconds(2);
            var mainGameManager = FindObjectOfType<MainGameManager>();
            mainGameManager.GameOver();
        }

        #endregion


    }
}
