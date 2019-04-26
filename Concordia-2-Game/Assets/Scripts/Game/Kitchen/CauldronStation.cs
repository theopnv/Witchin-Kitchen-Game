using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class CauldronStation : KitchenStation
    {
        private Cauldron m_cauldronFX;

        protected override void Awake()
        {
            base.Awake();
            m_cauldronFX = GetComponent<Cauldron>();
        }

        public override bool ShouldAcceptIngredient(Ingredient type)
        {
            return MARecipeManager.CollectIngredient(type);
        }

        protected override void OnCollectIngredient()
        {
            m_cauldronFX.StartCooking();
        }

        public override void ProcessIngredient()
        {
            MARecipeManager.ProcessIngredient(m_storedIngredient);  //Add to recipe
            m_cauldronFX.StopCooking();
            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
        }
        
        // Override liquid and bubbles colors per player
        protected override void OnSetOwner(PlayerManager owner)
        {
            var liquid = transform.Find("Liquid");
            var matOverrideLiquid = liquid.gameObject.AddComponent<gfx.MaterialOverride>();
            
            var liquidColor = ColorsManager.Get().CauldronLiquidColors[owner.ID];
            matOverrideLiquid.Entries.Add(new gfx.MaterialOverride.Entry("_Color", liquidColor));
            matOverrideLiquid.Apply();

            var bubbles = transform.Find("Bubbles");
            var matOverrideBubbles = bubbles.gameObject.AddComponent<gfx.MaterialOverride>();

            var bubblesColor = ColorsManager.Get().CauldronBubblesColors[owner.ID];
            matOverrideBubbles.Entries.Add(new gfx.MaterialOverride.Entry("_EmissionColor", bubblesColor));
            matOverrideBubbles.Apply();
        }
    }


}
