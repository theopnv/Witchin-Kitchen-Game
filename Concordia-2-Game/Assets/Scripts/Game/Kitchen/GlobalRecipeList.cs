using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace con2.game
{

    public class GlobalRecipeList : MonoBehaviour
    {
        private static List<Ingredient[]> m_sharedRecipeList = new List<Ingredient[]>();

        //Synchronize this method because I'm suspicious of what will happen when all cauldrons call Start() ...
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Ingredient[] GetNextRecipe(int currentRecipeIndex)
        {
            if (currentRecipeIndex >= m_sharedRecipeList.Count)
            {
                m_sharedRecipeList.Add(GenerateRandomRecipe(Random.Range(3,6)));
            }

            return m_sharedRecipeList[currentRecipeIndex];
        }

        private static Ingredient[] GenerateRandomRecipe(int numIngredients)
        {
            Ingredient[] recipe = new Ingredient[numIngredients];
            for (int i = 0; i < numIngredients; i++)
            {
                recipe[i] = (Ingredient)Random.Range(0, (int)Ingredient.NOT_AN_INGREDIENT - 1);
            }
            return recipe;
        }
    }
}
