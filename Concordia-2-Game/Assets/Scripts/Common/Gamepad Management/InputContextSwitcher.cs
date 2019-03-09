using con2;
using con2.game;
using System;
using System.Collections;
using System.Collections.Generic;
using con2.lobby;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace con2
{

    public class InputContextSwitcher : MonoBehaviour
    {
        Func<int, List<IInputConsumer>> 
            f_menuContext, 
            f_lobbyContext, 
            f_gameContext;

        public void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            f_menuContext = GetMenuContext;
            f_lobbyContext = GetLobbyContext;
            f_gameContext = GetGameContext;

            //Initialize gamepads
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            var gp = managers.GetComponentInChildren<GamepadMgr>();
            gp.InitializeGampads();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case SceneNames.MainMenu:
                    SetToMenuContext();
                    break;
                case SceneNames.Lobby:
                    SetToLobbyContext();
                    break;
                default: break;
            }
        }

        public void SetToGameContext()
        {
            SwitchContext(f_gameContext);
        }

        public void SetToMenuContext()
        {
            SwitchContext(f_menuContext);
        }

        public void SetToLobbyContext()
        {
            SwitchContext(f_lobbyContext);
        }

        private static void SwitchContext(Func<int, List<IInputConsumer>> contextFunction)
        {
            //Ask gpm for number of player, use that number to set contexts (with playercontroller, and only allow menu input for p1)
            for (int i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                GamepadMgr.Pad(i).SwitchGamepadContext(contextFunction(i), i);
            }
        }

        private static List<IInputConsumer> GetMenuContext(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();
            return inputConsumers;
        }

        private static List<IInputConsumer> GetLobbyContext(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            inputConsumers.Add(managers.GetComponentInChildren<LobbyManager>());
            return inputConsumers;
        }

        private static List<IInputConsumer> GetGameContext(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();

            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            var mgm = managers.GetComponentInChildren<MainGameManager>();
            inputConsumers.Add(mgm);
            var pmi = managers.GetComponentInChildren<PauseMenuInstantiator>();
            inputConsumers.Add(pmi);

            var player = Players.GetPlayerByID(playerIndex);
            inputConsumers.Add(player.GetComponent<FightStun>());

            var kitchenParents = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
            var kitchenStations = new List<ACookingMinigame>();
            foreach (var kitchen in kitchenParents)
            {
                var stations = kitchen.GetComponentsInChildren<ACookingMinigame>();
                kitchenStations.AddRange(stations);
            }

            foreach (ACookingMinigame station in kitchenStations)
            {
                inputConsumers.Add(station);
            }

            inputConsumers.Add(player.GetComponent<PlayerInputController>());

            return inputConsumers;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

}
