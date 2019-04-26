using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace con2.game
{
    public class IngredientDanceEvent : AbstractAudienceEvent
    {
        public float m_ingredientDanceDuration = 20.0f;

        void Start()
        {
            SetUpEvent();
        }

        public override void EventStart()
        { }

        public override IEnumerator EventImplementation()
        {
            IngredientDance[] allIngredients = FindObjectsOfType<IngredientDance>();

            _MessageFeedManager.AddMessageToFeed("The ingredients are alive!", MessageFeedManager.MessageType.arena_event);

            foreach (IngredientDance ingredient in allIngredients)
            {
                ingredient.Dance(true);
            }

            yield return new WaitForSeconds(m_ingredientDanceDuration);

            foreach (IngredientDance ingredient in allIngredients)
            {
                ingredient.Dance(false);
            }
        }

        public override Events.EventID GetEventID()
        {
            return Events.EventID.ingredient_dance;
        }

    }
}
