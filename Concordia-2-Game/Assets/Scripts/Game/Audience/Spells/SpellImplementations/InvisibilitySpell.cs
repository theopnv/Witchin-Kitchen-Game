using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class InvisibilitySpell : ASpell
    {
        public float m_InvisibilityDuration = 10.0f;

        void Start()
        {
            SetUpSpell();
        }

        public override IEnumerator SpellImplementation()
        {
            var targetPlayer = m_mainManager.GetPlayerById(_TargetedPlayer.id);
            var skin = targetPlayer.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
            var puff = targetPlayer.transform.Find("Puff").gameObject;

            if (skin)
            {
                Debug.Log("Skin found!");
                skin.SetActive(false);
                puff.SetActive(false);
                puff.SetActive(true);
            }

            yield return new WaitForSeconds(m_InvisibilityDuration);

            if (skin)
            {
                skin.SetActive(true);
                puff.SetActive(false);
                puff.SetActive(true);
            }

        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.invisibility;
        }
    }

}
