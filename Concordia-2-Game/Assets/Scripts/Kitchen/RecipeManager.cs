using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Ingredient
{
    GHOST_PEPPER,
    NOT_AN_INGREDIENT
}

public class RecipeManager : MonoBehaviour
{
    private Recipe m_recipe;
    public int ingredientCount = 3;
    public Text counter;

    void Start()
    {
        Ingredient[] newRecipe = {Ingredient.GHOST_PEPPER, Ingredient.GHOST_PEPPER, Ingredient.GHOST_PEPPER};
        m_recipe = new Recipe(newRecipe);
        counter.text = ingredientCount + "";
    }

    public bool CollectIngredient(Ingredient collectedIngredient)
    {
        if (m_recipe.IsIngredientNeeded(collectedIngredient))
        {
            return true;
        }
        return false;
    }

    public void ProcessIngredient(Ingredient ingredient)
    {
        m_recipe.ProcessIngredient(ingredient);
        ingredientCount--;
        counter.text = ingredientCount + "";
    }

    // Update is called once per frame
    void Update()
    {
        if (m_recipe.IsComplete())
        {
            //You did it!
            counter.text = "You made a potion!";
            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            MainGameManager mgm = managers.GetComponentInChildren<MainGameManager>();
            CookingMinigame minigame = GetComponent<CookingMinigame>();
            mgm.Gameover(minigame.m_stationOwner);
        }
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

    public bool IsComplete()
    {
        return m_isComplete;
    }

    public class IngredientStatus
    {
        public Ingredient m_type;
        public bool m_processed;

        public IngredientStatus(Ingredient type)
        {
            m_type = type;
            m_processed = false;
        }
    }
}

