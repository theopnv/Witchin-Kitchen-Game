using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace con2.game
{
    public class FishFlop : MonoBehaviour
    {
        private float m_bounceIntensity = 1.5f;

        [Range(0.0f, 0.1f)]
        public float m_bounceFilter = 0.01f;

        private GameObject m_ground;
        private PickableObject m_thisObj;

        // Use this for initialization
        void Start()
        {
            m_ground = GameObject.Find("Ground");
            m_thisObj = GetComponent<PickableObject>();
        }

        private void Update()
        {
            Wiggle();
            var body = GetComponentInParent<Rigidbody>();
            if (Mathf.Abs(body.velocity.y) <= m_bounceFilter && (transform.position.y - m_ground.transform.position.y) < 1.0f)
            {
                //Bounce
                var vel = body.velocity;
                vel += Vector3.up * m_bounceIntensity;
                body.velocity = vel;
            }
        }

        private void Wiggle()
        {
            transform.Rotate(Time.deltaTime * Vector3.up * 5);
        }
    }
}
