using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace con2.game
{

    public class GlobalRecipeList : MonoBehaviour
    {
        private static List<Ingredient[]> m_sharedRecipeList = new List<Ingredient[]>();

        private static List<Ingredient[]> m_potionRecipes = new List<Ingredient[]> {
            new[] { Ingredient.GHOST_PEPPER, Ingredient.GHOST_PEPPER },
            new[] { Ingredient.NEWT_EYE, Ingredient.NEWT_EYE },
            new[] { Ingredient.NEWT_EYE, Ingredient.GHOST_PEPPER, Ingredient.GHOST_PEPPER },
            new[] { Ingredient.GHOST_PEPPER, Ingredient.NEWT_EYE, Ingredient.GHOST_PEPPER }
        };

        //Synchronize this method because I'm suspicious of what will happen when all cauldrons call Start() ...
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Ingredient[] GetNextRecipe(int currentRecipeIndex)
        {
            if (currentRecipeIndex >= m_sharedRecipeList.Count)
            {
                int nextIndex = Random.Range(0, m_potionRecipes.Count);
                while (m_sharedRecipeList.Count > 0 
                    && m_potionRecipes[nextIndex].Equals(m_sharedRecipeList[m_sharedRecipeList.Count - 1]))
                {
                    nextIndex = Random.Range(0, m_potionRecipes.Count);
                }
                Ingredient[] next = m_potionRecipes[nextIndex];
                m_sharedRecipeList.Add(m_potionRecipes[nextIndex]);
            }

            return m_sharedRecipeList[currentRecipeIndex];
        }
    }
}
