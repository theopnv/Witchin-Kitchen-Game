using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin2Win : MonoBehaviour 
{
    bool m_spin = false;
    float m_targetAngle = 0.0f;

    void Update()
    {
        if (m_spin)
        {
            float currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, m_targetAngle, Time.deltaTime * 5);
            transform.eulerAngles = new Vector3(0.0f, currentAngle, 0.0f);
        }
    }

    public void SetTargetYAngle(float newTarget)
    {
        m_targetAngle = newTarget;
    }

    public void SetToSpin(bool shouldSpin)
    {
        m_spin = shouldSpin;
    }
}

