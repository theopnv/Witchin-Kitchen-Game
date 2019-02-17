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
        foreach (PlayerRange player in m_players)
        {
            //m_playerPresses.Add(GamepadMgr.Pad(player.GetPlayer().GetComponent<FightControls>().PlayerIndex).Action(GamepadAction.ID.INTERACT));
        }
    }

    public override bool ConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.INTERACT))
        {
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
