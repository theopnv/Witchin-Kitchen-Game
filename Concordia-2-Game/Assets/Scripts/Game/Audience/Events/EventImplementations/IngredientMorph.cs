using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class IngredientMorph : MonoBehaviour
    {
        public GameObject RemoveIngredient()
        {
            GameObject ingredient = transform.Find("Ingredient").gameObject;
            ingredient.transform.parent = null;
            return ingredient;
        }

        public void Morph(GameObject newIngredient)
        {
            newIngredient.transform.parent = transform;
            newIngredient.transform.SetPositionAndRotation(transform.position, transform.rotation);
            GetComponentInChildren<ParticleSystem>().Play();
        }
    }
}
