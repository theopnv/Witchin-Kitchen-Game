using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public abstract class KitchenStation : MonoBehaviour
    {
        private ItemSpawner m_itemSpawner;
        private ACookingMinigame[] m_miniGames;
        protected Ingredient m_storedIngredient;
        protected RecipeManager m_recipeManager;
        private PlayerManager m_owner;

        protected abstract void OnAwake();
        protected abstract void OnSetOwner(PlayerManager owner);
        public abstract bool ShouldAcceptIngredient(Ingredient type);
        protected abstract void OnCollectIngredient();
        public abstract void ProcessIngredient();
        
        private void Awake()
        {
            m_miniGames = GetComponents<ACookingMinigame>();
            m_recipeManager = GetComponent<RecipeManager>();
            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
            OnAwake();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var ingredient = collision.gameObject.GetComponent<PickableObject>();
            if (ingredient && !ingredient.IsHeld())
            {
                var ingredientType = collision.gameObject.GetComponentInChildren<IngredientType>();
                if (ingredientType && ShouldAcceptIngredient(ingredientType.m_type))
                {
                    OnCollectIngredient();
                    m_storedIngredient = ingredientType.m_type;
                    Destroy(collision.gameObject);

                    var nextMinigame = Random.Range(0, m_miniGames.Length);
                    m_miniGames[nextMinigame].StartMinigame();
                }
            }
        }

        public void SetOwner(PlayerManager owner)
        {
            m_owner = owner;
            foreach (ACookingMinigame game in m_miniGames)
            {
                game.SetStationOwner(owner, this);
            }
            OnSetOwner(owner);
        }

        public PlayerManager GetOwner() => m_owner;

        public bool IsStoringIngredient()
        {
            return m_storedIngredient != Ingredient.NOT_AN_INGREDIENT;
        }
    }

}