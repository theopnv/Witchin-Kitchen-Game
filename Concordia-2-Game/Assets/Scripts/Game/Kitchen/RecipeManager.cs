using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public enum Ingredient
    {
        GHOST_PEPPER,
        NEWT_EYE,
        NOT_AN_INGREDIENT
    }

    public class RecipeManager : MonoBehaviour
    {
        private Recipe m_currentPotionRecipe;
        private int m_currentRecipeIndex = -1;
        public Text m_recipeUI, m_score;
        private KitchenStation m_thisStation;
        private MainGameManager m_mgm;

        void Start()
        {
            m_thisStation = GetComponent<KitchenStation>();
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_mgm = managers.GetComponentInChildren<MainGameManager>();
            NextRecipe();
        }

        void NextRecipe()
        {
            if (m_recipeUI == null)
            {
                m_recipeUI = Players.Dic[m_thisStation.GetOwner().ID].PlayerHUD.Recipe;
            }
            if (m_score == null)
            {
                m_score = Players.Dic[m_thisStation.GetOwner().ID].PlayerHUD.Score;
            }

            m_currentPotionRecipe = new Recipe(GlobalRecipeList.GetNextRecipe(++m_currentRecipeIndex));
            m_recipeUI.text = m_currentPotionRecipe.GetRecipeUI();
            var owner = m_thisStation.GetOwner();
            owner.Score = m_currentRecipeIndex;
            m_score.text = owner.Score.ToString();
            m_mgm.UpdateRanks();
        }

        public bool CollectIngredient(Ingredient collectedIngredient)
        {
            if (m_currentPotionRecipe.IsIngredientNeeded(collectedIngredient) && !m_thisStation.IsStoringIngredient())
            {
                m_currentPotionRecipe.CollectIngredient(collectedIngredient);
                m_recipeUI.text = m_currentPotionRecipe.GetRecipeUI();
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
            if (m_currentPotionRecipe.IsComplete())
            {
                //You did it!
                m_recipeUI.text = "You made a potion, keep going!";
                NextRecipe();
            }
        }

        public int GetNumCompletedPotions()
        {
            return m_currentRecipeIndex;
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
            IngredientStatus missingIngredientOfThisType = m_fullRecipe.Find(
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

        public string GetRecipeUI()
        {
            string recipeUI = "";
            foreach (IngredientStatus status in m_fullRecipe)
            {
                if (status.m_collected == false)
                {
                    recipeUI += "X - " + status.m_type + "\n";
                }
                else
                {
                    char checkmark = '\u2713';
                    recipeUI += checkmark.ToString() + " - " + status.m_type + "\n";
                }
            }
            return recipeUI;
        }

        public bool IsComplete()
        {
            return m_isComplete;
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
