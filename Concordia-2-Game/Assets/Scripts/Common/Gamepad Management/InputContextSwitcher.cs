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
                default: break;
            }
        }

        public void SetToGameContext(int i)
        {
            SwitchContext(f_gameContext, i);
        }

        public void SetToMenuContext()
        {
            SwitchContext(f_menuContext);
        }

        private static void SwitchContext(Func<int, List<IInputConsumer>> contextFunction, int i = -1)
        {
            if (i == -1)
            {
                for (i = 0; i < PlayersInfo.PlayerNumber; i++)
                {
                    GamepadMgr.Pad(i).SwitchGamepadContext(contextFunction(i), i);
                }
            }
            else
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
