using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class LightningManager : MonoBehaviour
    {
        private LightningBoltScript m_lightningThing;

        void Start()
        {
            m_lightningThing = GetComponent<LightningBoltScript>();
        }

        void OnSpellCast(PlayerManager targetPlayer)
        {
            m_lightningThing.EndObject = targetPlayer.gameObject;
            m_lightningThing.Trigger();
        }
    }
}
