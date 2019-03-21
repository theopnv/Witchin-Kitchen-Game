using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public enum Ingredient
    {
        PEPPER,
        NEWT_EYE,
        MUSHROOM,
        FISH,
        PUMPKIN,
        NOT_AN_INGREDIENT
    }

    public class IngredientType : MonoBehaviour
    {
        public Ingredient m_type = Ingredient.NOT_AN_INGREDIENT;
    }
}
