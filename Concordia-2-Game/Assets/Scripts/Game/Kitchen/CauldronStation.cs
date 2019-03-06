using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class CauldronStation : KitchenStation
    {
        private RecipeManager m_recipeManager;
        private Cauldron m_cauldronFX;

        protected override void OnAwake()
        {
            m_recipeManager = GetComponent<RecipeManager>();
            m_cauldronFX = GetComponent<Cauldron>();
        }

        public override bool ShouldAcceptIngredient(Ingredient type)
        {
            return m_recipeManager.CollectIngredient(type);
        }

        protected override void OnCollectIngredient()
        {
            m_cauldronFX.StartCooking();
        }

        public override void ProcessIngredient()
        {
            m_recipeManager.ProcessIngredient(m_storedIngredient);  //Add to recipe
            m_cauldronFX.StopCooking();
            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
        }
    }


}
