using con2;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static con2.GamepadAction;

public class MashButton : CookingMinigame
{
    public ButtonID m_inputToMash = ButtonID.INTERACT;
    public const int m_numberOfPressesRequired = 11;
    public float m_timeToLoseProgress = 1.0f;

    private int m_currentNumberOfPresses = 0;
    private float m_lastMashTime = 0;


    override public void StartMinigameSpecifics()
    {

    }

    public override bool TryToConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(m_inputToMash))
        {
            m_lastMashTime = Time.time;
            m_currentNumberOfPresses++;
            if (m_currentNumberOfPresses >= m_numberOfPressesRequired)
            {
                EndMinigame();
            }
            return true;
        }
        return false;
    }

    override public void UpdateMinigameSpecifics()
    {
        if (m_currentNumberOfPresses > 0)
        {
            if (Time.time - m_lastMashTime > m_timeToLoseProgress)
            {
                m_lastMashTime = Time.time;
                m_currentNumberOfPresses--;
            }
        }
    }

    override public void EndMinigameSpecifics()
    {

    }

}
