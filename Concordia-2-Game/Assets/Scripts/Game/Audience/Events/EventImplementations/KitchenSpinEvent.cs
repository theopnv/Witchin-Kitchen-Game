using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class KitchenSpinEvent : AbstractAudienceEvent
    {
        public float m_kitchenSpinDuration = 24.0f; //With spin rate 30.0f, the duration should be 12 * numberOfDesiredSpins
        public float m_spinRate = 30.0f;
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
            var turnDirection = 1.0f;
            if (Random.Range(0.0f, 1.0f) < 0.5f)
            {
                turnDirection = -1.0f;
            }

            m_spinner.ParentObjectsToBase();
            m_spinner.SetSpinDir(turnDirection * m_spinRate * Vector3.up);
            m_spinner.SetIsSpinning(true);

            yield return new WaitForSeconds(m_kitchenSpinDuration);

            m_spinner.SetIsSpinning(false);
            m_spinner.UnparentObjectsFromBase();

            yield return null;
        }

        public override Events.EventID GetEventID()
        {
            return Events.EventID.kitchen_spin;
        }
    }
}
