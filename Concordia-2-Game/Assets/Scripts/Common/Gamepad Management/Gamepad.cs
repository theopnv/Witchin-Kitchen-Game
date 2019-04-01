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
        public static readonly InputID INP_HORIZONTAL2 = new InputID("Horizontal2");
        public static readonly InputID INP_VERTICAL2 = new InputID("Vertical2");
        public static readonly InputID INP_PUNCH = new InputID("Punch");
        public static readonly InputID INP_INTERACT = new InputID("Interact");
        public static readonly InputID INP_XBUTTON = new InputID("XButton");
        public static readonly InputID INP_YBUTTON = new InputID("YButton");
        public static readonly InputID INP_START = new InputID("Start");
        public static readonly InputID INP_LEFT_TRIGGER = new InputID("LeftTrigger");
        public static readonly InputID INP_RIGHT_TRIGGER = new InputID("RightTrigger");

        private Dictionary<GamepadAction.ID, GamepadAction> actions;      // One to one
        private Dictionary<InputID, List<GamepadAction.ID>> inputToAction; // One to many

        private int m_playerID;
        private bool m_blockGamepad;

        public Gamepad(int playerID)
        {
            m_playerID = playerID;

            actions = new Dictionary<GamepadAction.ID, GamepadAction>();
            //Button Actions
            for (GamepadAction.ID actionID = GamepadAction.ID.__BUTTONS__ + 1; actionID < GamepadAction.ID.__AXES__; ++actionID)
            {
                actions.Add(actionID, new GamepadAction(playerID, actionID, true));
            }
            //Axis Actions
            for (GamepadAction.ID actionID = GamepadAction.ID.__AXES__ + 1; actionID < GamepadAction.ID.MAX_ID; ++actionID)
            {
                actions.Add(actionID, new GamepadAction(playerID, actionID, false));
            }

            inputToAction = new Dictionary<InputID, List<GamepadAction.ID>>();
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
            foreach (KeyValuePair<GamepadAction.ID, GamepadAction> action in actions)
            {
                action.Value.SetInputConsumers(inputConsumers, playerId);
            }
        }

        // DEFINE DEFAULT CONTROLS HERE!
        private void setupDefaultMappings()
        {
            addMapping(INP_PUNCH, GamepadAction.ID.PUNCH);
            addMapping(INP_INTERACT, GamepadAction.ID.INTERACT);
            addMapping(INP_XBUTTON, GamepadAction.ID.XBUTTON);
            addMapping(INP_YBUTTON, GamepadAction.ID.YBUTTON);
            addMapping(INP_START, GamepadAction.ID.START);

            addMapping(INP_HORIZONTAL, GamepadAction.ID.HORIZONTAL);
            addMapping(INP_VERTICAL, GamepadAction.ID.VERTICAL);
            addMapping(INP_HORIZONTAL2, GamepadAction.ID.HORIZONTAL2);
            addMapping(INP_VERTICAL2, GamepadAction.ID.VERTICAL2);
            addMapping(INP_LEFT_TRIGGER, GamepadAction.ID.LEFT_TRIGGER);
            addMapping(INP_RIGHT_TRIGGER, GamepadAction.ID.RIGHT_TRIGGER);
        }

        private void addMapping(InputID inputID, GamepadAction.ID actionID)
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
                var list = new List<GamepadAction.ID>();
                list.Add(actionID);
                inputToAction.Add(inputID, list);
            }
        }

        private string getInputForPlayerIdx(string input)
        {
            return input + " P" + (m_playerID + 1);
        }

        private string getInputForPlayerIdxWithIdOffset(string input)
        {
            return input + " P" + (m_playerID + 2);
        }

        // Internal use only
        public void Poll()
        {
            if (m_blockGamepad)
            {
                return;
            }

            foreach (InputID inputID in inputToAction.Keys)
            {
                var list = inputToAction[inputID];

                foreach (var actionID in list)
                {
                    var action = actions[actionID];

                    var justPressed = Input.GetButtonDown(getInputForPlayerIdx(inputID.Value));
                    var pressed = Input.GetButton(getInputForPlayerIdx(inputID.Value));
                    var justReleased = Input.GetButtonUp(getInputForPlayerIdx(inputID.Value));
                    var axisValue = Input.GetAxis(getInputForPlayerIdx(inputID.Value));

                    //Accept input from the "5th" controller if this controller is lost...
                    var temp = Input.GetJoystickNames();
                    if (temp[m_playerID] == "")
                    {
                        justPressed = justPressed || Input.GetButtonDown(getInputForPlayerIdxWithIdOffset(inputID.Value));
                        pressed = pressed || Input.GetButton(getInputForPlayerIdxWithIdOffset(inputID.Value));
                        justReleased = justReleased || Input.GetButtonUp(getInputForPlayerIdxWithIdOffset(inputID.Value));
                        axisValue += Input.GetAxis(getInputForPlayerIdxWithIdOffset(inputID.Value));
                    }

                    action.SetNewInput(justPressed, pressed, justReleased, axisValue);
                }
            }

        }

        public GamepadAction Action(GamepadAction.ID actionID)
        {
            return actions[actionID];
        }

        public void RemapAction(GamepadAction.ID actionID, InputID inputID)
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

        public void BlockGamepad(bool block)
        {
            m_blockGamepad = block;
        }
    }
}
