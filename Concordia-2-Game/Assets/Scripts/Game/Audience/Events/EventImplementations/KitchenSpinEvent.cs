using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class KitchenSpinEvent : AbstractAudienceEvent
    {
        private const float m_kitchenSpinDuration = 24.0f;
        private const float m_accelerationTime = 2.0f;
        private const float m_maxSpeedTime = m_kitchenSpinDuration - m_accelerationTime * 2.0f;
        private const float m_totalSpin = 720.0f; // (degrees)
        private const float m_maxSpinRate = m_totalSpin * 2.0f / (m_kitchenSpinDuration + m_maxSpeedTime);

        public GameObject m_spinBase;
        private KitchenSpin m_spinner;

        void Start()
        {
            SetUpEvent();
            m_spinner = m_spinBase.GetComponent<KitchenSpin>();
        }

        public override void EventStart()
        { }

        public override IEnumerator EventImplementation()
        {
            _MessageFeedManager.AddMessageToFeed("The audience made the kitchens spin!", MessageFeedManager.MessageType.arena_event);
            var turnDirection = 1.0f;
            if (Random.Range(0.0f, 1.0f) < 0.5f)
            {
                turnDirection = -1.0f;
            }

            m_spinner.ParentObjectsToBase();
            m_spinner.SetSpinDir(turnDirection * Vector3.up);

            StartCoroutine(StartSpin());

            yield return null;
        }

        public IEnumerator StartSpin()
        {
            m_spinner.SetIsSpinning(true);
            var startTime = Time.time;
            while (Time.time - startTime <= 2.0f)
            {
                m_spinner.SetSpinSpeed(m_maxSpinRate * ((Time.time - startTime) / m_accelerationTime));
                yield return new WaitForEndOfFrame();
            }

            m_spinner.SetSpinSpeed(m_maxSpinRate);
            yield return new WaitForSeconds(m_maxSpeedTime);
            StartCoroutine(EndSpin());
        }

        public IEnumerator EndSpin()
        {
            var startTime = Time.time;
            while (Time.time - startTime <= m_accelerationTime)
            {
                m_spinner.SetSpinSpeed(m_maxSpinRate * (((startTime + m_accelerationTime) - Time.time)) / m_accelerationTime);
                yield return new WaitForEndOfFrame();
            }
            m_spinner.SetIsSpinning(false);
            m_spinner.UnparentObjectsFromBase();
        }

        public override Events.EventID GetEventID()
        {
            return Events.EventID.kitchen_spin;
        }
    }
}
