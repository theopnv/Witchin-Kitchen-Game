using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPunchable: MonoBehaviour, IPunchable
{
    Rigidbody m_rb;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public void Punch(Vector3 knockVelocity, float stunTime)
    {
        m_rb.AddForce(knockVelocity, ForceMode.VelocityChange);
    }
}
