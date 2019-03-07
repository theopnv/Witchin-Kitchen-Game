using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin2Win : MonoBehaviour 
{
    Vector2 m_currentRotation;
    float m_targetAngle;
    int m_rotationDir;

    void Update()
    {
        float dif = Mathf.DeltaAngle(transform.eulerAngles.y, m_targetAngle);
        if (Math.Abs(dif) > 10)
            transform.Rotate(Vector3.up * 600 * Time.deltaTime * m_rotationDir * (Math.Abs(dif)/90.0f));
    }

    public void SetTargetYAngle(float newTarget)
    {
        m_targetAngle = newTarget;
    }

    public void SetTargetRotation(int newTarget)
    {
        m_rotationDir = newTarget;
    }
}

