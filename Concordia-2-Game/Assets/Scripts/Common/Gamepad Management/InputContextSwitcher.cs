using System;
using System.Collections.Generic;
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

        public void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            f_menuContext = GetMenuContext;
            f_gameContext = GetGameContext;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case SceneNames.MainMenu:
                    SetToMenuContext();
                    break;
                case SceneNames.Lobby:
                case SceneNames.Game:
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

        private static List<IInputConsumer> GetGameContext(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();

            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            var mgm = managers.GetComponentInChildren<AMainManager>();
            inputConsumers.AddRange(mgm.GetInputConsumers(playerIndex));

            return inputConsumers;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

}
