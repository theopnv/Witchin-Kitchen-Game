using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class DiscoManiaSpell : ASpell
    {
        public float m_DiscoManiaDuration = 10.0f;

        private Gamepad[] m_playerGamepads;
        // Start is called before the first frame update

        void Start()
        {
            SetUpSpell();
        }

        public override IEnumerator SpellImplementation()
        {
            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            GameObject[] players = managers.GetComponentInChildren<SpawnPlayersController>().GetPlayers();

            m_playerGamepads = new Gamepad[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                m_playerGamepads[i] = GamepadMgr.Pad(i);
            }

            foreach (Gamepad pad in m_playerGamepads)
            {
                pad.InvertMovement();
            }

            yield return new WaitForSeconds(m_DiscoManiaDuration);

            foreach (Gamepad pad in m_playerGamepads)
            {
                pad.InvertMovement();
            }
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.disco_mania;
        }
    }

}
