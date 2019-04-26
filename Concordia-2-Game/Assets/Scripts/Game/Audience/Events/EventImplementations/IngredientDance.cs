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
        private TrailRenderer m_trail;

        private AMainManager m_mainGameManager;

        void Awake()
        {
            m_mainGameManager = FindObjectOfType<AMainManager>();
        }

        // Use this for initialization
        void Start()
        {
            var players = m_mainGameManager.PlayersInstances;
            foreach (var player in players)
            {
                m_players.Add(player.Value.gameObject);
            }

            m_ground = GameObject.Find("Ground");

            m_thisObj = GetComponent<PickableObject>();

            m_trail = GetComponentInChildren<TrailRenderer>();
            m_trail.emitting = false;

            m_headingAngle = Random.Range(0.0f, 2f * Mathf.PI);
            m_trailHeadingAngle = Random.Range(0.0f, 2f * Mathf.PI);
        }

        private float m_trailHeadingAngle = 0.0f;
        private float m_trailHeadingAngleMultiplier = 40.0f;
        private float m_trailCircleRadius = 0.3f;

        private void Update()
        {
            m_trail.emitting = m_shouldDance && !m_thisObj.IsHeld();

            if (m_shouldDance)
            {
                Wiggle();
                var body = GetComponent<Rigidbody>();
                if ((transform.position.y - m_ground.transform.position.y) < 1.7f)
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

                var dir = new Vector3(Mathf.Cos(m_trailHeadingAngle), 0.0f, Mathf.Sin(m_trailHeadingAngle));
                m_trail.transform.position = transform.position + m_trailCircleRadius * dir;

                m_trailHeadingAngle += Time.deltaTime * m_trailHeadingAngleMultiplier;
            }
        }

        public void Dance(bool shouldDance)
        {
            m_shouldDance = shouldDance;
        }

        private float m_headingAngle = 0.0f;
        private float m_headingAngleMultiplier = 2.0f;

        private void MoveAway()
        {
            if (!m_thisObj.IsHeld())
            {
                Vector3 pos = transform.position;
                Vector3 newpos = pos;
                var aPlayerIsInRange = false;

                foreach (var player in m_players)
                {
                    Vector3 separation = pos - player.transform.position;

                    if (separation.magnitude <= m_danceDistance)
                    {
                        aPlayerIsInRange = true;
                        newpos += separation.normalized * Time.deltaTime * 3;
                    }
                }

                if (!aPlayerIsInRange)
                {
                    var dir = new Vector3(Mathf.Cos(m_headingAngle), 0.0f, Mathf.Sin(m_headingAngle));
                    newpos += dir * Time.deltaTime * 1.5f;

                    m_headingAngle += Time.deltaTime * m_headingAngleMultiplier;
                }
                else
                {
                    var dir = newpos - transform.position;
                    m_headingAngle = Mathf.Atan2(newpos.y, newpos.y);
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
