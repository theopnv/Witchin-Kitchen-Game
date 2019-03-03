using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorFriction : MonoBehaviour
{
    [SerializeField] private float m_frictionCoefficient = 0.2f;
    private Rigidbody m_rb;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 modifiedVelocity = m_rb.velocity;

        if (Math.Abs(modifiedVelocity.y) <= 0.1 && modifiedVelocity.magnitude > 0.001)  //Do not horizontal apply friction when falling
        {
            modifiedVelocity.x *= 1 - m_frictionCoefficient;
            modifiedVelocity.z *= 1 - m_frictionCoefficient;
            m_rb.velocity = modifiedVelocity;
        }
    }

    public void ModulateFriction(float frictionFraction)
    {
        m_frictionCoefficient *= frictionFraction;
    }
}
