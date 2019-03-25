using con2.messages;
using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class LightningManager : MonoBehaviour, ISpellSubscriber
    {
        private LightningBoltScript m_lightningThing;
        public float m_timeBetweenStrikes = 0.1f;
        public int m_numOfStrikes = 3;
        private AMainManager m_mainManager;
        private AudioSource audioSource;

        void Start()
        {
            m_lightningThing = GetComponent<LightningBoltScript>();
            m_mainManager = FindObjectOfType<AMainManager>();
            audioSource = GetComponent<AudioSource>();

            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            var eventManager = managers.GetComponentInChildren<SpellsManager>();
            for (int i = 0; i < (int)Spells.SpellID.max_id; i++)
                eventManager.AddSubscriber((Spells.SpellID)i, this);
        }

        public void ActivateSpellMode(messages.Player targetedPlayer)
        {
            var player = m_mainManager.GetPlayerById(targetedPlayer.id).gameObject;
            m_lightningThing.EndObject = player;
            audioSource.Play();
            var light = player.GetComponentInChildren<Light>();
            StartCoroutine(TriggerLightning(light));
        }

        private IEnumerator TriggerLightning(Light strikeLight)
        {
            int i = m_numOfStrikes;
            while (i > 0)
            {
                i--;
                m_lightningThing.Trigger();
                StartCoroutine(FlickerLight(strikeLight));
                yield return new WaitForSeconds(m_timeBetweenStrikes);
            }
        }

        private IEnumerator FlickerLight(Light strikeLight)
        {
            strikeLight.enabled = true;
            yield return new WaitForSeconds(m_timeBetweenStrikes / 2.0f);
            strikeLight.enabled = false;
        }
    }
}
