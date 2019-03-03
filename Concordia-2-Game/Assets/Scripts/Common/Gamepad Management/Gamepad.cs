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
        public static readonly InputID INP_LEFT_TRIGGER = new InputID("LeftTrigger");
        public static readonly InputID INP_RIGHT_TRIGGER = new InputID("RightTrigger");

        private Dictionary<GamepadAction.ID, GamepadAction> actions;      // One to one
        private Dictionary<InputID, List<GamepadAction.ActionID>> inputToAction; // One to many

        private int m_playerID;

        public Gamepad(int playerID)
        {
            m_playerID = playerID;

            actions = new Dictionary<GamepadAction.ID, GamepadAction>();
            //Button Actions
            for (GamepadAction.ID actionID = GamepadAction.ID.__BUTTONS__ + 1; actionID < GamepadAction.ID.__AXES__; ++actionID)
            {
                actions.Add(actionID, new GamepadAction(playerID, new GamepadAction.ActionID(actionID, true)));
            }
            //Axis Actions
            for (GamepadAction.ID actionID = GamepadAction.ID.__AXES__ + 1; actionID < GamepadAction.ID.MAX_ID; ++actionID)
            {
                actions.Add(actionID, new GamepadAction(playerID, new GamepadAction.ActionID(actionID, false)));
            }

            inputToAction = new Dictionary<InputID, List<GamepadAction.ActionID>>();
            setupDefaultMappings();
            foreach (InputID inputID in inputToAction.Keys)
            {
                // Save default mapping
                var list = inputToAction[inputID];
                foreach (var actionID in list)
                {
                    var action = actions[actionID.GetID()];
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
            addMapping(INP_PUNCH, new GamepadAction.ActionID(GamepadAction.ID.PUNCH, true));
            addMapping(INP_INTERACT, new GamepadAction.ActionID(GamepadAction.ID.INTERACT, true));
            addMapping(INP_START, new GamepadAction.ActionID(GamepadAction.ID.START, true));

            addMapping(INP_HORIZONTAL, new GamepadAction.ActionID(GamepadAction.ID.HORIZONTAL, false));
            addMapping(INP_VERTICAL, new GamepadAction.ActionID(GamepadAction.ID.VERTICAL, false));
            addMapping(INP_LEFT_TRIGGER, new GamepadAction.ActionID(GamepadAction.ID.LEFT_TRIGGER, false));
            addMapping(INP_RIGHT_TRIGGER, new GamepadAction.ActionID(GamepadAction.ID.RIGHT_TRIGGER, false));
        }

        private void addMapping(InputID inputID, GamepadAction.ActionID actionID)
        {
            var action = actions[actionID.GetID()];
            action.currentInputID = inputID;

            if (inputToAction.ContainsKey(inputID))
            {
                var list = inputToAction[inputID];
                list.Add(actionID);
            }
            else
            {
                var list = new List<GamepadAction.ActionID>();
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

            foreach (InputID inputID in inputToAction.Keys)
            {
                var list = inputToAction[inputID];

                foreach (var actionID in list)
                {
                    var action = actions[actionID.GetID()];

                    var justPressed = Input.GetButtonDown(getInputForPlayerIdx(inputID.Value));
                    var pressed = Input.GetButton(getInputForPlayerIdx(inputID.Value));
                    var justReleased = Input.GetButtonUp(getInputForPlayerIdx(inputID.Value));
                    var axisValue = Input.GetAxis(getInputForPlayerIdx(inputID.Value));
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
            list.Remove(action.GetActionID());
            if (list.Count == 0)
            {
                inputToAction.Remove(inputID);
            }

            addMapping(inputID, action.GetActionID());
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
                    var action = actions[actionID.GetID()];
                    action.defaultInputID = inputID;
                }
            }
        }
    }
}
