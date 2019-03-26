using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace con2.game
{

    public abstract class KitchenStation : MonoBehaviour
    {
        private ItemSpawner m_itemSpawner;
        private ACookingMinigame[] m_miniGames;
        protected Ingredient m_storedIngredient;
        protected ARecipeManager MARecipeManager;
        private PlayerManager m_owner;
        [SerializeField] GameObject HitPrefab;

        protected abstract void OnSetOwner(PlayerManager owner);
        public abstract bool ShouldAcceptIngredient(Ingredient type);
        protected abstract void OnCollectIngredient();
        public abstract void ProcessIngredient();

        protected virtual void Awake()
        {
            m_miniGames = GetComponents<ACookingMinigame>();
        }

        protected virtual void Start()
        {
            m_storedIngredient = Ingredient.NOT_AN_INGREDIENT;
            MARecipeManager = GetComponent<ARecipeManager>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var ingredient = collision.gameObject.GetComponent<PickableObject>();
            if (ingredient && !ingredient.IsHeld())
            {
                var ingredientType = collision.gameObject.GetComponentInChildren<IngredientType>();
                if (ingredient.ThrownIntentionallyBy().Equals(m_owner) && ingredientType && ShouldAcceptIngredient(ingredientType.m_type))
                {
                    OnCollectIngredient();
                    m_storedIngredient = ingredientType.m_type;
                    Destroy(collision.gameObject);

                    var nextMinigame = Random.Range(0, m_miniGames.Length);
                    m_miniGames[nextMinigame].StartMinigame();
                }
                else
                {
                    var burst = Instantiate(HitPrefab, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
                    StartCoroutine(DestroyWhenComplete(burst));
                }

                ingredient.ResetAimAssist();
            }
        }

        private IEnumerator DestroyWhenComplete(GameObject obj)
        {
            yield return new WaitForSeconds(1.0f);
            GameObject.Destroy(obj);
        }
        public bool IsStoringIngredient()
        {
            return m_storedIngredient != Ingredient.NOT_AN_INGREDIENT;
        }

        public PlayerManager GetOwner() => m_owner;

        public void SetOwner(PlayerManager owner)
        {
            m_owner = owner;

            var aimAssistSystem = owner.gameObject.GetComponent<PlayerPickUpDropObject>();
            aimAssistSystem.SetAimAssistedStation(this.gameObject);

            foreach (var game in m_miniGames)
            {
                game.SetOwner(m_owner);
                game.KitchenStation = this;
            }
            OnSetOwner(m_owner);
        }
    }

}