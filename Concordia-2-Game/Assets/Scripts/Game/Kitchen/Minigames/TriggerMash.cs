using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static con2.GamepadAction;

namespace con2.game
{

    public class TriggerMash : ACookingMinigame
    {
        private bool m_expectingLeft = true;

        private int m_numberOfPressesRequired;
        public float m_timeToLoseProgress = 1.0f;

        private int m_currentNumberOfPresses = 0;
        private float m_lastMashTime = 0;

        private SpoonMash m_masher;

        public override void BalanceMinigame()
        {
            switch (m_stationOwner.PlayerRank)
            {
                case PlayerManager.Rank.FIRST:
                    m_numberOfPressesRequired = 16;
                    break;
                case PlayerManager.Rank.LAST:
                    m_numberOfPressesRequired = 6;
                    break;
                default:
                    m_numberOfPressesRequired = 11;
                    break;
            }
        }

        override public void StartMinigameSpecifics()
        {
            m_currentNumberOfPresses = 0;
            m_lastMashTime = 0;

            m_prompt.text = "Alternate L and R triggers!";

            m_masher = GetComponentInChildren<SpoonMash>();
            m_masher.StartMash();
        }

        public override bool TryToConsumeInput(GamepadAction input)
        {
            if (input.GetActionID().Equals(ID.LEFT_TRIGGER) && input.m_axisValue > 0.8)
            { 
                if (m_expectingLeft && input.m_axisValue > 0.8)
                {
                    Debug.Log("Left Trigger");
                    AcceptMash();
                }

                return true;
            }

            if (input.GetActionID().Equals(ID.RIGHT_TRIGGER) && input.m_axisValue > 0.8)
            {
                if (!m_expectingLeft && input.m_axisValue > 0.8)
                {
                    Debug.Log("Right Trigger");
                    AcceptMash();
                }

                return true;
            }

            return false;
        }

        private void AcceptMash()
        {
            m_lastMashTime = Time.time;
            m_currentNumberOfPresses++;
            m_masher.Mash();

            if (m_currentNumberOfPresses >= m_numberOfPressesRequired)
            {
                EndMinigame();
            }

            m_expectingLeft = !m_expectingLeft;
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
            m_masher.EndMash();
        }

    }
}
