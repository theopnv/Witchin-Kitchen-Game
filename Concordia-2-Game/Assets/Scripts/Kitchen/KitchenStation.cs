using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenStation : MonoBehaviour
{
    private CookingMinigame m_miniGame;
    private Ingredient m_storedIngredient;
    [SerializeField]
    private bool m_hasOwner;
    private RecipeManager m_recipeManager;

    private void Awake()
    {
        m_miniGame = GetComponent<CookingMinigame>();
        m_recipeManager = GetComponent<RecipeManager>();
        m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickableObject ingredient = collision.gameObject.GetComponent<PickableObject>();
        if (ingredient)
        {
            if (m_recipeManager == null || m_recipeManager.CollectIngredient(ingredient.ingredientType))    //If is generic station, or is cauldron and needs the ingredient 
            {
                m_storedIngredient = ingredient.ingredientType;
                collision.gameObject.SetActive(false);
                m_miniGame.StartMinigame();
            }
        }
    }

    public void SetOwner(GameObject owner)
    {
        m_miniGame.SetStationOwner(owner, this);

        //Apply player color to station?

    }

    public void ProcessIngredient()
    {
        if (m_recipeManager)    //Is cauldron
        {
            m_recipeManager.ProcessIngredient(m_storedIngredient);  //Add to recipe
        }
        else                //Is some other kitchen station
        {
            //Process ingredient (chop, smash, etc.)
        }

        m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
    }

    public bool CollectIngredient(Ingredient type)
    {
        return m_recipeManager.CollectIngredient(type);
    }
}
