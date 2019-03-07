using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class IngredientMorphController : AbstractAudienceEvent
    {
        void Start()
        {
            SetUpEvent();
        }

        public override Events.EventID GetEventID()
        {
            return Events.EventID.ingredient_morph;
        }

        public override void EventStart()
        { }

        public override IEnumerator EventImplementation()
        {
            IngredientMorph[] allIngredients = GameObject.FindObjectsOfType<IngredientMorph>();
            List<GameObject> innerIngredients = new List<GameObject>();

            m_eventText.text = "The ingredients were morphed!";

            foreach (IngredientMorph ingredient in allIngredients)
            {
                innerIngredients.Add(ingredient.RemoveIngredient());
            }

            Shuffle(innerIngredients);

            int i = 0;
            foreach (IngredientMorph ingredient in allIngredients)
            {
                ingredient.Morph(innerIngredients[i++]);
            }

            var players = Players.Dic;
            for (int j = 0; j < players.Count; j++)
            {
                var pickUpSystem = players[j].GetComponent<PlayerPickUpDropObject>();
                pickUpSystem.UpdateHeldObjectWeight();
            }

            yield return null;
        }

        private void Shuffle(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int k = Random.Range(0, list.Count);
                GameObject temp = list[k];
                list[k] = list[i];
                list[i] = temp;
            }
        }
    }
}
