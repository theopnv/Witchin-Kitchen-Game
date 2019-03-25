using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public enum Ingredient
    {
        PEPPER = 0,
        NEWT_EYE = 1,
        MUSHROOM = 2,
        FISH = 3,
        PUMPKIN = 4,

        NOT_AN_INGREDIENT
    }

    public class IngredientType : MonoBehaviour
    {
        public Ingredient m_type = Ingredient.NOT_AN_INGREDIENT;
    }
}
