using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class Gamepad
    {
        public class InputID
        {
            private string ID;

            public InputID(string ID)
            {
                this.ID = ID;
            }

            public string Value {
                get { return ID; }
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                InputID otherID = obj as InputID;

                return otherID.Value == this.Value;
            }

            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }

            public override string ToString()
            {
                return ID;
            }
        }

        public static readonly InputID INP_HORIZONTAL = new InputID("Horizontal");
        public static readonly InputID INP_VERTICAL = new InputID("Vertical");
        public static readonly InputID INP_PUNCH = new InputID("Punch");
        public static readonly InputID INP_INTERACT = new InputID("Interact");
        public static readonly InputID INP_START = new InputID("Start");

        private Dictionary<GamepadAction.ButtonID, GamepadAction> actions;      // One to one
        private Dictionary<InputID, List<GamepadAction.ButtonID>> inputToAction; // One to many
        private GamepadAction joystickAction;
        private Vector2 movementDirection;
        private Vector2 movementDirectionRaw;

        private bool m_movementIsInverted = false;

        private int m_playerID;

        public Gamepad(int playerID)
        {
            m_playerID = playerID;

            actions = new Dictionary<GamepadAction.ButtonID, GamepadAction>();
            for (GamepadAction.ButtonID actionID = 0; actionID < GamepadAction.ButtonID.MAX_ID; ++actionID)
            {
                actions.Add(actionID, new GamepadAction(playerID, actionID));
            }

            joystickAction = new GamepadAction(playerID, GamepadAction.ButtonID.MAX_ID);

            inputToAction = new Dictionary<InputID, List<GamepadAction.ButtonID>>();
            setupDefaultMappings();
            foreach (InputID inputID in inputToAction.Keys)
            {
                // Save default mapping
                var list = inputToAction[inputID];
                foreach (var actionID in list)
                {
                    var action = actions[actionID];
                    action.defaultInputID = inputID;
                }
            }
        }

        public void SwitchGamepadContext(List<IInputConsumer> inputConsumers, int playerId)
        {
            foreach (KeyValuePair<GamepadAction.ButtonID, GamepadAction> action in actions)
            {
                action.Value.SetInputConsumers(inputConsumers, playerId);
            }

            joystickAction.SetInputConsumers(inputConsumers, playerId);
        }

        // DEFINE DEFAULT CONTROLS HERE!
        private void setupDefaultMappings()
        {
            addMapping(INP_PUNCH, GamepadAction.ButtonID.PUNCH);
            addMapping(INP_INTERACT, GamepadAction.ButtonID.INTERACT);
            addMapping(INP_START, GamepadAction.ButtonID.START);
        }

        private void addMapping(InputID inputID, GamepadAction.ButtonID actionID)
        {
            var action = actions[actionID];
            action.currentInputID = inputID;

            if (inputToAction.ContainsKey(inputID))
            {
                var list = inputToAction[inputID];
                list.Add(actionID);
            }
            else
            {
                var list = new List<GamepadAction.ButtonID>();
                list.Add(actionID);
                inputToAction.Add(inputID, list);
            }
        }

        private string getInputForPlayerIdx(string input)
        {
            return input + " P" + (m_playerID + 1);
        }

        // Internal use only
        public void Poll()
        {
            // Buttons
            foreach (InputID inputID in inputToAction.Keys)
            {
                var list = inputToAction[inputID];

                foreach (var actionID in list)
                {
                    var action = actions[actionID];

                    var justPressed = Input.GetButtonDown(getInputForPlayerIdx(inputID.Value));
                    var pressed = Input.GetButton(getInputForPlayerIdx(inputID.Value));
                    var justReleased = Input.GetButtonUp(getInputForPlayerIdx(inputID.Value));

                    action.SetNewButtonInput(justPressed, pressed, justReleased);
                }
            }

            // Axes
            movementDirection.x = Input.GetAxis(getInputForPlayerIdx(INP_HORIZONTAL.Value));
            movementDirection.y = Input.GetAxis(getInputForPlayerIdx(INP_VERTICAL.Value));
            movementDirectionRaw.x = Input.GetAxisRaw(getInputForPlayerIdx(INP_HORIZONTAL.Value));
            movementDirectionRaw.y = Input.GetAxisRaw(getInputForPlayerIdx(INP_VERTICAL.Value));

            if (movementDirectionRaw.magnitude > 0.0f)
            {
                if (m_movementIsInverted)
                {
                    movementDirectionRaw *= -1;
                }
                joystickAction.SetNewJoystickInput(movementDirectionRaw);
            }
        }




        // Public API
        public void InvertMovement()
        {
            m_movementIsInverted = !m_movementIsInverted;
        }

        public GamepadAction Action(GamepadAction.ButtonID actionID)
        {
            return actions[actionID];
        }

        public void RemapAction(GamepadAction.ButtonID actionID, InputID inputID)
        {
            var action = actions[actionID];

            // Remove old mapping
            var list = inputToAction[action.currentInputID];
            list.Remove(actionID);
            if (list.Count == 0)
            {
                inputToAction.Remove(inputID);
            }

            addMapping(inputID, actionID);
        }

        public void ResetMappings()
        {
            inputToAction.Clear();

            for (GamepadAction.ButtonID actionID = 0; actionID < GamepadAction.ButtonID.MAX_ID; ++actionID)
            {
                var action = actions[actionID];
                addMapping(action.defaultInputID, actionID);
            }
        }
    }
}
