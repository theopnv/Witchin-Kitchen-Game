using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class LetIngredientsThroughWalls : MonoBehaviour
    {
        public const int IngredientLayer = 12, InvisibleWallLayer = 13;

        // Start is called before the first frame update
        void Start()
        {
            Physics.IgnoreLayerCollision(IngredientLayer, InvisibleWallLayer, true);
        }
    }

}
