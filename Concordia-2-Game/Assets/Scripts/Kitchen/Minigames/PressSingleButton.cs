using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressSingleButton : CookingMinigame
{
    List<GamepadAction> m_playerPresses;

    override public void StartMinigameSpecifics()
    {

    }

    public override bool TryToConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.INTERACT))
        {
            EndMinigame();
            return true;
        }
        return false;
    }

    override public void UpdateMinigameSpecifics()
    {

    }

    override public void EndMinigameSpecifics()
    {

    }

}
