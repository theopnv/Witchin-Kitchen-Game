using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class IngredientMorphController : AbstractAudienceEvent
    {
        public float m_swapDelay = 3.0f;
        public int m_numberOfSwaps = 5;
        public GameObject m_smokePuffPrefab;

        void Start()
        {
            SetUpEvent();
        }

        public override void EventStart()
        { }

        public override IEnumerator EventImplementation()
        {
            _MessageFeedManager.AddMessageToFeed("The ingredients were morphed!", MessageFeedManager.MessageType.arena_event);

            int count = 0;
            while (count < m_numberOfSwaps)
            {
                count++;
                DoSwap();
                yield return new WaitForSeconds(m_swapDelay);
            }

            yield return null;
        }

        private void DoSwap()
        {
            IngredientMorph[] allIngredients = FindObjectsOfType<IngredientMorph>();
            List<GameObject> innerIngredients = new List<GameObject>();

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

            var players = m_mainGameManager.Players;
            for (int j = 0; j < players.Count; j++)
            {
                var pickUpSystem = players[j].GetComponent<PlayerPickUpDropObject>();
                pickUpSystem.UpdateHeldObjectWeight();
            }
        }

        public override Events.EventID GetEventID()
        {
            return Events.EventID.ingredient_morph;
        }

        private void Shuffle(List<GameObject> list)
        {
            var temp = list[0];
            list.RemoveAt(0);
            list.Add(temp);
        }
    }
}
