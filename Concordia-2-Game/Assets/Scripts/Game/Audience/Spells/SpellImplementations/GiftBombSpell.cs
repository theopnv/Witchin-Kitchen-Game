using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class GiftBombSpell : ASpell
    {
        public GameObject m_giftPrefab, m_bombPrefab;

        void Start()
        {
            SetUpSpell();
        }

        public override IEnumerator SpellImplementation()
        {
            var targetPlayer = m_mainGameManager.Players[_TargetedPlayer.id];
            var gift = Instantiate(m_giftPrefab, targetPlayer.transform.position + new Vector3(0, 0, -2), new Quaternion(0, 0, 0, 0));
            var giftComponent = gift.GetComponent<Gift>();
            giftComponent.SetIngredientType(Ingredient.NOT_AN_INGREDIENT);
            giftComponent.SetContents(m_bombPrefab);
            giftComponent.SetColor(ColorsManager.Get().PlayerGiftColors[_TargetedPlayer.id]);
            yield return null;
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.gift_bomb;
        }
    }

}
