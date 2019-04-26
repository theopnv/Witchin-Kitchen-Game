using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float m_speed = 6.0f;
        [SerializeField] private float m_flyTime = 2.0f;

        void Start()
        {
            var explosive = GetComponent<ExplosiveItem>();
            explosive.ExplodeByTime(m_flyTime);
            explosive.ExplodeOnContact();

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * m_speed;
        }
    }
}
