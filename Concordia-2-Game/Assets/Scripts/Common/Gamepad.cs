using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static readonly InputID INP_HORIZONTAL   = new InputID("Horizontal");
    public static readonly InputID INP_VERTICAL     = new InputID("Vertical");
    public static readonly InputID INP_PUNCH        = new InputID("Punch");

    private Dictionary<GamepadAction.ID, GamepadAction> actions;      // One to one
    private Dictionary<InputID, List<GamepadAction.ID>> inputToAction; // One to many
    private Vector2 movementDirection;
    private Vector2 movementDirectionRaw;

    public Gamepad()
    {
        actions = new Dictionary<GamepadAction.ID, GamepadAction>();
        for (GamepadAction.ID actionID = 0; actionID < GamepadAction.ID.MAX_ID; ++actionID)
        {
            actions.Add(actionID, new GamepadAction());
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

    // DEFINE DEFAULT CONTROLS HERE!
    private void setupDefaultMappings()
    {
        addMapping(INP_PUNCH, GamepadAction.ID.PUNCH);
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

    private string getInputForPlayerIdx(string input, int playerIdx)
    {
        return input + " P" + (playerIdx + 1);
    }

    // Internal use only
    public void Poll(int playerIdx)
    {
        // Buttons
        foreach (InputID inputID in inputToAction.Keys)
        {
            var list = inputToAction[inputID];

            foreach (var actionID in list)
            {
                var action = actions[actionID];

                var justPressed = Input.GetButtonDown(getInputForPlayerIdx(inputID.Value, playerIdx));
                var pressed = Input.GetButton(getInputForPlayerIdx(inputID.Value, playerIdx));
                var justReleased = Input.GetButtonUp(getInputForPlayerIdx(inputID.Value, playerIdx));

                action.SetNew(justPressed, pressed, justReleased);
            }
        }

        // Axes
        movementDirection.x = Input.GetAxis(getInputForPlayerIdx(INP_HORIZONTAL.Value, playerIdx));
        movementDirection.y = Input.GetAxis(getInputForPlayerIdx(INP_VERTICAL.Value, playerIdx));
        movementDirectionRaw.x = Input.GetAxisRaw(getInputForPlayerIdx(INP_HORIZONTAL.Value, playerIdx));
        movementDirectionRaw.y = Input.GetAxisRaw(getInputForPlayerIdx(INP_VERTICAL.Value, playerIdx));
    }




    // Public API
    public bool InvertMovement;

    public GamepadAction Action(GamepadAction.ID actionID)
    {
        return actions[actionID];
    }

    public Vector2 MovementDirection()
    {
        if (InvertMovement)
            return -movementDirection;

        return movementDirection;
    }

    public Vector2 MovementDirectionRaw()
    {
        if (InvertMovement)
            return -movementDirectionRaw;

        return movementDirectionRaw;
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
        inputToAction.Clear();

        for (GamepadAction.ID actionID = 0; actionID < GamepadAction.ID.MAX_ID; ++actionID)
        {
            var action = actions[actionID];
            addMapping(action.defaultInputID, actionID);
        }
    }
}
