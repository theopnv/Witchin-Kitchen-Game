using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KitchenStation : MonoBehaviour
{
    private ACookingMinigame m_miniGame;
    protected Ingredient m_storedIngredient;

    protected abstract void OnAwake();
    public abstract bool ShouldAcceptIngredient(Ingredient type);
    protected abstract void OnCollectIngredient();
    public abstract void ProcessIngredient();

    private void Awake()
    {
        m_miniGame = GetComponent<ACookingMinigame>();
        m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;

        OnAwake();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickableObject ingredient = collision.gameObject.GetComponent<PickableObject>();
        if (ingredient && !ingredient.IsHeld())
        {
            if (ShouldAcceptIngredient(ingredient.m_ingredientType))
            {
                OnCollectIngredient();
                m_storedIngredient = ingredient.m_ingredientType;
                GameObject.Destroy(collision.gameObject);
                m_miniGame.StartMinigame();
            }
        }
    }

    public void SetOwner(GameObject owner)
    {
        m_miniGame.SetStationOwner(owner, this);

        //Apply player color to station?

    }
       
    public bool IsStoringIngredient()
    {
        return m_storedIngredient != Ingredient.NOT_AN_INGREDIENT;
    }
}
