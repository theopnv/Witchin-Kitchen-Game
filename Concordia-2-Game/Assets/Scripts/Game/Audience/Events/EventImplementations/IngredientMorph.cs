using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class IngredientMorph : MonoBehaviour
    {
        AudioSource audioSource;
        public AudioClip morphSound;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

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
            var puff = GetComponentInChildren<ParticleSystem>();
            var m = puff.main;  //Apparently this has to be split into 2 lines...
            m.startDelay = 0.0f;
            puff.Play();

            audioSource.clip = morphSound;
            audioSource.Play();
        }
    }
}
