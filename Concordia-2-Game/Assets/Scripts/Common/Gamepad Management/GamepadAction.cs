using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class GamepadAction
    {
        public enum ButtonID
        {
            PUNCH,
            INTERACT,
            MAX_ID, //Used as a max count and to handle joystick input
        }

        public Gamepad.InputID defaultInputID;
        public Gamepad.InputID currentInputID;

        private List<IInputConsumer> m_InputConsumers;
        private GamepadAction.ButtonID m_actionID;
        private GameObject m_player;

        //this is shitty code...
        public Vector2 m_movementDirection;

        public GamepadAction(int playerID, GamepadAction.ButtonID actionID)
        {
            m_actionID = actionID;
            m_InputConsumers = new List<IInputConsumer>();

            GameObject kitchenParent = GameObject.FindGameObjectWithTag(Tags.KITCHEN);
            CookingMinigame[] kitchenStations = kitchenParent.GetComponentsInChildren<CookingMinigame>();

            foreach (CookingMinigame station in kitchenStations)
            {
                m_InputConsumers.Add(station);
            }

            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            SpawnPlayersController playerSpawner = managers.GetComponentInChildren<SpawnPlayersController>();
            GameObject[] players = playerSpawner.GetPlayers();
            foreach (GameObject player in players)
            {
                PlayerInputController controller = player.GetComponent<PlayerInputController>();
                if (controller.GetPlayerIndex() == playerID)
                {
                    m_player = player;
                    m_InputConsumers.Add(controller);
                }
            }
        }

        // Internal use only
        public void SetNewButtonInput(bool justPressed, bool pressed, bool justReleased)
        {
            if (justPressed)
            {
                ConsumeInput();
            }

            /*
            if (pressed && PressedEvent != null)
            {
                ConsumeInput(PressedEvent);
            
            if (justReleased && JustReleasedEvent != null)
            {
                ConsumeInput(JustReleasedEvent);
            }
            */
        }

        public void SetNewJoystickInput(Vector2 movementDirection)
        {
            m_movementDirection = movementDirection;
            ConsumeInput();
        }

        private void ConsumeInput()
        {
            foreach (IInputConsumer consumer in m_InputConsumers)
            {
                if (consumer.ConsumeInput(this))
                {
                    return;
                }
            }
        }

        public GamepadAction.ButtonID GetActionID()
        {
            return m_actionID;
        }

        public GameObject GetPlayer()
        {
            return m_player;
        }
    }
}

