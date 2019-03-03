using con2.game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class GamepadAction
    {
        public enum ID
        {
            __BUTTONS__,
            PUNCH,
            INTERACT,
            START,
            __AXES__,
            HORIZONTAL,
            VERTICAL,
            LEFT_TRIGGER,
            RIGHT_TRIGGER,
            MAX_ID
        }

        public Gamepad.InputID defaultInputID;
        public Gamepad.InputID currentInputID;

        private List<IInputConsumer> m_InputConsumers = new List<IInputConsumer>();
        private GamepadAction.ID m_actionID;
        bool m_isButton;
        private int m_playerId;

        public float m_axisValue;

        public GamepadAction(int playerID, GamepadAction.ID actionID, bool isButton)
        {
            m_actionID = actionID;
            m_isButton = isButton;
        }

        public void SetInputConsumers(List<IInputConsumer> inputConsumers, int playerId)
        {
            m_InputConsumers = inputConsumers;
            m_playerId = playerId;
        }

        protected void ConsumeInput()
        {
            foreach (IInputConsumer consumer in m_InputConsumers)
            {
                if (consumer.ConsumeInput(this))
                {
                    return;
                }
            }
        }

        public void SetNewInput(bool justPressed, bool pressed, bool justReleased, float movementDirection)
        {
            if (m_isButton)
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
            else
            {
                if (Math.Abs(movementDirection) > 0.001)
                {
                    m_axisValue = movementDirection;
                    ConsumeInput();
                }
            }
        }

        public int GetPlayerId()
        {
            return m_playerId;
        }

        public GamepadAction.ID GetActionID()
        {
            return m_actionID;
        }
    }
}

