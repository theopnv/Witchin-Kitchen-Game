using con2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class PlayerFireball : MonoBehaviour, IInputConsumer
    {
        [SerializeField] private GameObject m_fireballPrefab;
        [SerializeField] private GameObject _FireballIndicatorPrefab;
        [SerializeField] private GameObject m_spawnLocation;
        [SerializeField] public float m_reloadSeconds, m_recoil;

        private Rigidbody m_player;
        public FireballEmberIndicator m_indicator;
        private bool m_canCastFireball = true;

        public void Start()
        {
            Transform parent = transform.parent;
            if (parent)
            {
                m_player = parent.gameObject.GetComponent<Rigidbody>();
                var fireballIndicatior = Instantiate(_FireballIndicatorPrefab);
                m_indicator = fireballIndicatior.GetComponent<FireballEmberIndicator>();
                m_indicator.SetPlayer(m_player.gameObject);
            }
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (!m_canCastFireball)
                return false;

            if (input.GetActionID().Equals(con2.GamepadAction.ID.RIGHT_TRIGGER))
            {
                if (input.m_axisValue > 0.5)
                {
                    CastFireball();
                    return true;
                }
            }

            return false;
        }

        public void CastFireball()
        {
            //We position the fireball initially based on the player
            GameObject newFireball = Instantiate(m_fireballPrefab, m_spawnLocation.transform.position, m_spawnLocation.transform.rotation);
            newFireball.transform.forward = m_spawnLocation.transform.forward;
            SetUpExplosive(newFireball, transform.parent.gameObject, false);

            Recoil();

            StartCoroutine(FireballCooldown());
        }

        public void FireballTurret(GameObject launcher)
        {
            if (m_spawnLocation)
            {
                GameObject newFireball = Instantiate(m_fireballPrefab, m_spawnLocation.transform.position, m_spawnLocation.transform.rotation);
                newFireball.transform.forward = m_spawnLocation.transform.forward;
                SetUpExplosive(newFireball, launcher, true);
            }
        }

        private void SetUpExplosive(GameObject newFireball, GameObject launcher, bool immuneLauncher)
        {
            ExplosiveItem explosive = newFireball.GetComponent<ExplosiveItem>();
            explosive.m_launcher = launcher;

            if (immuneLauncher)
            {
                explosive.m_immuneTargets.Add(launcher);
            }
        }

        private void Recoil()
        {
            m_player.AddForce(-transform.forward * m_recoil, ForceMode.VelocityChange);
        }

        public void SetCanCast(bool canCast)
        {
            m_canCastFireball = canCast;
        }

        private IEnumerator FireballCooldown()
        {
            m_canCastFireball = false;
            m_indicator.SetCharged(false);
            yield return new WaitForSeconds(m_reloadSeconds);
            m_canCastFireball = true;
            m_indicator.SetCharged(true);
        }

        public void ModulateReloadTime(float reloadModulator)
        {
            m_reloadSeconds *= reloadModulator;
        }
    }
}
