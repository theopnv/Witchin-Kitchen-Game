using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamepad
{
    public class InputID
    {
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL   = "Vertical";
        public const string PUNCH      = "Punch";
    }

    private Dictionary<GamepadAction.ID, GamepadAction> actions;
    private Dictionary<string, GamepadAction.ID> inputToAction;
    private Vector2 movementDirection;
    private Vector2 movementDirectionRaw;

    public Gamepad()
    {
        actions = new Dictionary<GamepadAction.ID, GamepadAction>();
        for (GamepadAction.ID id = 0; id < GamepadAction.ID.MAX_ID; ++id)
        {
            actions.Add(id, new GamepadAction());
        }

        inputToAction = new Dictionary<string, GamepadAction.ID>();
        setupDefaultMappings();
        foreach (string input in inputToAction.Keys)
        {
            var action = actions[inputToAction[input]];
            action.mappedInput = input;
        }
    }

    // DEFINE DEFAULT CONTROLS HERE!
    private void setupDefaultMappings()
    {
        inputToAction.Add(InputID.PUNCH, GamepadAction.ID.PUNCH);
    }

    private string getInputForPlayerIdx(string input, int playerIdx)
    {
        return input + " " + playerIdx;
    }

    // Internal use only
    public void Poll(int playerIdx)
    {
        // Buttons
        foreach (string input in inputToAction.Keys)
        {
            var action = actions[inputToAction[input]];

            var justPressed = Input.GetButtonDown(getInputForPlayerIdx(input, playerIdx));
            var pressed = Input.GetButton(getInputForPlayerIdx(input, playerIdx));
            var justReleased = Input.GetButtonUp(getInputForPlayerIdx(input, playerIdx));

            action.SetNew(justPressed, pressed, justReleased);
        }

        // Axes
        movementDirection.x = Input.GetAxis(getInputForPlayerIdx(InputID.HORIZONTAL, playerIdx));
        movementDirection.y = Input.GetAxis(getInputForPlayerIdx(InputID.VERTICAL, playerIdx));
        movementDirectionRaw.x = Input.GetAxisRaw(getInputForPlayerIdx(InputID.HORIZONTAL, playerIdx));
        movementDirectionRaw.y = Input.GetAxisRaw(getInputForPlayerIdx(InputID.VERTICAL, playerIdx));
    }




    // Public API
    public bool InvertMovement;

    public GamepadAction Action(GamepadAction.ID id)
    {
        return actions[id];
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

    public void RemapAction(GamepadAction.ID id, string input)
    {
        // Remove old mapping
        var action = actions[id];
        inputToAction.Remove(action.mappedInput);

        inputToAction.Add(input, id);
    }

    public void ResetMappings()
    {
        inputToAction.Clear();

        for (GamepadAction.ID id = 0; id < GamepadAction.ID.MAX_ID; ++id)
        {
            var action = actions[id];

            inputToAction.Add(action.mappedInput, id);
        }
    }
}
