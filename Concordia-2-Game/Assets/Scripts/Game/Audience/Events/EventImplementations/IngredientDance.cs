using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace con2.game
{
    public class IngredientDance : MonoBehaviour
    {
        [Range(0.0f, 10.0f)]
        public float m_bounceIntensity = 5.0f;

        [Range(0.0f, 0.1f)]
        public float m_bounceFilter = 0.01f;

        private bool m_shouldDance = false;
        private List<GameObject> m_players = new List<GameObject>();
        private float m_danceDistance = 3.0f;

        private GameObject m_ground;
        private PickableObject m_thisObj;

        private MainGameManager m_mainGameManager;

        void Awake()
        {
            m_mainGameManager = FindObjectOfType<MainGameManager>();
        }

        // Use this for initialization
        void Start()
        {
            var players = m_mainGameManager.Players;
            foreach (KeyValuePair<int, PlayerManager> player in players)
            {
                m_players.Add(player.Value.gameObject);
            }

            m_ground = GameObject.Find("Ground");

            m_thisObj = GetComponent<PickableObject>();
        }

        private void Update()
        {
            if (m_shouldDance)
            {
                Wiggle();
                var body = GetComponent<Rigidbody>();
                if ((transform.position.y - m_ground.transform.position.y) < 1.5f)
                {
                    MoveAway();
                    if (Mathf.Abs(body.velocity.y) <= m_bounceFilter)
                    {
                        //Bounce
                        var vel = body.velocity;
                        vel += Vector3.up * m_bounceIntensity;
                        body.velocity = vel;
                    }
                }
            }
        }

        public void Dance(bool shouldDance)
        {
            m_shouldDance = shouldDance;
        }

        private void MoveAway()
        {
            if (!m_thisObj.IsHeld())
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
        }

        private void Wiggle()
        {
            transform.Rotate(Time.deltaTime * Vector3.up * 45);
        }
    }
}
