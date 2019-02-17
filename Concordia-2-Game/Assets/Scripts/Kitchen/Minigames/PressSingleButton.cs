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
            m_playerPresses.Add(GamepadMgr.Pad(player.GetPlayer().GetComponent<FightControls>().PlayerIndex).Action(GamepadAction.ID.INTERACT));
        }
    }

    override public void UpdateMinigameSpecifics()
    {
        foreach (PlayerRange player in m_players)
        {
            //Await game input
            if (Input.GetKeyDown(KeyCode.E))
            {
                EndMinigame();
            }
        }
    }

    override public void EndMinigameSpecifics()
    {

    }

}
