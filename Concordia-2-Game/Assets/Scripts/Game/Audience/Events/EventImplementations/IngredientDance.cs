using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace con2.game
{
    public class IngredientDance : MonoBehaviour
    {
        private bool m_shouldDance = false;
        private List<GameObject> m_players = new List<GameObject>();
        private float m_danceDistance = 3.0f;

        // Use this for initialization
        void Start()
        {
            var players = Players.Dic;
            foreach (KeyValuePair<int, PlayerManager> player in players)
            {
                m_players.Add(player.Value.gameObject);
            }
        }

        private void Update()
        {
            if (m_shouldDance)
            {
                MoveAway();
                Wiggle();
            }
        }

        public void Dance(bool shouldDance)
        {
            m_shouldDance = shouldDance;
        }

        private void MoveAway()
        {
            Vector3 pos = transform.position;
            Vector3 newpos = pos;
            foreach (var player in m_players)
            {
                Vector3 separation = pos - player.transform.position;

                if (separation.magnitude <= m_danceDistance)
                {
                    newpos += separation.normalized * Time.deltaTime * 3;
                }
            }
            transform.position = newpos;
        }

        private void Wiggle()
        {
            transform.Rotate(Time.deltaTime * Vector3.up * 45);
        }
    }
}
