using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class KitchenStation : MonoBehaviour
    {
        private ACookingMinigame m_miniGame;
        private Ingredient m_storedIngredient;
        private RecipeManager m_recipeManager;
        private PlayerManager m_owner;

        protected abstract void OnAwake();
        public abstract bool ShouldAcceptIngredient(Ingredient type);
        protected abstract void OnCollectIngredient();
        public abstract void ProcessIngredient();

        private Cauldron m_cauldronFX;
        private Spin2Win m_spoonSpinner;

        private void Awake()
        {
            m_miniGame = GetComponent<ACookingMinigame>();
            m_recipeManager = GetComponent<RecipeManager>();
            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;

            m_cauldronFX = GetComponent<Cauldron>();
            m_spoonSpinner = GetComponentInChildren<Spin2Win>();
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
                    Destroy(collision.gameObject);
                    m_miniGame.StartMinigame();
                }
            }
        }

        public void SetOwner(PlayerManager owner)
        {
            m_owner = owner;
            m_miniGame.SetStationOwner(owner, this);

            //Apply player color to station?
        }


        public bool IsStoringIngredient()
        {
            return m_storedIngredient != Ingredient.NOT_AN_INGREDIENT;
        }
    }

}