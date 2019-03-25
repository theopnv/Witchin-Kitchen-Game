using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.messages;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{
    public class RecipeManagerGame : ARecipeManager
    {
        private MainGameManager m_mgm;

        protected override void Awake()
        {
            base.Awake();

            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_mgm = managers.GetComponentInChildren<MainGameManager>();
            m_potionSpawner = m_mgm.GetComponent<OnCompletePotion>();
            m_audienceInteractionManager.OnReceivedIngredientPollResults += OnReceivedIngredientPollResults;
        }

        protected override void Start()
        {
            base.Start();
        }

        void OnDisable()
        {
            m_audienceInteractionManager.OnReceivedIngredientPollResults -= OnReceivedIngredientPollResults;
        }

        private void OnReceivedIngredientPollResults(IngredientPoll poll)
        {
            GlobalRecipeList.m_featuredIngredient = (game.Ingredient)poll.ingredients.OrderByDescending(i => i.votes).First().id;
        }

        protected override void NextRecipe()
        {
            m_currentPotionRecipe = new Recipe(GlobalRecipeList.GetNextRecipe(++m_currentRecipeIndex));
            SetNewRecipeUI();
            Owner.CompletedPotionCount = m_currentRecipeIndex;
            m_mgm.UpdateRanks();
            m_audienceInteractionManager.SendGameStateUpdate();
            if (m_currentRecipeIndex > 0)
            {
                var spellManager = FindObjectOfType<SpellsManager>();
                spellManager.LaunchSpellRequest(Owner.ID);
            }
        }

        public override void ProcessIngredient(Ingredient ingredient)
        {
            m_currentPotionRecipe.ProcessIngredient(ingredient);
        }

        protected override AMainManager GetMainManager() => m_mgm;

    }


}
