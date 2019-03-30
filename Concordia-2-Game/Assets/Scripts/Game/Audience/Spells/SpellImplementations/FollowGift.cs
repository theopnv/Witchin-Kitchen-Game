using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace con2.game
{
    public class FollowGift : MonoBehaviour
    {
        [Range(0.0f, 10.0f)]
        public float m_bounceIntensity = 5.0f;

        [Range(0.0f, 0.1f)]
        public float m_bounceFilter = 0.01f;

        private Transform m_followTarget;
        [SerializeField] private float m_followDistance = 3.0f, m_followSpeed = 5.0f;

        private GameObject m_ground;
        private Rigidbody m_rb;

        // Use this for initialization
        void Start()
        {
            m_ground = GameObject.Find("Ground");
            m_rb = GetComponent<Rigidbody>();
        }

        public void SetFollowTarget(Transform toFollow)
        {
            m_followTarget = toFollow;
        }

        private void Update()
        {
            if (m_followTarget != null)
            {
                Follow();
                if ((transform.position.y - m_ground.transform.position.y) < 1.7f)
                {
                    if (Mathf.Abs(m_rb.velocity.y) <= m_bounceFilter)
                    {
                        //Bounce
                        var vel = m_rb.velocity;
                        vel += Vector3.up * m_bounceIntensity;
                        m_rb.velocity = vel;
                    }
                }
            }
        }

        private void Follow()
        {
            Vector3 separation = transform.position - m_followTarget.position;
            if (separation.magnitude >= m_followDistance)
            {
                transform.position -= separation.normalized * Time.deltaTime * m_followSpeed;
            }
        }
    }
}
