using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public abstract class AHitAllInRange : MonoBehaviour
    {
        [SerializeField] protected float m_strength, m_stunTime;

        protected bool m_stunnedSomething = false;

        private List<KeyValuePair<GameObject, IPunchable[]>> m_punchablesInRadius;
        [HideInInspector] public List<GameObject> m_immuneTargets;

        private void Awake()
        {
            m_immuneTargets = new List<GameObject>();
            m_punchablesInRadius = new List<KeyValuePair<GameObject, IPunchable[]>>();
            OnStart();
        }

        protected abstract void OnStart();

        void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }

            IPunchable[] targetPunchableComponents = other.gameObject.GetComponentsInChildren<IPunchable>();
            if (targetPunchableComponents.Length > 0)
                m_punchablesInRadius.Add(new KeyValuePair<GameObject, IPunchable[]>(other.gameObject, targetPunchableComponents));
        }

        void OnTriggerExit(Collider other)
        {
            KeyValuePair<GameObject, IPunchable[]> leavingObject = m_punchablesInRadius.Find(item => item.Key.Equals(other.gameObject));
            m_punchablesInRadius.Remove(leavingObject);
        }

        public void Hit(CamShakeMgr.Intensity shakeIntensity)
        {
            m_stunnedSomething = false;

            for (int i = 0; i < m_punchablesInRadius.Count; i++)
            {
                KeyValuePair<GameObject, IPunchable[]> target = m_punchablesInRadius[i];
                if (target.Key == null)
                {
                    m_punchablesInRadius.Remove(target);
                    i--;
                }
                else if (!m_immuneTargets.Contains(target.Key))
                {
                    foreach (IPunchable punchableComponent in target.Value)
                    {
                        Vector3 hitVector = (target.Key.transform.position - transform.position).normalized * m_strength;
                        punchableComponent.Punch(ModulateHitVector(hitVector), m_stunTime);
                        if (target.Key.tag == "Player")
                            m_stunnedSomething = true;
                    }
                }
            }

            if (m_stunnedSomething)
                CamShakeMgr.Get().Shake(shakeIntensity);

            AfterHitting();
        }

        protected virtual Vector3 ModulateHitVector(Vector3 hitVector)
        {
            hitVector.y = 1.0f; //small upward force on all regular hits so objects don't drag on the ground
            return hitVector;
        }

        protected abstract void AfterHitting();
    }
}
