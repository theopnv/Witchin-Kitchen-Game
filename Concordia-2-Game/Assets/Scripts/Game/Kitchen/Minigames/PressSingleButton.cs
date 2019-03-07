using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressSingleButton : ACookingMinigame
{
    override public void StartMinigameSpecifics()
    {

    }

    public override void BalanceMinigame(MainGameManager mgm)
    {
        
    }

    public override bool TryToConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ID.INTERACT))
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
