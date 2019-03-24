using System;
using System.Collections;
using System.Collections.Generic;
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

        private static List<Ingredient[]> m_sharedRecipeList = new List<Ingredient[]>();

        //Synchronize this method because I'm suspicious of what will happen when all cauldrons call Start() ...
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Ingredient[] GetNextRecipe(int currentRecipeIndex)
        {
            if (currentRecipeIndex >= m_sharedRecipeList.Count)
            {
                m_sharedRecipeList.Add(GenerateRandomRecipe(UnityEngine.Random.Range(2,4)));
            }

            return m_sharedRecipeList[currentRecipeIndex];
        }

        private static Ingredient[] GenerateRandomRecipe(int numIngredients)
        {
            Ingredient[] recipe = new Ingredient[numIngredients];
            for (int i = 0; i < numIngredients; i++)
            {
                var ing = Ingredient.NOT_AN_INGREDIENT;
                int pos = 0;
                int tries = 0;
                while (pos > -1)    //no duplicate ingredients in recipes
                {
                    ing = (Ingredient)UnityEngine.Random.Range(0, (int)Ingredient.NOT_AN_INGREDIENT);
                    pos = Array.IndexOf(recipe, ing);
                }
                recipe[i] = ing;
            }
            return recipe;
        }

        #endregion
    }
}
