using System;
using System.Collections;
using System.Collections.Generic;
using con2.lobby;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{
    public class RecipeManagerLobby : ARecipeManager
    {
        public Action OnProcessedIngredient;
        public Action OnCompletedPotion;
        private LobbyManager m_mgm;

        protected override AMainManager GetMainManager() => m_mgm;

        protected override void Awake()
        {
            base.Awake();

            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_mgm = managers.GetComponentInChildren<LobbyManager>();
            m_potionSpawner = m_mgm.GetComponent<OnCompletePotion>();
        }

        protected override void Update()
        {
            if (m_currentPotionRecipe.IsComplete())
            {
                OnCompletedPotion?.Invoke();
            }
            base.Update();
        }

        protected override void NextRecipe()
        {
            var list = new Ingredient[2]
            {
                Ingredient.MUSHROOM,
                Ingredient.MUSHROOM
            };
            m_currentPotionRecipe = new Recipe(list);
            SetNewRecipeUI();
            Owner.CompletedPotionCount = ++m_currentRecipeIndex;
        }

        public override void ProcessIngredient(Ingredient ingredient)
        {
            m_currentPotionRecipe.ProcessIngredient(ingredient);
            OnProcessedIngredient?.Invoke();
        }
    }


}
