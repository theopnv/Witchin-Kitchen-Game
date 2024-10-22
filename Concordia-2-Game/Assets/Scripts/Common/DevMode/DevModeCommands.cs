﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using con2.game;
using con2.messages;
using UnityEngine.SceneManagement;
using static System.Int32;
using Event = UnityEngine.Event;
using Random = System.Random;

namespace con2
{
    public class DevModeCommands : MonoBehaviour
    {
        [SerializeField]
        private GameObject _AudienceInteractionManagerPrefab;

        private AudienceInteractionManager _AudienceInteractionManager;

        private int _CurTestIdx = 0;

        void Awake()
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
            repo.RegisterCommand("game", Game);

            // Game
            repo.RegisterCommand("continuous_events", ContinuousEvents);
            repo.RegisterCommand("random_event", RandomEvent);
            repo.RegisterCommand("ev_fr", EventFreezingRain);
            repo.RegisterCommand("ev_na", EventNetworkAds);
            repo.RegisterCommand("ev_mf", EventMeteoritesFalling);
            repo.RegisterCommand("ev_im", EventIngredientMorph);
            repo.RegisterCommand("ev_ks", EventKitchenSpin);
            repo.RegisterCommand("ev_id", EventIngredientDance);
            repo.RegisterCommand("ev_gg", EventGrassGrowth);

            repo.RegisterCommand("spell_dm", SpellDiscoMania);
            repo.RegisterCommand("spell_mmp", SpellMegaMagePunch);
            repo.RegisterCommand("spell_fb", SpellFireballForAll);
            repo.RegisterCommand("spell_rs", SpellRocketSpeed);
            repo.RegisterCommand("spell_gi", SpellGiftItem);
            repo.RegisterCommand("spell_gb", SpellGiftBomb);
            repo.RegisterCommand("spell_iv", SpellInvisibility);

            repo.RegisterCommand("game_over", GameOver);
            repo.RegisterCommand("test", Test);
            repo.RegisterCommand("test_chain", TestChain);
        }

        public string Help(string[] args)
        {
            var commonMessage = string.Join(
                Environment.NewLine,
                "Such hacking skills, much technology, wow (ง ͠° ͟ل͜ ͡°)ง",
                "",
                "List of available commands for all scenes:",
                "- 'reload': Reloads the current scene",
            "- 'game': Starts Game scene with a specified number of players ('game X')");
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
                        "- 'ev_ks': Simulates the Kitchen Spin (ks) event",
                        "- 'ev_id': Simulates the Ingredient Dance (id) event",
                        "- 'ev_gg': Simulates the Grass Growth (gg) event",
                        "- 'spell_dm': Simulates the Disco Mania (dm) spell on player 1",
                        "- 'spell_mmp': Simulates the Mega Mage Punch (mmp) spell on player 1",
                        "- 'spell_fb': Simulates the Fireball For All (fb) spell on player 1",
                        "- 'spell_rs': Simulates the Rocket Speed (rs) spell on player 1",
                        "- 'spell_gi': Simulates the Gift Item (gi) spell on player 1",
                        "- 'spell_gb': Simulates the Gift Bomb (gb) spell on player 1",
                        "- 'spell_iv': Simulates the Invisibility (iv) spell on player 1",
                        "- 'game_over': Ends the game",
                        "- 'test': Launches the next test",
                        "- 'test_chain': Launches a series of tests");

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
            var eventManager = FindObjectOfType<EventManager>();
            eventManager?.BroadcastPollResults(chosenEvent);
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

        private string EventKitchenSpin(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulateEvent", Events.EventID.kitchen_spin);
            return "Will start the Kitchen Spin event in 2 seconds";
        }

        private string EventIngredientDance(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulateEvent", Events.EventID.ingredient_dance);
            return "Will start the Ingredient Dance event in 2 seconds";
        }

        private string EventGrassGrowth(string[] args)
        {
            if (GetCurrentSceneName() != SceneNames.Game)
            {
                return "You must be in the " + SceneNames.Game + " scene to start this command";
            }

            StartCoroutine("SimulateEvent", Events.EventID.grass_growth);
            return "Will start the Grass Growth event in 2 seconds";
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

        private string SpellRocketSpeed(string[] args)
        {
            StartCoroutine("SimulateSpell", Spells.SpellID.rocket_speed);
            return "Will cast the Rocket Speed spell on player 1 in 2 seconds";
        }

        private string SpellGiftItem(string[] args)
        {
            StartCoroutine("SimulateSpell", Spells.SpellID.gift_item);
            return "Will cast the Gift Item spell on player 1 in 2 seconds";
        }

        private string SpellGiftBomb(string[] args)
        {
            StartCoroutine("SimulateSpell", Spells.SpellID.gift_bomb);
            return "Will cast the Gift Bomb spell on player 1 in 2 seconds";
        }

        private string SpellInvisibility(string[] args)
        {
            StartCoroutine("SimulateSpell", Spells.SpellID.invisibility);
            return "Will cast the Invisibility spell on player 1 in 2 seconds";
        }

        private IEnumerator SimulateSpell(Spells.SpellID id)
        {
            yield return new WaitForSeconds(2);

            var spell = new messages.Spell()
            {
                spellId = (int)id,
                targetedPlayer = new messages.Player() { id = 0 },
                caster = new Viewer { name = "God", socketId = ""}
            };
            var spellsManager = FindObjectOfType<SpellsManager>();
            spellsManager?.OnSpellCasted(spell);
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

        public string Game(string[] args)
        {
            if (args.Length < 1)
            {
                return "First parameter is the number of players (up to 4)";
            }
            GameInfo.PlayerNumber = Parse(args[0]);
            LoadGameScene();
            return "Started Game scene with " + GameInfo.PlayerNumber + " players";
        }

        private void LoadGameScene()
        {
            SceneManager.LoadSceneAsync(SceneNames.Game);
        }

        private string Test(string[] args)
        {
            List<Func<string>> list = new List<Func<string>>();
            list.Add(() => EventFreezingRain(args));
            list.Add(() => EventNetworkAds(args));
            list.Add(() => EventMeteoritesFalling(args));
            list.Add(() => EventIngredientMorph(args));
            list.Add(() => EventKitchenSpin(args));
            list.Add(() => EventIngredientDance(args));
            list.Add(() => EventGrassGrowth(args));
            list.Add(() => SpellDiscoMania(args));
            list.Add(() => SpellMegaMagePunch(args));
            list.Add(() => SpellFireballForAll(args));
            list.Add(() => SpellRocketSpeed(args));
            list.Add(() => SpellGiftItem(args));
            list.Add(() => SpellGiftBomb(args));
            list.Add(() => SpellInvisibility(args));

            var mainGameManager = FindObjectOfType<MainGameManager>();
            mainGameManager.GAME_TIMER = 10000;

            var action = list[_CurTestIdx];
            var retStr = action.Invoke();

            _CurTestIdx++;
            if (_CurTestIdx >= list.Count)
                _CurTestIdx = 0;

            return "Launching test " + _CurTestIdx + "/" + list.Count + ": " + retStr;
        }

        private string TestChain(string[] args)
        {
            Test(args);
            StartCoroutine(CallNextTest(args));
            return "Launching tests";
        }

        private IEnumerator CallNextTest(string[] args)
        {
            if (_CurTestIdx == 0)
            {
                yield return null;
            }
            yield return new WaitForSeconds(10f);
            TestChain(args);
        }

    }
}
