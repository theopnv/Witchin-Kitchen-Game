using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class FightStun : MonoBehaviour, IInputConsumer
    {
        public AnimationCurve modifierCurve;
        StunStars m_visualFeedback;

        private float timerElapsed = 0.0f;
        private float timerMax = 0.0f;

        private void FixedUpdate()
        {
            timerElapsed = Mathf.Min(timerMax, timerElapsed + Time.deltaTime);
            m_visualFeedback = GetComponentInChildren<StunStars>();
        }

        public void Stun(float seconds)
        {
            timerMax = seconds;
            timerElapsed = 0.0f;
            m_visualFeedback.Play(seconds);
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (input.GetActionID().Equals(GamepadAction.ID.HORIZONTAL)
                || input.GetActionID().Equals(GamepadAction.ID.VERTICAL))
            {
                return false;
            }
            else
            {
                return timerElapsed/timerMax <= 0.9f;
            }
        }

        public float getMovementModifier()
        {
            // Avoid division by 0
            if (timerMax == 0.0f)
                return 1.0f;

            return modifierCurve.Evaluate(timerElapsed / timerMax);
        }
    }

}
