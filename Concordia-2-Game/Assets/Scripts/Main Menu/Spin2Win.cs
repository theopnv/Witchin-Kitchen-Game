using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin2Win : MonoBehaviour 
{
    bool m_spin = false;

    void Update()
    {
        if (m_spin)
        {
            transform.Rotate(new Vector3(0, 280, 0) * Time.deltaTime);
        }
    }

    public void SetToSpin(bool shouldSpin)
    {
        m_spin = shouldSpin;
    }
}

