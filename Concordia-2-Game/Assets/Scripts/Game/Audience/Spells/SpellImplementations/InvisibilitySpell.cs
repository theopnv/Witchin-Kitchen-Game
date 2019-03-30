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
            var puff = targetPlayer.transform.Find("Puff").gameObject;
            var skins = targetPlayer.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var skin in skins)
            {
                skin.gameObject.SetActive(false);
            }
            if (skins.Length > 0)
            {
                Debug.Log("Skin found!");
                puff.SetActive(false);
                puff.SetActive(true);
            }

            yield return new WaitForSeconds(m_InvisibilityDuration);

            foreach (var skin in skins)
            {
                skin.gameObject.SetActive(true);
            }
            if (skins.Length > 0)
            {
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
