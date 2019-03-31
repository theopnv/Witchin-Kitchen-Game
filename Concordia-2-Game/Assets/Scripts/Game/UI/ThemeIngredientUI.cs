using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{
    public class ThemeIngredientUI : MonoBehaviour
    {
        [SerializeField] private Image _Ingredient;

        void Start()
        {
            StartCoroutine(Disappear());
        }

        public void SetIngredientSprite(Ingredient ingredientType)
        {
            var spritesLookupTable = new Dictionary<Ingredient, string>
            {
                { Ingredient.FISH, "Fish"},
                { Ingredient.MUSHROOM, "Mushroom"},
                { Ingredient.NEWT_EYE, "Newt Eye"},
                { Ingredient.PUMPKIN, "Pumpkin"},
            };

            if (spritesLookupTable.ContainsKey(ingredientType))
            {
                _Ingredient.sprite = Resources.Load<Sprite>(spritesLookupTable[ingredientType]);
            }
        }

        private IEnumerator Disappear()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }
    }
}