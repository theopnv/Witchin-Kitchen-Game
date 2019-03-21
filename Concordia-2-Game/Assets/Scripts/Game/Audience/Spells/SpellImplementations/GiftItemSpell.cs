using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class GiftItemSpell : ASpell
    {
        public GameObject m_giftPrefab;

        void Start()
        {
            SetUpSpell();
        }

        public override IEnumerator SpellImplementation()
        {
            var targetPlayer = Players.GetPlayerByID(_TargetedPlayer.id);
            var gift = Instantiate(m_giftPrefab, targetPlayer.transform.position + new Vector3(0, 0, -2), new Quaternion(0, 0, 0, 0));
            var giftComponent = gift.GetComponent<Gift>();
            var neededItem = FindNeededIngredient(targetPlayer);
            giftComponent.SetIngredientType(neededItem.Type);
            giftComponent.SetContents(neededItem.Prefab);
            giftComponent.SetColor(ColorsManager.Get().PlayerMeshColors[_TargetedPlayer.id]);
            yield return null;
        }

        private SpawnableItem FindNeededIngredient(PlayerManager pm)
        {
            var cauldrons = FindObjectsOfType<CauldronStation>();
            foreach (CauldronStation cauldron in cauldrons)
            {
                if (cauldron.GetOwner().Equals(pm))
                {
                    var rm = cauldron.GetComponent<RecipeManager>();
                    Ingredient neededIngredient = rm.GetNextNeededIngredient();
                    var itemSpawner = FindObjectOfType<ItemSpawner>();
                    return itemSpawner.SpawnableItems[neededIngredient];
                }
            }
            return null;
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.gift_item;
        }
    }

}
