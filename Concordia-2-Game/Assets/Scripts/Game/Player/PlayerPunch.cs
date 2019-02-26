using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class PlayerPunch : MonoBehaviour, IInputConsumer
    {
        private bool m_canPunch = true;

        private List<KeyValuePair<GameObject, IPunchable[]>> m_punchTargets;

        public float m_punchUpwardsForce = 4.0f;
        public float m_punchForceMultiplier = 7.0f;
        public float m_punchStunSeconds = 1.0f;
        public float m_punchReloadSeconds = 1.33f;
        private float m_punchCooldownTimer;

        private Color m_startColor;
        private Color m_noPunchColor = new Color(1.0f, 1.0f, 1.0f);
        private MeshRenderer m_renderer;

        // Start is called before the first frame update
        void Start()
        {
            m_punchTargets = new List<KeyValuePair<GameObject, IPunchable[]>>();

            //To be used on model's gloves?
            /*
            m_startColor = GetComponent<MeshRenderer>().material.color;
            m_renderer = GetComponent<MeshRenderer>();
            */
        }

        void Update()
        {
            /*
            if (m_punchCooldownTimer > 0.01f)
            {
                m_punchCooldownTimer -= Time.deltaTime;
                ShowPunchCooldown();
            }
            */
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (!m_canPunch)
                return false;

            if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.PUNCH))
            {
                DoPunch();
                return true;
            }

            return false;
        }

        private void DoPunch()
        {
            Vector3 puncherPosition = transform.parent.position;
            foreach (KeyValuePair<GameObject, IPunchable[]> target in m_punchTargets)
            {
                foreach (IPunchable punchableComponent in target.Value)
                {
                    Vector3 knockVelocity = target.Key.transform.position - puncherPosition;
                    knockVelocity = knockVelocity.normalized;
                    knockVelocity *= m_punchForceMultiplier;
                    knockVelocity.y = m_punchUpwardsForce;

                    punchableComponent.Punch(knockVelocity, m_punchStunSeconds);
                }
            }

            m_canPunch = false;
            StartCoroutine(PunchCooldown());
        }

        private IEnumerator PunchCooldown()
        {
            m_punchCooldownTimer = m_punchReloadSeconds;
            yield return new WaitForSeconds(m_punchReloadSeconds);
            m_canPunch = true;
        }

        private void ShowPunchCooldown()
        {
            // Visualize the reload timer
            float progress = 1.0f - m_punchCooldownTimer/m_punchReloadSeconds;
            m_renderer.material.color = m_startColor * progress + m_noPunchColor * (1.0f - progress);
        }

        private bool IsTarget(Collider other)
        {
            // Ignore parent collision
            return other.gameObject != gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTarget(other))
            {
                IPunchable[] targetPunchableComponents = other.gameObject.GetComponentsInChildren<IPunchable>();
                if (targetPunchableComponents.Length > 0)
                    m_punchTargets.Add(new KeyValuePair<GameObject, IPunchable[]>(other.gameObject, targetPunchableComponents));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsTarget(other))
            {
                KeyValuePair<GameObject, IPunchable[]> leavingObject = m_punchTargets.Find(item => item.Key.Equals(other.gameObject));
                m_punchTargets.Remove(leavingObject);
            }
        }

    }

}
