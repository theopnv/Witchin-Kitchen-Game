using con2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class StirMinigame : ACookingMinigame
    {
        private static Vector2
            TOP_LEFT = new Vector2(-1.0f, 1.0f).normalized
            , TOP = new Vector2(0.0f, 1.0f).normalized
            , TOP_RIGHT = new Vector2(1.0f, 1.0f).normalized
            , RIGHT = new Vector2(1.0f, 0.0f).normalized
            , BOTTOM_RIGHT = -TOP_LEFT
            , BOTTOM = -TOP
            , BOTTOM_LEFT = -TOP_RIGHT
            , LEFT = -RIGHT;

        private static Vector2[] GOALS = { TOP, TOP_RIGHT, RIGHT, BOTTOM_RIGHT, BOTTOM, BOTTOM_LEFT, LEFT, TOP_LEFT };

        private Vector2 m_pointingDirection;
        private int m_turnsRequired, m_fullTurnCount, m_currentGoal, m_turnDirection;

        private Spin2Win m_spoonSpinner;

        override public void StartMinigameSpecifics()
        {
            m_pointingDirection = Vector3.zero;
            m_currentGoal = 0;
            m_fullTurnCount = 0;
            m_turnsRequired = 2;

            if (Random.Range(0.0f, 1.0f) < 0.5f)
            {
                m_turnDirection = -1;
            }
            else
            {
                m_turnDirection = 1;
            }

            m_spoonSpinner = GetComponentInChildren<Spin2Win>();
            m_spoonSpinner.SetTargetYAngle(m_currentGoal*45);
            m_spoonSpinner.SetTargetRotation(m_turnDirection);

            m_prompt.text = "Stir R-joystick " + (m_turnDirection == 1 ? "" : "counter-") + "clockwise " + m_turnsRequired + " times!";
        }

        public override bool TryToConsumeInput(GamepadAction input)
        {
            if (input.GetActionID().Equals(con2.GamepadAction.ID.HORIZONTAL2))
            {
                float joystick = input.m_axisValue;
                m_pointingDirection.x += joystick;
                return true;
            }
            if (input.GetActionID().Equals(con2.GamepadAction.ID.VERTICAL2))
            {
                float joystick = input.m_axisValue;
                m_pointingDirection.y = joystick;
                return true;
            }
            return false;
        }

        override public void UpdateMinigameSpecifics()
        {
            m_pointingDirection.Normalize();
            if (Vector3.Dot(m_pointingDirection, GOALS[m_currentGoal]) > 0.9f)    //The dot product of two parallel normalized vectors is 1
            {
                CycleCurrentGoal();
            }
        }

        override public void EndMinigameSpecifics()
        {

        }

        private void CycleCurrentGoal()
        {
            m_currentGoal += m_turnDirection;
            m_currentGoal %= GOALS.Length;

            if (m_currentGoal < 0)
            {
                m_currentGoal = GOALS.Length + m_currentGoal;
            }

            if (GOALS[m_currentGoal] == TOP)
            {
                FullTurnComplete();
            }

            m_spoonSpinner.SetTargetYAngle(m_currentGoal * 45);
        
        }

        private void FullTurnComplete()
        {
            m_fullTurnCount++;

            m_prompt.text = "Stir R-joystick " + (m_turnDirection == 1 ? "" : "counter-") + "clockwise " + (m_turnsRequired - m_fullTurnCount) + " times!";

            if (m_fullTurnCount == m_turnsRequired)
            {
                EndMinigame();
            }
        }
    }

}
