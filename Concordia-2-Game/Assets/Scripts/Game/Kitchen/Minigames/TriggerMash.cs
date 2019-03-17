using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static con2.GamepadAction;

namespace con2.game
{

    public class TriggerMash : ACookingMinigame
    {
        private bool m_expectingLeft = true, m_leftWasRaised = true, m_rightWasRaised = true;

        private int m_numberOfPressesRequired;
        public float m_timeToLoseProgress = 1.0f;

        private int m_currentNumberOfPresses = 0;
        private float m_lastMashTime = 0;

        private SpoonTap m_tapper;

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

            m_tapper = GetComponentInChildren<SpoonTap>();
            m_tapper.StartTap();
        }

        public override bool TryToConsumeInput(GamepadAction input)
        {
            if (input.GetActionID().Equals(ID.LEFT_TRIGGER))
            {
                if (m_expectingLeft && m_leftWasRaised && input.m_axisValue > 0.9f)
                {
                    m_leftWasRaised = false;
                    AcceptMash();
                }

                if (!m_leftWasRaised && input.m_axisValue < 0.7f)
                {
                    m_leftWasRaised = true;
                }

                return true;
            }

            if (input.GetActionID().Equals(ID.RIGHT_TRIGGER))
            {
                if (!m_expectingLeft && m_rightWasRaised && input.m_axisValue > 0.9f)
                {
                    m_rightWasRaised = false;
                    AcceptMash();
                }

                if (!m_rightWasRaised && input.m_axisValue < 0.7f)
                {
                    m_rightWasRaised = true;
                }

                return true;
            }

            return false;
        }

        private void AcceptMash()
        {
            m_lastMashTime = Time.time;
            m_currentNumberOfPresses++;

            if (m_currentNumberOfPresses >= m_numberOfPressesRequired)
            {
                EndMinigame();
            }

            m_expectingLeft = !m_expectingLeft;
            m_tapper.Aim(m_expectingLeft);
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
            m_tapper.EndTap();
            StartCoroutine(FireballDelay());
        }

        private IEnumerator FireballDelay()
        {
            var fb = m_stationOwner.GetComponentInChildren<PlayerFireball>();
            fb.SetCanCast(false);

            yield return new WaitForSeconds(2.0f);

            fb.SetCanCast(true);
        }

    }
}
