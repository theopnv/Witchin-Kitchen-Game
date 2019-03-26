using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public enum Ingredient
    {
        NEWT_EYE = 0,
        MUSHROOM = 1,
        FISH = 2,
        PUMPKIN = 3,

        NOT_AN_INGREDIENT
    }

    public class IngredientType : MonoBehaviour
    {
        public Ingredient m_type = Ingredient.NOT_AN_INGREDIENT;
    }
}
