using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{
    public abstract class ARecipeManager : MonoBehaviour
    {
        protected Recipe m_currentPotionRecipe;
        protected int m_currentRecipeIndex = -1;
        [HideInInspector] public PlayerHUD m_recipeUI;
        [HideInInspector] public PlayerManager Owner;
        protected KitchenStation m_thisStation;
        protected ItemSpawner m_itemSpawner;
        protected OnCompletePotion m_potionSpawner;
        protected AudienceInteractionManager m_audienceInteractionManager;

        [HideInInspector] private ScoringFeedback m_scoringFeedback;

        public bool m_TestComplete = false;

        protected virtual void Awake()
        {
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_thisStation = GetComponent<KitchenStation>();
            m_itemSpawner = managers.GetComponentInChildren<ItemSpawner>();
            m_audienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            m_scoringFeedback = GetComponentInChildren<ScoringFeedback>();
        }

        protected virtual void Start()
        {
            if (m_recipeUI == null)
            {
                var player = GetMainManager().GetPlayerById(Owner.ID);
                m_recipeUI = player.PlayerHUD;
            }

            NextRecipe();
        }

        protected abstract void NextRecipe();
        protected abstract AMainManager GetMainManager();

        protected void SetNewRecipeUI()
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
                    --m_itemSpawner.SpawnedItemsCount[collectedIngredient];
                    m_itemSpawner.SpawnableItems[collectedIngredient]?.AskToInstantiate();
                }
                Owner.CollectedIngredientCount++;
                return true;
            }
            return false;
        }

        public virtual void ProcessIngredient(Ingredient ingredient)
        {
            m_scoringFeedback.ActivateProcessedIngredient();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (m_currentPotionRecipe == null)
            {
                NextRecipe();
            }

            if (m_currentPotionRecipe.IsComplete())
            {
                //You did it!
                m_scoringFeedback.ActivateScored();
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
            var missingIngredientOfThisType = m_fullRecipe.Find(
                temp => !temp.m_processed && temp.m_type == ingredient
            );
            missingIngredientOfThisType.m_processed = true;

            var complete = true;
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
