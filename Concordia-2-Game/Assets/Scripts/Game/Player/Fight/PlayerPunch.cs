using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class PlayerPunch : AHitAllInRange, IInputConsumer
    {
        public CamShakeMgr.Intensity ShakeIntensity = CamShakeMgr.Intensity.TINY;

        private AudioSource audioSource;

        public float m_punchUpwardsForce = 4.0f;

        private bool m_canPunch = true;
        private float m_punchCooldownTimer;
        [SerializeField] public float m_reloadSeconds;

        private Color m_startColor;
        private Color m_noPunchColor = new Color(1.0f, 1.0f, 1.0f);
        private MeshRenderer m_renderer;

        protected override void OnStart()
        {
            //To be used on model's gloves?
            /*
            m_startColor = GetComponent<MeshRenderer>().material.color;
            m_renderer = GetComponent<MeshRenderer>();
            */

            audioSource = GetComponent<AudioSource>();
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

            if (input.GetActionID().Equals(con2.GamepadAction.ID.PUNCH))
            {
                Hit();
                audioSource.Play();
                if (m_didStun)
                    CamShakeMgr.Get().Shake(ShakeIntensity);
                StartCoroutine(PunchCooldown());
                return true;
            }

            return false;
        }

        protected override Vector3 ModulateHitVector(Vector3 hitVector)
        {
            hitVector.y = m_punchUpwardsForce;
            return hitVector;
        }

        public void ModulatePunchCooldown(float punchCooldownhMultiplier)
        {
            m_reloadSeconds *= punchCooldownhMultiplier;
        }

        public void ModulatePunchStrength(float punchStrengthMultiplier)
        {
            m_strength *= punchStrengthMultiplier;
        }

        protected override void AfterHitting()
        { }

        private IEnumerator PunchCooldown()
        {
            m_canPunch = false;
            m_punchCooldownTimer = m_reloadSeconds;
            yield return new WaitForSeconds(m_reloadSeconds);
            m_canPunch = true;
        }

        private void ShowPunchCooldown()
        {
            // Visualize the reload timer
            float progress = 1.0f - m_punchCooldownTimer/m_reloadSeconds;
            m_renderer.material.color = m_startColor * progress + m_noPunchColor * (1.0f - progress);
        }

    }

}
