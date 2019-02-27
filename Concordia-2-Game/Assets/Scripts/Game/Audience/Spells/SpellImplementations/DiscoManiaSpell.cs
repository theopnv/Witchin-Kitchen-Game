using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class DiscoManiaSpell : ASpell
    {
        public float m_DiscoManiaDuration = 10.0f;

        // Start is called before the first frame update

        void Start()
        {
            SetUpSpell();
        }

        public override IEnumerator SpellImplementation()
        {
            var playerGamepad = GamepadMgr.Pad(_TargetedPlayer.id);
            playerGamepad.InvertMovement();

            yield return new WaitForSeconds(m_DiscoManiaDuration);

            playerGamepad.InvertMovement();
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.disco_mania;
        }
    }

}
