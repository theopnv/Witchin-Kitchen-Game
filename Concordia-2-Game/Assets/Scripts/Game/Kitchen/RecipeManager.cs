using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{
    public class RecipeManager : MonoBehaviour
    {
        private Recipe m_currentPotionRecipe;
        private int m_currentRecipeIndex = -1;
        public PlayerHUD m_recipeUI;
        private KitchenStation m_thisStation;
        private MainGameManager m_mgm;
        private ItemSpawner m_itemSpawner;
        private OnCompletePotion m_potionSpawner;
        private AudienceInteractionManager m_audienceInteractionManager;

        public bool m_TestComplete = false;

        void Start()
        {
            m_thisStation = GetComponent<KitchenStation>();
            m_itemSpawner = FindObjectOfType<ItemSpawner>();
            m_audienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            if (m_recipeUI == null)
            {
                m_recipeUI = Players.Dic[GetOwner().ID].PlayerHUD;
            }
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_mgm = managers.GetComponentInChildren<MainGameManager>();
            m_potionSpawner = m_mgm.GetComponent<OnCompletePotion>();
            NextRecipe();
        }

        void NextRecipe()
        {
            m_currentPotionRecipe = new Recipe(GlobalRecipeList.GetNextRecipe(++m_currentRecipeIndex));
            SetNewRecipeUI();
            var owner = GetOwner();
            owner.CompletedPotionCount = m_currentRecipeIndex;
            m_mgm.UpdateRanks();
            m_audienceInteractionManager.SendGameStateUpdate();
            if (m_currentRecipeIndex > 0)
            {
                var spellManager = FindObjectOfType<SpellsManager>();
                spellManager.LaunchSpellRequest(owner.ID);
            }
        }

        void SetNewRecipeUI()
        {
            m_currentPotionRecipe.SetNewRecipeUI(m_recipeUI);
        }

        void UpdateRecipeUI(Ingredient collected)
        {
            m_currentPotionRecipe.UpdateRecipeUI(m_recipeUI, collected);
        }

        public bool CollectIngredient(Ingredient collectedIngredient)
        {
            if (m_currentPotionRecipe.IsIngredientNeeded(collectedIngredient) && !m_thisStation.IsStoringIngredient())
            {
                m_currentPotionRecipe.CollectIngredient(collectedIngredient);
                UpdateRecipeUI(collectedIngredient);
                if (collectedIngredient != Ingredient.NEWT_EYE)
                {
                    m_itemSpawner.SpawnableItems[collectedIngredient]?.AskToInstantiate();
                }
                var owner = GetOwner();
                owner.CollectedIngredientCount++;
                return true;
            }
            return false;
        }

        public void ProcessIngredient(Ingredient ingredient)
        {
            m_currentPotionRecipe.ProcessIngredient(ingredient);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_currentPotionRecipe == null)
            {
                NextRecipe();
            }

            if (m_currentPotionRecipe.IsComplete())
            {
                //You did it!
                m_potionSpawner.OnPotionComplete(this);
                NextRecipe();
            }

            if (m_TestComplete)
            {
                m_TestComplete = false;
                m_potionSpawner.OnPotionComplete(this);
                NextRecipe();
            }
        }

        public int GetNumCompletedPotions()
        {
            return m_currentRecipeIndex;
        }

        public PlayerManager GetOwner()
        {
            return m_thisStation.GetOwner();
        }

        public Ingredient GetNextNeededIngredient()
        {
            return m_currentPotionRecipe.GetNextNeededIngredient();
        }
    }

    public class Recipe
    {
        List<IngredientStatus> m_fullRecipe;
        bool m_isComplete = false;

        public Recipe(Ingredient[] recipe)
        {
            m_fullRecipe = new List<IngredientStatus>();
            for (int i = 0; i < recipe.Length; i++)
            {
                m_fullRecipe.Add(new IngredientStatus(recipe[i]));
            }
        }

        public bool IsIngredientNeeded(Ingredient ingredient)
        {
            List<IngredientStatus> missingIngredientsOfThisType = m_fullRecipe.FindAll(
                delegate (IngredientStatus temp)
                {
                    return !temp.m_processed && temp.m_type == ingredient;
                }
              );
            return missingIngredientsOfThisType.Count > 0;
        }

        public void CollectIngredient(Ingredient ingredient)
        {
            var missingIngredientOfThisType = m_fullRecipe.Find(
                delegate (IngredientStatus temp)
                {
                    return !temp.m_collected && temp.m_type == ingredient;
                }
              );
            missingIngredientOfThisType.m_collected = true;
        }

        public void ProcessIngredient(Ingredient ingredient)
        {
            IngredientStatus missingIngredientOfThisType = m_fullRecipe.Find(
                delegate (IngredientStatus temp)
                {
                    return !temp.m_processed && temp.m_type == ingredient;
                }
              );
            missingIngredientOfThisType.m_processed = true;

            bool complete = true;
            foreach (IngredientStatus status in m_fullRecipe)
            {
                if (status.m_processed == false)
                    complete = false;
            }
            m_isComplete = complete;
        }

        public void UpdateRecipeUI(PlayerHUD recipeUI, Ingredient collected)
        {
            List<Image> newIcons = new List<Image>();
            foreach (IngredientStatus status in m_fullRecipe)
            {
                Image icon = GlobalRecipeList.IconSprites[status.m_type];
                newIcons.Add(icon);
            }
            recipeUI.CollectIngredient(collected);
        }

        public void SetNewRecipeUI(PlayerHUD recipeUI)
        {
            List<Image> newIcons = new List<Image>();
            foreach (IngredientStatus status in m_fullRecipe)
            {
                newIcons.Add(GlobalRecipeList.IconSprites[status.m_type]);
            }
            recipeUI.SetNewRecipeIcons(newIcons);
        }

        public bool IsComplete()
        {
            return m_isComplete;
        }

        public Ingredient GetNextNeededIngredient()
        {
            foreach (IngredientStatus i in m_fullRecipe)
            {
                if (!i.m_collected)
                {
                    return i.m_type;
                }
            }
            //If nothing needed, then return random ingredient
            return (Ingredient)Random.Range(0, (int)Ingredient.NOT_AN_INGREDIENT);
        }

        public class IngredientStatus
        {
            public Ingredient m_type;
            public bool m_collected, m_processed;

            public IngredientStatus(Ingredient type)
            {
                m_type = type;
                m_collected = false;
                m_processed = false;
            }
        }
    }


}
