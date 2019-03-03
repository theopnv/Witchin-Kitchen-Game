using con2.game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class GamepadAction
    {
        public struct ActionID
        {
            ID m_id;
            bool m_isButton;

            public ActionID(ID id, bool isButton)
            {
                m_id = id;
                m_isButton = isButton;
            }

            public ID GetID()
            {
                return m_id;
            }

            public bool isButton()
            {
                return m_isButton;
            }
        }

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
        private GamepadAction.ActionID m_actionID;
        private int m_playerId;

        public float m_axisValue;

        public GamepadAction(int playerID, GamepadAction.ActionID actionID)
        {
            m_actionID = actionID;
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
            if (m_actionID.isButton())
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

        public GamepadAction.ActionID GetActionID()
        {
            return m_actionID;
        }
    }
}

