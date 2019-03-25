using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public class GlobalRecipeList : MonoBehaviour
    {
        #region UI

        [SerializeField]
        [Tooltip("List of ingredient icons")]
        public List<IngredientIcon> IngredientIcons = new List<IngredientIcon>();
        public static Dictionary<Ingredient, Image> IconSprites;

        private void Start()
        {
            IconSprites = new Dictionary<Ingredient, Image>();
            foreach (var item in IngredientIcons)
            {
                IconSprites.Add(item.Type, item.Prefab);
            }
        }

        #endregion

        #region Mechanics

        public static Ingredient m_featuredIngredient;
        private static List<Ingredient[]> m_sharedRecipeList = new List<Ingredient[]>();

        //Synchronize this method because I'm suspicious of what will happen when all cauldrons call Start() ...
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Ingredient[] GetNextRecipe(int currentRecipeIndex)
        {
            if (currentRecipeIndex >= m_sharedRecipeList.Count)
            {
                m_sharedRecipeList.Add(GenerateRandomRecipe(Random.Range(2,4)));
            }

            return m_sharedRecipeList[currentRecipeIndex];
        }

        private static Ingredient[] GenerateRandomRecipe(int numIngredients)
        {
            var recipe = new Ingredient[numIngredients];
            recipe[0] = m_featuredIngredient;
            for (var i = 1; i < numIngredients; i++)
            {
                recipe[i] = (Ingredient)Random.Range(0, (int)Ingredient.NOT_AN_INGREDIENT);
            }
            var rnd = new System.Random();
            recipe = recipe.OrderBy(x => rnd.Next()).ToArray();
            return recipe;
        }

       #endregion
    }
}
