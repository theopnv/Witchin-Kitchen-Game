using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace con2.game
{

    public class GlobalRecipeList : MonoBehaviour
    {
        #region UI

        [SerializeField]
        [Tooltip("List of ingredient icons")]
        public List<IngredientIcon> IngredientIcons = new List<IngredientIcon>();
        public static Dictionary<Ingredient, Image> IconSprites;
        public static Sprite completedIngredient;
        public Sprite completedIngredientSprite;

        private void Start()
        {
            IconSprites = new Dictionary<Ingredient, Image>();
            foreach (var item in IngredientIcons)
            {
                IconSprites.Add(item.Type, item.Prefab);
            }
            completedIngredient = completedIngredientSprite; //can't set static sprites in the inspector...

            m_sharedRecipeList = new List<Ingredient[]>();
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
            var recipe = new Ingredient[numIngredients];
            for (int j = 0; j < numIngredients; j++)    //otherwise the whole array initializes to value 0 of Ingredient, and then that ingredient is filtered out of all recipes by the duplicates filter
            {
                recipe[j] = Ingredient.NOT_AN_INGREDIENT;
            }
            var i = 0;

            if ((Ingredient)GameInfo.ThemeIngredient == Ingredient.NOT_AN_INGREDIENT)
            {
                GameInfo.ThemeIngredient = Random.Range(0, (int)Ingredient.NOT_AN_INGREDIENT);
            }
            recipe[i++] = (Ingredient)GameInfo.ThemeIngredient;
            for (; i < numIngredients; i++)
            {
                var ing = Ingredient.NOT_AN_INGREDIENT;
                int pos = 0;
                while (pos > -1)    //no duplicate ingredients in recipes
                {
                    ing = (Ingredient)UnityEngine.Random.Range(0, (int)Ingredient.NOT_AN_INGREDIENT);
                    pos = Array.IndexOf(recipe, ing);
                }
                recipe[i] = ing;
            }
            var rnd = new System.Random();
            recipe = recipe.OrderBy(x => rnd.Next()).ToArray();
            return recipe;
        }

       #endregion
    }
}
